namespace API.Processes.Account;
public sealed class ChangePasswordProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public sealed class Response { }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserEntity> _userManager;

        public Handler(IHttpContextAccessor httpContextAccessor,
            UserManager<UserEntity> userManager)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string> { "User does not exist" });
            }

            var result = await _userManager.ChangePasswordAsync(user,
                request.CurrentPassword,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<Response>.Failure(errors);
            }

            return Result<Response>.Success(new Response { });
        }
    }

}
