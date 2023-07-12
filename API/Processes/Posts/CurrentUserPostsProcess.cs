namespace API.Processes.Posts;
public sealed class CurrentUserPostsProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>> { }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
        public string ImageMetadata { get; set; }
        public string ImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<PostEntity, Response>()
                .ForMember(dest => dest.ImageMetadata, opt => opt.MapFrom(src => src.Images.MaxBy(c => c.CreatedAt).ImageMetadata))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images.MaxBy(c => c.CreatedAt).ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            AlumniDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var query = _context.Posts
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .Where(p => p.UserId == userId)
                .AsQueryable();

            var result = _mapper.Map<IReadOnlyList<Response>>(query);

            return result;
        }
    }

}
