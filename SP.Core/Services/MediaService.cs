using Microsoft.Extensions.Configuration;
using SP.Core.Interfaces.Services;
using System.Net.Http.Headers;

namespace SP.Core.Services;

public class MediaService : IMediaService
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;
    private readonly string _bucketName = "images";

    public MediaService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _supabaseUrl = config["Supabase:Url"] ?? "";
        _supabaseKey = config["Supabase:Key"] ?? "";
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var requestUrl = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{uniqueFileName}";

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        request.Headers.Add("apikey", _supabaseKey);

        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erreur Supabase Storage : {error}");
        }

        return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{uniqueFileName}";
    }
}
