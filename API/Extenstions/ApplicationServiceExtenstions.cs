namespace API.Extenstions;
public static class ApplicationServiceExtenstions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddControllers()
            .AddFluentValidation(_ => _.RegisterValidatorsFromAssemblyContaining<Program>());

        services.AddScoped<ITokenService, TokenService>();

        services.AddMediatR(_ => _.RegisterServicesFromAssemblyContaining<Program>());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddDbContext<AlumniDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString(Constants.DefaultConnection)));

        return services;
    }
}
