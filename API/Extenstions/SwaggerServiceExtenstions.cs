namespace API.Extenstions;
public static class SwaggerServiceExtenstions
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(_ =>
        {
            _.AssumeDefaultVersionWhenUnspecified = true;
            _.ReportApiVersions = true;
            _.DefaultApiVersion = new ApiVersion(1, 0);
        });

        services.AddVersionedApiExplorer(_ => _.GroupNameFormat = "'v'VVV");

        services.AddSwaggerGen(_ =>
        {
            _.CustomSchemaIds(_ => _.FullName?.Replace("+", "."));

            _.AddSecurityDefinition("AlunmiApiBearerAuth", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "Input a valid token to access this API"
            });

            _.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "AlunmiApiBearerAuth" }
                    }, new List<string>() }
            });
        });

        return services;
    }
}
