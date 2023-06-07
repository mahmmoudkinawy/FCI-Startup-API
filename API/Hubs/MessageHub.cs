namespace API.Hubs;

/// <summary>
/// Authorized Message Hub that's accessed via 'hubs/message'.
/// </summary>
[SignalRHub]
[Authorize]
public sealed class MessageHub : Hub
{
    private readonly AlumniDbContext _context;
    private readonly IMapper _mapper;

    public MessageHub(AlumniDbContext context, IMapper mapper)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// ReceiveMessageThread Hub method method that's used for receiving a message.
    /// <returns>Returns nothing.</returns>
    /// </summary>
    [SignalRMethod("ReceiveMessageThread")]
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUserFullName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var callerUserId = Context.User.GetUserById();
        var otherUserId = await _context.Users.Where(u => u.UserName == otherUser).Select(u => u.Id).FirstOrDefaultAsync();

        var messages = await _context.Messages
            .Include(u => u.Sender)
                .ThenInclude(i => i.Images)
            .Include(u => u.Recipient)
                .ThenInclude(i => i.Images)
            .Where
                (
                    m =>
                        m.Recipient.Id == callerUserId && !m.RecipientDeleted &&
                        m.Sender.Id == otherUserId ||
                        m.Recipient.Id == otherUserId && !m.SenderDeleted &&
                        m.Sender.Id == callerUserId
                )
            .OrderBy(m => m.MessageSentDate)
            .ToListAsync();

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    /// <summary>
    /// NewMessage Hub method method that's used for sending a message to another user.
    /// <returns>Returns the messages.</returns>
    /// </summary>
    [SignalRMethod("NewMessage")]
    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var currentUserId = Context.User.GetUserById();
        var currentUser = await _context.Users.FindAsync(currentUserId);

        if (currentUser.UserName == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot send messages to yourself");

        var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser.UserName);
        var recipient = await _context.Users.FirstOrDefaultAsync(u => u.UserName == createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not Found user");

        var message = new MessageEntity
        {
            Id = Guid.NewGuid(),
            Sender = sender,
            Recipient = recipient,
            SenderName = sender.UserName,
            RecipientName = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _context.Groups.FindAsync(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }

        _context.Messages.Add(message);

        if (await _context.SaveChangesAsync() > 0)
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await RemoveFromMessageGroup(Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    private async Task RemoveFromMessageGroup(string connectionId)
    {
        var connection = await _context.Connections.FindAsync(connectionId);
        _context.Connections.Remove(connection);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await _context.Groups.FindAsync(groupName);

        var connection = new ConnectionEntity
        {
            ConnectionId = Context.ConnectionId,
            Username = Context.User.GetUserFullName()
        };

        if (group is null)
        {
            group = new GroupEntity
            {
                Name = groupName
            };
            _context.Groups.Add(group);
        }

        group.Connections.Add(connection);

        return await _context.SaveChangesAsync() > 0;
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

}

public sealed class CreateMessageDto
{
    public string RecipientUsername { get; set; }
    public string Content { get; set; }
}

public sealed class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; }
    public string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; }
    public string RecipientPhotoUrl { get; set; }
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}

public sealed class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<MessageEntity, MessageDto>();
    }
}