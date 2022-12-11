namespace API.DbContexts;
public static class Seed
{
    public static async Task SeedUsers(UserManager<UserEntity> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var users = new List<UserEntity>
        {
            new UserEntity
            {
                UserName = "bob",
                Email = "bob@test.com",
                Gender = "male",
                GraduationYear = new DateTime(2022, 07, 07),
                DateOfBirth = new DateTime(2000, 01, 01),
                FirstName = "bob",
                LastName = "bob"
            },
            new UserEntity
            {
                UserName = "lisa",
                Email = "lisa@test.com",
                Gender = "female",
                GraduationYear = new DateTime(2021, 07, 07),
                DateOfBirth = new DateTime(1999, 01, 15),
                FirstName = "lisa",
                LastName = "lisa"
            }
        };

        foreach (var user in users)
        {
            await userManager.CreateAsync(user, "Pa$$w0rd");
        }

    }
}
