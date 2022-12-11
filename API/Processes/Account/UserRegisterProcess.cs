namespace API.Processes.Account;
public sealed class UserRegisterProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime GraduationYear { get; set; }
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

            RuleFor(_ => _.FirstName)
                .NotEmpty();

            RuleFor(_ => _.LastName)
                .NotEmpty();

            RuleFor(_ => _.Gender)
                .NotEmpty();

            RuleFor(_ => _.DateOfBirth)
                .LessThan(DateTime.Now.Date)
                .NotEmpty();

            RuleFor(_ => _.GraduationYear)
                .LessThan(DateTime.Now.Date)
                .GreaterThan(new DateTime(2003, 1, 1))
                .NotEmpty();

            RuleFor(_ => _.Password)
                .NotEmpty();
        }
    }

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, UserEntity>()
                .ForMember(_ => _.UserName, _ => _.MapFrom(_ => _.Email));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<Result<Response>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);

            if (userExists is not null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "User already exists"
                });
            }

            var user = _mapper.Map<UserEntity>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return Result<Response>.Failure(errors);
            }

            return Result<Response>.Success(new Response
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Token = _tokenService.CreateToken(user)
            });
        }
    }

}
