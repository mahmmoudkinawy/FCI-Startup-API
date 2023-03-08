namespace API.Services;

public sealed class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IConfiguration config)
    {
        _cloudinary = new Cloudinary(
            new Account
            {
                Cloud = config[Constants.CloudinarySettings.CloudName],
                ApiKey = config[Constants.CloudinarySettings.ApiKey],
                ApiSecret = config[Constants.CloudinarySettings.ApiSecret]
            });
    }

    public async Task<string> UploadPhotoAsync(IFormFile imageFile)
    {
        var uploadResult = new ImageUploadResult();

        if (imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult.Url.AbsoluteUri;
    }

}
