namespace API.DbContexts;
public sealed class AlumniDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options)
    { }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<ImageEntity> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PostEntity>()
            .HasOne(u => u.User)
            .WithMany(p => p.Posts);

        builder.Entity<ImageEntity>()
            .HasOne(u => u.User)
            .WithMany(p => p.Images);
    }

}
