namespace SP.Core.Interfaces.Services;

using SP.Domain.Entities;

public interface IUserService
{
    Task<User> RegisterAsync(User user, string clearPassword);
}