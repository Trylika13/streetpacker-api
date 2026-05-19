using SP.Domain.Entities;

namespace SP.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddUserAsync(User user);}