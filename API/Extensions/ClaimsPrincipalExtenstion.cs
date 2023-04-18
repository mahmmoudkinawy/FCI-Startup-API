namespace API.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserById(this ClaimsPrincipal claims)
        => Guid.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static string GetUserFullName(this ClaimsPrincipal claims)
        => claims.FindFirstValue(ClaimTypes.Name)!;
}
