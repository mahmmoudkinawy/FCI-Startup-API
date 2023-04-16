namespace API.Processes.Messages;
public sealed class GetMessagesForUserProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Container { get; set; } = "Unread";
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

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var query = _context.Messages
                .OrderByDescending(m => m.MessageSentDate)
                .AsQueryable();

            query = request.Container switch
            {
                "Inbox" => query.Where(m => m.RecipientId == userId && !m.RecipientDeleted),
                "Outbox" => query.Where(m => m.SenderId == userId && !m.SenderDeleted),
                _ => query.Where(m => m.RecipientId == userId && !m.RecipientDeleted && m.DateRead == null)
            };

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);
        }
    }

}
