namespace API.Extenstions;

public static class ClaimsPrincipalExtenstion
{
    public static Guid GetUserById(this ClaimsPrincipal claims)
        => Guid.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier));
}
