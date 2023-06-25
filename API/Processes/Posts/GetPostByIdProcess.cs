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
        public string PostImageUrl { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public Guid UserId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<PostEntity, Response>()
                .ForMember(p => p.PostImageUrl, dest => dest.MapFrom(src => src.Images.OrderByDescending(x => x.CreatedAt).FirstOrDefault()!.ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.PostId);

            var postFromDb = await _context.Posts
                .Include(i => i.Images)
                .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken: cancellationToken);

            if (postFromDb is null)
            {
                return Result<Response>.Failure(new List<string> { });
            }

            var postToReturn = _mapper.Map<Response>(postFromDb);

            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var postIsLikedByCurrentUser = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == currentUserId && l.PostId == postFromDb.Id,
                    cancellationToken: cancellationToken);

            if (postIsLikedByCurrentUser is not null)
            {
                postToReturn.IsLikedByCurrentUser = true;
            }

            return Result<Response>.Success(postToReturn);
        }
    }
}
