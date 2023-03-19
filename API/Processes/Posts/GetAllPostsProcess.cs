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
                .ForMember(p => p.OwnerImageUrl, dest => dest.MapFrom(src => src.User.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IMapper _mapper;

        public Handler(AlumniDbContext context, IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            var query = _context.Posts.AsQueryable();

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
