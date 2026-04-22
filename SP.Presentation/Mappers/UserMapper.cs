using SP.Domain.Entities;
using SP.Presentation.Dtos;

namespace SP.Presentation.Mappers;

public static class UsersMapper
{
    // public static UserLoginDto ToDto(this User userEntity)
    // {
    //     return new UserLoginDto
    //     {
    //         Alias = userEntity.Alias,
    //         Password = userEntity.PasswordHash
    //     };
    // }

    public static User ToEntity(this UserRegistrationDto dto)
    {
        return new User
        {
            Email = dto.Email,
            PasswordHash = dto.Password,
            Username = dto.Username
        };
    }
}