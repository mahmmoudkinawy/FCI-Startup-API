namespace API.Processes.Posts;

public sealed class UpdatePostProcess
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
                .ForMember(e => e.UpdatedAt, m => m.MapFrom(d => DateTime.UtcNow));
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var idValueFromRoute = requestRouteQuery!.Values["postId"];

            var postId = Guid.Parse(idValueFromRoute.ToString());

            var postFromDb = await _context.Posts
                .FirstOrDefaultAsync(p =>
                    p.Id == postId &&
                    p.UserId == userId, cancellationToken: cancellationToken);

            if (postFromDb is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Post does not exist."
                });
            }

            _mapper.Map(request, postFromDb);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "An error had occured"
            });
        }
    }
}
