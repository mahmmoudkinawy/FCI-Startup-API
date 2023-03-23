namespace API.Processes.Users;

public sealed class CurrentUserProcess
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime GraduationYear { get; set; }
        public ICollection<ImageDto> Images { get; set; }
    }

    public sealed class ImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>()
                .ForMember(dest => dest.Age, o => o.MapFrom(u => u.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(u => $"{u.FirstName} {u.LastName}"));
            CreateMap<ImageEntity, ImageDto>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _userManager.Users
                .Include(i => i.Images)
                .Where(p => p.Id == userId)
                .ProjectTo<Response>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return user;
        }
    }
}
