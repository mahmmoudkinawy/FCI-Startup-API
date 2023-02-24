namespace API.Processes.Posts;

public sealed class DeletePostProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid PostId { get; set; }
    }

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

            var post = await _context.Posts
                .FirstOrDefaultAsync(p =>
                    p.Id == request.PostId &&
                    p.UserId == userId, cancellationToken: cancellationToken);

            if (post is null)
            {
                return Result<Response>.Failure(new List<string> { });
            }

            _context.Posts.Remove(post);

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
