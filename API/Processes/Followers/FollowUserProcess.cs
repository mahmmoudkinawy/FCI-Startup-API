namespace API.Processes.Followers;

public sealed class FollowerUserProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid DestinationUserId { get; set; }
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
            var sourceUser = await _context.Users
                .Include(c => c.FollowersUsers)
                .FirstOrDefaultAsync(c => c.Id == userId, cancellationToken: cancellationToken);

            var destinationUser = await _context.Users
                .FindAsync(new object?[] { request.DestinationUserId }, cancellationToken: cancellationToken);

            if (destinationUser is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "User you want to follow does not exist."
                });
            }

            if (sourceUser.Id == destinationUser.Id)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "You can not follow yourself."
                });
            }

            var userFollow = await _context.Followers
                .FirstOrDefaultAsync(u => u.SourceUserId == userId && u.DestinationUserId == destinationUser.Id,
                 cancellationToken: cancellationToken);

            if (userFollow is not null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "You are already follow this user."
                });
            }

            userFollow = new UserFollowEntity
            {
                SourceUserId = userId,
                DestinationUserId = request.DestinationUserId
            };

            sourceUser.FollowersUsers.Add(userFollow);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Problem follow the user"
            });
        }

    }
}

