var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerServices();

builder.Services.AddIdentityServices(); 

builder.Services.AddConfigureCors();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(Constants.CorsPolicyName);

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AlumniDbContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
try
{
    await dbContext.Database.MigrateAsync();
    await Seed.SeedUsers(userManager);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured while applying pending migrations");
}

await app.RunAsync();