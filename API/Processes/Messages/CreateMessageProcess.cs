namespace API.Processes.Messages;
public sealed class CreateMessageProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Content { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string SenderName { get; set; }
        public string SenderImageUrl { get; set; }
        public string RecipientName { get; set; }
        public string RecipientImageUrl { get; set; }
        public string Content { get; set; }
        public DateTime DateRead { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<MessageEntity, Response>()
                .ForMember(dest => dest.SenderImageUrl, opt => opt.MapFrom(u => u.Sender.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
                .ForMember(dest => dest.RecipientImageUrl, opt => opt.MapFrom(u => u.Recipient.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl));
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.Content)
                .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            AlumniDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var recipientIdValueFromRoute = requestRouteQuery!.Values["recipientId"];

            var recipientId = Guid.Parse(recipientIdValueFromRoute.ToString());

            if (userId == recipientId)
            {
                return Result<Response>.Failure(new List<string> { "You can't message yourself." });
            }

            var sender = await _context.Users
                .Include(i => i.Images)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);

            var recipient = await _context.Users
                .Include(i => i.Images)
                .FirstOrDefaultAsync(u => u.Id == recipientId, cancellationToken: cancellationToken);

            if (recipient is null)
            {
                return Result<Response>.Failure(new List<string> { "Recipient does not exist." });
            }

            var message = new MessageEntity
            {
                Id = Guid.NewGuid(),
                Sender = sender,
                Recipient = recipient,
                MessageSentDate = DateTime.UtcNow,
                RecipientName = $"{recipient.FirstName} {recipient.LastName}",
                SenderName = $"{sender.FirstName} {sender.LastName}",
                Content = request.Content
            };

            _context.Messages.Add(message);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                var messageToReturn = _mapper.Map<Response>(message);
                return Result<Response>.Success(messageToReturn);
            }

            return Result<Response>.Failure(new List<string> { "Something went wrong." });
        }

    }

}
