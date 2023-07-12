namespace API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSignalR();

        services.AddControllers()
            .AddFluentValidation(_ => _.RegisterValidatorsFromAssemblyContaining<Program>());

        services.AddSingleton<PresenceTracker>();

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IClarityImage, ClarityImage>();

        services.AddScoped<IPhotoService, PhotoService>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddMediatR(_ => _.RegisterServicesFromAssemblyContaining<Program>());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddDbContext<AlumniDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString(Constants.DefaultConnection)));

        return services;
    }
}
