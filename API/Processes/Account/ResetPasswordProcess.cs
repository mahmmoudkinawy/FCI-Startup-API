namespace API.Processes.Account;
public sealed class ResetPasswordProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Password { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(_ => _.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var userId = httpContext.Request.Query["userId"];
            var token = httpContext.Request.Query["token"];

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "User with this Id does not exist"
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<Response>.Failure(errors);
            }

            return Result<Response>.Success(new Response { });
        }
    }
}
