namespace API.Processes.Posts;
public sealed class CreatePostProcess
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
        public string ImageMetadata { get; set; }
        public Guid UserId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, PostEntity>()
                .ForMember(r => r.CreatedAt, o => o.MapFrom(d => DateTime.UtcNow));

            CreateMap<PostEntity, Response>();
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IClarityImage _clarityImage;

        public Handler(
            AlumniDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IPhotoService photoService,
            IClarityImage clarityImage)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _photoService = photoService ??
                throw new ArgumentNullException(nameof(mapper));
            _clarityImage = clarityImage ??
                throw new ArgumentNullException(nameof(clarityImage));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _context.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);

            var postToAdd = _mapper.Map<PostEntity>(request);

            user.Posts.Add(postToAdd);

            if(request.Image is not null)
            {
                var imageUploadUri = await _photoService.UploadPhotoAsync(request.Image);

                var imageMetaDataResult = await _clarityImage.GetResultsAsync(imageUploadUri);

                var imageToCreate = new ImageEntity
                {
                    Id = Guid.NewGuid(),
                    Post = postToAdd,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = imageUploadUri,
                    ImageMetadata = imageMetaDataResult
                };

                _context.Images.Add(imageToCreate);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                var postToReturn = _mapper.Map<Response>(postToAdd);

                return Result<Response>.Success(postToReturn);
            }

            return Result<Response>.Failure(new List<string>
            {
                "An error had occurred"
            });
        }

    }
}
