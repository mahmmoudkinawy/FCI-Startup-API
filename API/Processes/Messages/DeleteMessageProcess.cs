namespace API.Processes.Messages;
public sealed class DeleteMessageProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid MessageId { get; set; }
    }

    public sealed class Response { }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            AlumniDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var message = await _context.Messages
                .FindAsync(new object?[] { request.MessageId },
                    cancellationToken: cancellationToken);

            if (message is null)
            {
                return Result<Response>.Failure(new List<string> { "Message does not exist." });
            }

            if (message.SenderId != userId && message.RecipientId != userId)
            {
                return Result<Response>.Failure(new List<string> { "You don't Own this Message." });
            }

            if (message.SenderId == userId)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientId == userId)
            {
                message.RecipientDeleted = true;
            }

            if (message.RecipientDeleted && message.SenderDeleted)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }

    }

}
