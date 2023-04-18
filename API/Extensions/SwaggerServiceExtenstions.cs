﻿using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Extensions;
public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(opts =>
        {
            opts.AssumeDefaultVersionWhenUnspecified = true;
            opts.ReportApiVersions = true;
            opts.DefaultApiVersion = new ApiVersion(1, 0);
        });

        services.AddVersionedApiExplorer(opts => opts.GroupNameFormat = "'v'VVV");

        services.AddSwaggerGen(opts =>
        {
            opts.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);

            opts.CustomSchemaIds(opts => opts.FullName?.Replace("+", "."));

            opts.DocInclusionPredicate((docName, apiDesc) => true);

            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

            opts.IncludeXmlComments(xmlCommentsFullPath);

            opts.AddSecurityDefinition("AlunmiApiBearerAuth", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "Input a valid token to access this API"
            });

            opts.AddSecurityRequirement(new OpenApiSecurityRequirement
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
