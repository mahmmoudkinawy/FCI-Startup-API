namespace API.Processes.Images;

public sealed class UploadImageProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public IFormFile? ImageFile { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(image => image.ImageFile)
                .NotNull()
                .WithMessage("File is required.")
                .DependentRules(() =>
                {
                    RuleFor(image => image.ImageFile.Length)
                        .LessThanOrEqualTo(10 * 1024 * 1024)
                        .WithMessage("File size must be less than or equal to 10MB");

                    RuleFor(image => image.ImageFile.ContentType)
                        .Must(contentType => contentType == "image/jpeg" || contentType == "image/png")
                        .WithMessage("Invalid file type. Only JPEG and PNG files are allowed.");
                });
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AlumniDbContext _context;

        public Handler(
            IPhotoService photoService,
            IHttpContextAccessor httpContextAccessor,
            AlumniDbContext context)
        {
            _photoService = photoService ??
                throw new ArgumentNullException(nameof(photoService));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _context.Users
                .Include(p => p.Images)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            var imageUploadResult = await _photoService.UploadPhotoAsync(request.ImageFile);

            if (string.IsNullOrWhiteSpace(imageUploadResult))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "An error occured while uploading the image."
                });
            }

            var image = new ImageEntity
            {
                Id = Guid.NewGuid(),
                IsMain = user.Images.Count == 0,
                UserId = userId,
                ImageUrl = imageUploadResult
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }

}
