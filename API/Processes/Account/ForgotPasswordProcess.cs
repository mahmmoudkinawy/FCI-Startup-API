namespace API.Processes.Account;
public sealed class ForgotPasswordProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Email { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(_ => _.Email)
                .EmailAddress()
                .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IEmailSender _emailSender;

        public Handler(
            UserManager<UserEntity> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _emailSender = emailSender ??
                throw new ArgumentNullException(nameof(emailSender));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var frontendResetLink = $"http://localhost:4200/reset-password?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

                var emailTemplateContent = File.ReadAllText("wwwroot/Templates/Forgot-Password.html");

                emailTemplateContent = emailTemplateContent.Replace("[PasswordResetLink]", frontendResetLink)
                                                           .Replace("[UserName]", $"{user.FirstName}. {user.LastName}");

                await _emailSender.SendEmailAsync(
                    request.Email,
                    "Alumni - Reset Your Password",
                    emailTemplateContent);
            }

            return Result<Response>.Success(new Response { });
        }
    }
}
