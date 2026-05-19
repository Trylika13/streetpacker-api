using System.Text.Json.Serialization;

namespace SP.Presentation.Dtos;

public class TokenRequestDto
{
    [JsonPropertyName("token")]
    public required string AccessToken { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}