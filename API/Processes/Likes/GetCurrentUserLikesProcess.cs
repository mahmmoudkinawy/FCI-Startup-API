namespace API.Processes.Likes;
public sealed class GetCurrentUserLikesProcess
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
        public string OwnerName { get; set; }
        public string OwnerImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<PostEntity, Response>()
                .ForMember(p => p.OwnerName, dest => dest.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(p => p.OwnerImageUrl, dest => dest.MapFrom(src => src.User.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
                .ForMember(p => p.PostImageUrl, dest => dest.MapFrom(src => src.Images.OrderByDescending(x => x.CreatedAt).FirstOrDefault()!.ImageUrl));
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

        public async Task<PagedList<Response>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var query = _context.Likes
                .Include(l => l.Post)
                    .ThenInclude(p => p.Images)
                .Include(u => u.User)
                .Where(u => u.UserId == userId)
                .Select(u => u.Post)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                // I know the above code is very bad and ToLower() will make it slow!
                // But it's only small api for graduation.
                query = query.Where(_ => _.Content.ToLower().Contains(request.Keyword));
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }

}
