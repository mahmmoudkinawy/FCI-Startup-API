namespace API.Extenstions;
public static class IdentityServiceExtenstions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
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

        services.AddAuthentication();

        return services;
    }
}
