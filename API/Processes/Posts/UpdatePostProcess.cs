namespace API.Processes.Posts;

public sealed class UpdatePostProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, PostEntity>()
                .ForMember(e => e.UpdatedAt, m => m.MapFrom(d => DateTime.UtcNow));
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(_ => _.Content)
                .MinimumLength(3)
                .MaximumLength(50000)
                .NotEmpty();

            RuleFor(_ => _.Image)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(BeAValidImage)
                .When(_ => _.Image != null)
                .WithMessage("Invalid image format or size.");
        }

        private bool BeAValidImage(IFormFile file)
        {
            if (file == null)
            {
                return true;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".gif", ".png" };
            var maxFileSizeBytes = 10 * 1024 * 1024;

            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                return false;
            }

            if (file.Length > maxFileSizeBytes)
            {
                return false;
            }

            return true;
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;

        public Handler(
            AlumniDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IPhotoService photoService)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _photoService = photoService ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var idValueFromRoute = requestRouteQuery!.Values["postId"];

            var postId = Guid.Parse(idValueFromRoute.ToString());

            var postFromDb = await _context.Posts
                .FirstOrDefaultAsync(p =>
                    p.Id == postId &&
                    p.UserId == userId, cancellationToken: cancellationToken);

            if (postFromDb is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Post does not exist."
                });
            }

            _mapper.Map(request, postFromDb);

            if (request.Image is not null)
            {
                var imageUploadResult = await _photoService.UploadPhotoAsync(request.Image);

                var imageToCreate = new ImageEntity
                {
                    Id = Guid.NewGuid(),
                    PostId = postFromDb.Id,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = imageUploadResult
                };

                _context.Images.Add(imageToCreate);
            }

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
