using Microsoft.AspNetCore.Authorization;
using SP.Core.Interfaces.Services;
using SP.Presentation.Dtos;
using SP.Presentation.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace SP.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    #region Register

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto dto)
    {
        try
        {
            // Utilisation du mapper pour transformer le DTO en Entité 
            var userEntity = dto.ToEntity();

            //  On passe l'entité et le mot de passe en clair au service
            var result = await _authService.RegisterAsync(userEntity, dto.Password);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    #endregion

    #region Login

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto.Username, dto.Password);

        if (result == null)
        {
            return Unauthorized(new { message = "Identifiants invalides." });
        }

        var response = new AuthResponseDto
        {
            AccessToken = result.Value.Token,
            RefreshToken = result.Value.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
        return Ok(response);
    }

    #endregion

    #region Refresh

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

        if (result == null)
            return Unauthorized("Session expirée");

        return Ok(new AuthResponseDto
        {
            AccessToken = result.Value.Token,
            RefreshToken = result.Value.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }
}

#endregion