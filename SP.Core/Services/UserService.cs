using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SP.Core.Interfaces.Repositories;
using SP.Core.Interfaces.Services;
using SP.Domain.Entities;

namespace SP.Core.Services;

public class UserService : IUserService

{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    
    public UserService(IConfiguration config, IUserRepository userRepository)
    {
        _config = config;
        _userRepository = userRepository;
    }
    
    public async Task<User> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new Exception("Utilisateur introuvable."); 
        }

        await _userRepository.DeleteAsync(user);
        
        return user;
    }
    
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        // On demande simplement au repository d'aller chercher le user en base de données
        return await _userRepository.GetByIdAsync(userId);
    }
    
    public async Task<User> UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
        
        return user;
    }
}