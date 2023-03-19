namespace API.DbContexts;
public static class Seed
{
    public static async Task SeedUsers(
        UserManager<UserEntity> userManager,
        AlumniDbContext context)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(context));
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(userManager));

        if (await userManager.Users.AnyAsync()) return;

        var users = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.Parse("daa04b47-6c3d-4823-8ded-17e0f524d355"),
                UserName = "bob",
                Email = "bob@test.com",
                Gender = "male",
                GraduationYear = new DateTime(2022, 07, 07),
                DateOfBirth = new DateTime(2000, 01, 01),
                FirstName = "bob",
                LastName = "bob",
                Images = new List<ImageEntity>
                {
                    new ImageEntity
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "https://res.cloudinary.com/delibahag/image/upload/v1659010398/pds0qovxofpqepve2upw.png",
                        IsMain = true
                    },
                    new ImageEntity
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "https://res.cloudinary.com/delibahag/image/upload/v1656798900/wtgiatqk4oq7wzquwbuk.png",
                        IsMain = false
                    }
                }
            },
            new UserEntity
            {
                Id = Guid.Parse("6fafacd7-80fa-48c2-9dff-f12c01aa25ed"),
                UserName = "lisa",
                Email = "lisa@test.com",
                Gender = "female",
                GraduationYear = new DateTime(2021, 07, 07),
                DateOfBirth = new DateTime(1999, 01, 15),
                FirstName = "lisa",
                LastName = "lisa",
                Images = new List<ImageEntity>
                {
                    new ImageEntity
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "https://res.cloudinary.com/delibahag/image/upload/v1657117381/axdvspc6ngv40rvx6sfn.png",
                        IsMain = false
                    },
                    new ImageEntity
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "https://res.cloudinary.com/delibahag/image/upload/v1659015026/nujemzz7jy9ya49jjuhc.png",
                        IsMain = true
                    }
                }
            }
        };

        foreach (var user in users)
        {
            await userManager.CreateAsync(user, "Pa$$w0rd");    
        }

        var usersIds = new List<Guid>()
        {
            new Guid("6fafacd7-80fa-48c2-9dff-f12c01aa25ed"),
            new Guid("daa04b47-6c3d-4823-8ded-17e0f524d355")
        };

        var fakerPosts = new Faker<PostEntity>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Content, f => f.Lorem.Text())
            .RuleFor(p => p.UserId, f => usersIds[Random.Shared.Next(0, usersIds.Count)]);

        var posts = fakerPosts.GenerateLazy(21);

        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();
    }
}
