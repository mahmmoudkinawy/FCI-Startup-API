namespace API.Processes.Posts;

public sealed class GetPostByIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid PostId { get; set; }
    }

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

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var postFromDb = await _context.Posts.FindAsync(
                new object?[] { request.PostId },
                cancellationToken: cancellationToken);

            if(postFromDb is null)
            {
                return Result<Response>.Failure(new List<string> { });
            }

            var postToReturn = _mapper.Map<Response>(postFromDb);

            return Result<Response>.Success(postToReturn);
        }
    }
}
