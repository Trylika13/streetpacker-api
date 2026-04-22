using Microsoft.Extensions.Configuration;
using SP.Core.Interfaces.Repositories;
using SP.Core.Interfaces.Services;
using SP.Domain.Entities;

namespace SP.Core.Services;

public class UserService : IUserService

{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
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
}