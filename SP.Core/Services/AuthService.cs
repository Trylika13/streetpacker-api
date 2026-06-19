using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SP.Core.Interfaces.Repositories;
using SP.Core.Interfaces.Services;
using SP.Domain.Entities;
namespace SP.Core.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    public AuthService(IUserRepository userRepository, IConfiguration config, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _config = config;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<User> RegisterAsync(User user, string clearPassword)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
        if (existingUser != null)
            throw new Exception("User already exists"); 

        // Hasher le mot de passe 
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(clearPassword);
        
        user.Role = "User"; 
        
        return await _userRepository.AddUserAsync(user);
    }

    public async Task<(string Token, string RefreshToken)?> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if ( user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        
        var jwt =  GenerateJwtToken(user);
        var refreshTokenEntity = GenerateRefreshToken(user.UserId);
        
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        
        return (jwt, refreshTokenEntity.Token);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // CLAIMS
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    
    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(randomNumber),
            ExpiresAt = DateTime.UtcNow.AddDays(7), 
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            IsRevoked = false
        };
    }
    
    public async Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var savedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
    
        if (savedRefreshToken == null) {
            Console.WriteLine("❌ Token non trouvé en base !");
            return null;
        }

        if (!savedRefreshToken.IsActive) {
            Console.WriteLine("❌ Token invalide (révoqué ou expiré) !");
            Console.WriteLine("❌ Token déjà révoqué !");
            return null;
        }

        if (savedRefreshToken.User == null) {
            Console.WriteLine("❌ Utilisateur non chargé ! (Problème d'Include)");
            return null;
        }
        
        var newAccessToken = GenerateJwtToken(savedRefreshToken.User!);
        var newRefreshTokenEntity = GenerateRefreshToken(savedRefreshToken.UserId);
        
        savedRefreshToken.IsRevoked = true;

        await _refreshTokenRepository.UpdateAsync(savedRefreshToken);
        
        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
        
        return (newAccessToken, newRefreshTokenEntity.Token);
    }
    
}