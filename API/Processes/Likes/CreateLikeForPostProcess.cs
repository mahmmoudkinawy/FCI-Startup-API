namespace API.Processes.Likes;
public sealed class CreateLikeForPostProcess
{
    public sealed class Request : IRequest<Result<Response>> { }

    public sealed class Response { }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            AlumniDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var idValueFromRoute = requestRouteQuery!.Values["postId"];

            var postId = Guid.Parse(idValueFromRoute.ToString());

            var post = await _context.Posts
                .FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);

            if (post is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Sorry, we couldn't find the Post with the provided ID. Please check the ID and try again later."
                });
            }

            var userLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId,
                    cancellationToken: cancellationToken);

            if (userLike is not null)
            {
                _context.Likes.Remove(userLike);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Response>.Success(new Response { });
            }

            var like = new LikeEntity
            {
                Id = Guid.NewGuid(),
                LikedAt = DateTime.UtcNow,
                PostId = postId,
                UserId = userId
            };

            _context.Likes.Add(like);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "An error had occurred"
            });
        }

    }
}
