namespace API.Processes.Posts;

public sealed class GetAllPostsProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>>
    { }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<PostEntity, Response>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
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

        public async Task<IReadOnlyList<Response>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            var postsFromDb = await _context.Posts.ToListAsync(cancellationToken: cancellationToken);

            var postsToReturn = _mapper.Map<IReadOnlyList<Response>>(postsFromDb);

            return postsToReturn;
        }
    }
}
