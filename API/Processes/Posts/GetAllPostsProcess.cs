namespace API.Processes.Posts;
public sealed class GetAllPostsProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
        public string PostImageUrl { get; set; }
        public string ImageMetadata { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<PostEntity, Response>()
                .ForMember(p => p.OwnerId, dest => dest.MapFrom(src => src.User.Id))
                .ForMember(p => p.OwnerName, dest => dest.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(p => p.OwnerImageUrl, dest => dest.MapFrom(src => src.User.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
                .ForMember(p => p.ImageMetadata, dest => dest.MapFrom(src => src.Images.OrderByDescending(x => x.CreatedAt)!.FirstOrDefault()!.ImageMetadata))
                .ForMember(p => p.PostImageUrl, dest => dest.MapFrom(src => src.Images.OrderByDescending(x => x.CreatedAt).FirstOrDefault()!.ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(AlumniDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
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
            var postsQuery = _context.Posts
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                postsQuery = postsQuery.Where(_ => _.Content.ToLower().Contains(request.Keyword));
            }

            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var likedPostIdsByCurrentUser = await _context.Likes
                .Where(l => l.UserId == currentUserId)
                .Select(l => l.PostId)
                .ToListAsync(cancellationToken: cancellationToken);

            var responses = await PagedList<Response>.CreateAsync(
                postsQuery.ProjectTo<Response>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);

            foreach (var response in responses)
            {
                response.IsLikedByCurrentUser = likedPostIdsByCurrentUser.Contains(response.Id);
            }

            return responses;
        }

    }
}
