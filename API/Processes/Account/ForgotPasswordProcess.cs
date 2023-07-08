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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender,
            IUrlHelperFactory urlHelperFactory)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _emailSender = emailSender ??
                throw new ArgumentNullException(nameof(emailSender));
            _urlHelperFactory = urlHelperFactory ??
                throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var httpContext = _httpContextAccessor.HttpContext;
                var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(),
                    new ControllerActionDescriptor());

                var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

                var resetPasswordLink = urlHelper.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token
                });

                var forgotPasswordRequest = httpContext.Request;
                var baseUrl = $"{forgotPasswordRequest.Scheme}://{forgotPasswordRequest.Host}{forgotPasswordRequest.PathBase}{resetPasswordLink}";

                var emailTemplateContent = File.ReadAllText("wwwroot/Templates/Forgot-Password.html");

                emailTemplateContent = emailTemplateContent.Replace("[PasswordResetLink]", baseUrl)
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
