using SP.Domain.Entities;

namespace SP.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddUserAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task DeleteAsync(User user);
    Task UpdateAsync(User user);
}