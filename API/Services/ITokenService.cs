namespace API.Services;
public interface ITokenService
{
    string CreateToken(UserEntity user);
}
