namespace API.DbContexts;
public sealed class AlumniDbContext : IdentityDbContext<UserEntity>
{
    public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options)
    { }

}
