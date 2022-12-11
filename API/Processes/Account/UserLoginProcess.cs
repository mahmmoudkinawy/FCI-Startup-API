namespace API.Processes.Account;
public sealed class UserLoginProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public sealed class Response
    {
        public string FullName { get; set; }
        public string Token { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(_ => _.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(_ => _.Password)
                .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;

        public Handler(
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<Response>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Email or Password is invalid"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Email or Password is invalid"
                });
            }

            return Result<Response>.Success(new Response
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Token = "Token must go here"
            });
        }
    }

}
