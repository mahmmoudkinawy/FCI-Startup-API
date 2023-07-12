namespace API.Services;
public interface IClarityImage
{
    Task<string> GetResultsAsync(string imageUrl);
}
