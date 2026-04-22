using SP.Core.Interfaces.Services;
using SP.Presentation.Dtos;
using SP.Presentation.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace SP.Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    #region Register

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto dto)
    {
        try
        {
            // Utilisation du mapper pour transformer le DTO en Entité 
            var userEntity = dto.ToEntity();

            //  On passe l'entité et le mot de passe en clair au service
            var result = await _userService.RegisterAsync(userEntity, dto.Password);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    

    #endregion

    
}