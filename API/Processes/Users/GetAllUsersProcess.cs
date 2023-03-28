namespace API.Processes.Users;

public sealed class GetAllUsersProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>> { }

    public sealed class Response
    {
        public Guid Id { get; set; }
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

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var users = await _userManager.Users
                .Include(i => i.Images)
                .Where(u => u.Id != userId)
                .ProjectTo<Response>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken);

            return users;
        }
    }
}
