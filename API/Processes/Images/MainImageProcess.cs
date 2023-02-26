namespace API.Processes.Images;

public sealed class MainImageProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ImageId { get; set; }
    }

    public sealed class Response { }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AlumniDbContext _context;

        public Handler(
            IHttpContextAccessor httpContextAccessor,
            AlumniDbContext context)
        {
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

            var images = user.Images.ToList();

            if (!images.Any())
            {
                return Result<Response>.Failure(new List<string>
                {
                    "You do not have images to be set as main photo."
                });
            }

            if (images.Count == 1)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "You have only one photo and it's the main photo."
                });
            }

            var oldImage = images.FirstOrDefault(i => i.IsMain);

            if (oldImage.Id == request.ImageId)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Already the main photo."
                });
            }

            if (oldImage is not null)
            {
                oldImage.IsMain = false;
            }

            var imageToUpdate = images.FirstOrDefault(i => i.Id == request.ImageId);

            if (imageToUpdate is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "You don't have the permissions for to do this."
                });
            }

            imageToUpdate.IsMain = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }

    }
}
