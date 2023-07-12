namespace API.Processes.Comments;
public sealed class GetCommentsByPostIdProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>>
    {
        public Guid PostId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CommentEntity, Response>()
                .ForMember(dest => dest.OwnerImageUrl, opt => opt.MapFrom(src => src.Owner.Images.MaxBy(i => i.CreatedAt).ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IMapper _mapper;

        public Handler(
            AlumniDbContext context,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var comments = await _context.Comments
                .Include(c => c.Owner)
                    .ThenInclude(c => c.Images)
                .Where(c => c.PostId == request.PostId)
                .ToListAsync(cancellationToken: cancellationToken);

            return _mapper.Map<IReadOnlyList<Response>>(comments);
        }
    }
}
