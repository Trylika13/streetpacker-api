namespace SP.Core.Interfaces.Services;

public interface IMediaService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
}
