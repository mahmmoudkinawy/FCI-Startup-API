namespace API.Processes.Messages;
public sealed class GetMessageThreadProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>>
    {
        public Guid RecipientId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderImageUrl { get; set; }
        public Guid RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientImageUrl { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<MessageEntity, Response>()
              .ForMember(dest => dest.SenderImageUrl, opt => opt.MapFrom(u => u.Sender.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
              .ForMember(dest => dest.RecipientImageUrl, opt => opt.MapFrom(u => u.Recipient.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
              .ForMember(dest => dest.MessageSent, opt => opt.MapFrom(u => u.MessageSentDate));
        }
    }

    public sealed class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

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

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            if (userId == request.RecipientId)
            {
                return null!;
            }

            if(!await _context.Users
                .AnyAsync(u => u.Id == request.RecipientId, cancellationToken: cancellationToken))
            {
                return null;
            }

            var messages = await _context.Messages
                .Include(u => u.Sender)
                    .ThenInclude(i => i.Images)
                .Include(u => u.Recipient)
                    .ThenInclude(i => i.Images)
                .Where
                    (
                        m =>
                            m.RecipientId == userId &&
                            m.SenderId == request.RecipientId ||
                            m.RecipientId == request.RecipientId &&
                            m.RecipientId == userId
                    )
                .OrderBy(m => m.MessageSentDate)
                .ToListAsync(cancellationToken: cancellationToken);

            var unreadMessages = messages
                .Where(m => m.DateRead == null && m.RecipientId == userId)
                .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            return _mapper.Map<IReadOnlyList<Response>>(unreadMessages);
        }

    }

}
