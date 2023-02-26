namespace API.Services;

public interface IPhotoService
{
    Task<string> UploadPhotoAsync(IFormFile imageFile);
}
