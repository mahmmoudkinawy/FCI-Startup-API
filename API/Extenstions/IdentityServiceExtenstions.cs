namespace API.Extenstions;
public static class IdentityServiceExtenstions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddIdentityCore<UserEntity>(_ =>
        {
            _.Password.RequireNonAlphanumeric = false;
            _.SignIn.RequireConfirmedAccount = false;
            _.SignIn.RequireConfirmedPhoneNumber = false;
        })
            .AddUserManager<UserManager<UserEntity>>()
            .AddSignInManager<SignInManager<UserEntity>>()
            .AddEntityFrameworkStores<AlumniDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config[Constants.TokenKey]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
