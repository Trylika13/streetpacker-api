using SP.Domain.Entities;

namespace SP.Core.Interfaces.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(User user, string clearPassword);
    
    Task<(string Token, string RefreshToken)?> LoginAsync(string username, string password);
    
    Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken);
}