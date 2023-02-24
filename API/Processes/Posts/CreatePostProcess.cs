namespace API.Processes.Posts;

public sealed class CreatePostProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Content { get; set; }
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
            CreateMap<Request, PostEntity>()
                .ForMember(r => r.CreatedAt, o => o.MapFrom(d => DateTime.UtcNow));

            CreateMap<PostEntity, Response>();
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(_ => _.Content)
                .MinimumLength(3)
                .MaximumLength(50000)
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _context.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);

            var postToAdd = _mapper.Map<PostEntity>(request);

            user.Posts.Add(postToAdd);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                var postToReturn = _mapper.Map<Response>(postToAdd);

                return Result<Response>.Success(postToReturn);
            }

            return Result<Response>.Failure(new List<string>
            {
                "An error had occured"
            });
        }

    }
}
