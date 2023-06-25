namespace API.Processes.Users;

public sealed class GetUserByIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid UserId { get; set; }
    }

    public sealed class Response
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime GraduationYear { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<PostDto> Posts { get; set; }
    }

    public sealed class PostDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
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
            CreateMap<PostEntity, PostDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(i => i.Images.MaxBy(pi => pi.CreatedAt).ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _userManager.Users
                .Include(i => i.Images)
                .Include(i => i.Posts)
                    .ThenInclude(i => i.Images)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string> { "User does not exist" });
            }

            return Result<Response>.Success(_mapper.Map<Response>(user));
        }
    }
}
