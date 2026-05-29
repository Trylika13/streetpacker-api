using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Interfaces.Services;
using SP.Presentation.Dtos;

namespace SP.Presentation.Controllers;

[Authorize] // Sécurité max pour tout le fichier
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        
        // ⚠️ Assure-toi d'avoir cette méthode GetByIdAsync ou GetUserByIdAsync dans ton IUserService / UserRepository
        var user = await _userService.GetByIdAsync(userId); 
        
        if (user == null)
            return NotFound();

        return Ok(new 
        { 
            username = user.Username, 
            email = user.Email 
        });
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Utilisateur non identifié." });

            var userId = Guid.Parse(userIdClaim); 

            await _userService.DeleteUserAsync(userId);

            return NoContent(); 
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
    {
        // 1. Récupération de l'ID via le token
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);

        // 2. On chope le user en base
        var existingUser = await _userService.GetByIdAsync(userId);
        if (existingUser == null) return NotFound("Utilisateur introuvable.");

        // 3. On applique les modifs
        existingUser.Username = dto.Username;
        existingUser.Email = dto.Email;

        // 4. On sauvegarde
        await _userService.UpdateUserAsync(existingUser);

        return Ok(new { message = "Profil mis à jour avec succès." });
    }
    
}