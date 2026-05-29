namespace SP.Core.Interfaces.Services;

using SP.Domain.Entities;

public interface IUserService
{
    Task<User> DeleteUserAsync(Guid userId);
    
    Task<User?> GetByIdAsync(Guid userId);
    
    Task<User>  UpdateUserAsync(User user);
}