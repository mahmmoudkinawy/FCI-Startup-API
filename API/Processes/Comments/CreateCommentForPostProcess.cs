namespace API.Processes.Comments;
public sealed class CreateCommentForPostProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid? PostId { get; set; }
        public string Content { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CommentEntity, Response>()
                .ForMember(dest => dest.OwnerImageUrl, opt => opt.MapFrom(src => src.Owner.Images.MaxBy(i => i.CreatedAt).ImageUrl));
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(m => m.Content)
                .NotEmpty()
                .NotNull()
                .MaximumLength(1000);
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

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
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var idValueFromRoute = _httpContextAccessor.HttpContext?.GetRouteValue("postId");

            var postId = Guid.Parse(idValueFromRoute.ToString());

            request.PostId ??= postId;

            var post = await _context.Posts
                .FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);

            if (post is null)
            {
                return Result<Response>.Failure(new List<string> { "There is not post with the given Id." });
            }

            var comment = new CommentEntity
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                OwnerId = currentUserId,
                PostId = post.Id
            };

            _context.Comments.Add(comment);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                var commentToReturn = _mapper.Map<Response>(comment);

                return Result<Response>.Success(commentToReturn);
            }

            return Result<Response>.Failure(new List<string>
            {
                "An error had occurred"
            });
        }

    }
}
