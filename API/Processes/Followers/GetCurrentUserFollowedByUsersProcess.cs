namespace API.Processes.Followers;

public sealed class GetCurrentUserFollowedByUsersProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>> { }

    public sealed class Response
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime GraduationYear { get; set; }
        public string ImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(u => u.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(u => $"{u.FirstName} {u.LastName}"))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(o => o.Images.FirstOrDefault(p => p.IsMain).ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
    {
        private readonly AlumniDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            AlumniDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var followers = await _context.Followers
                .Include(c => c.SourceUser)
                .Where(u => u.DestinationUserId == userId)
                .Select(u => u.SourceUser)
                .ProjectTo<Response>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken);

            return followers;
        }

    }
}
