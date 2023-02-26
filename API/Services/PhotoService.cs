namespace API.Services;

public sealed class PhotoService : IPhotoService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _config;

    public PhotoService(BlobServiceClient blobServiceClient, IConfiguration config)
    {
        _blobServiceClient = blobServiceClient ??
            throw new ArgumentNullException(nameof(blobServiceClient));
        _config = config ??
            throw new ArgumentNullException(nameof(config));
    }

    public async Task<string> UploadPhotoAsync(IFormFile imageFile)
    {

        var containerName = _config[Constants.AzureBlobContainerName];

        using var stream = imageFile.OpenReadStream();

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var imageName = $"{imageFile.FileName}";
        var blobClient = containerClient.GetBlobClient(imageName);
        var options = new BlobHttpHeaders
        {
            ContentType = "image/jpeg"
        };

        var result = await blobClient.UploadAsync(stream, options);

        return $"{_blobServiceClient.Uri}{containerName}/{imageName}";
    }
}
