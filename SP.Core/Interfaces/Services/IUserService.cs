namespace SP.Core.Interfaces.Services;

using SP.Domain.Entities;

public interface IUserService
{
    Task<User> RegisterAsync(User user, string clearPassword);
    
    Task<(string Token, string RefreshToken)?> LoginAsync(string username, string password);
    
    Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken);
}