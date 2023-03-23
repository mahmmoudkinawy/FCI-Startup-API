namespace API.DbContexts;
public sealed class AlumniDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AlumniDbContext(DbContextOptions<AlumniDbContext> options) : base(options)
    { }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<ImageEntity> Images { get; set; }
    public DbSet<UserFollowEntity> Followers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PostEntity>()
            .HasOne(u => u.User)
            .WithMany(p => p.Posts);

        builder.Entity<ImageEntity>()
            .HasOne(u => u.User)
            .WithMany(p => p.Images);

        builder.Entity<UserFollowEntity>()
            .HasKey(k => new { k.SourceUserId, k.DestinationUserId });

        builder.Entity<UserFollowEntity>()
            .HasOne(u => u.SourceUser)
            .WithMany(u => u.FollowersUsers)
            .HasForeignKey(u => u.SourceUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserFollowEntity>()
            .HasOne(u => u.DestinationUser)
            .WithMany(u => u.FollowedByUsers)
            .HasForeignKey(u => u.DestinationUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

}
