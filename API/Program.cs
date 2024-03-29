var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerServices();

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddConfigureCors();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI(_ => _.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

//app.UseHttpsRedirection(); // Just for working with WebSockets in development.

app.UseCors(Constants.CorsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapHub<CommentHub>("hubs/comment");

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AlumniDbContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    await dbContext.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, dbContext);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured while applying pending migrations");
}

await app.RunAsync();