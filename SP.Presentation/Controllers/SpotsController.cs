using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Interfaces.Services;
using SP.Domain.Entities;
using SP.Presentation.Dtos;
using SP.Presentation.Mappers;

namespace SP.Presentation.Controllers;

[Authorize]
[ApiController ]
[Route("api/[controller]")]
public class SpotsController : ControllerBase
{
    private readonly ISpotService _spotService;
    
    public SpotsController(ISpotService spotService)
    {
        _spotService = spotService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var spots = await _spotService.GetAllSpotsAsync();
        var dtos = spots.Select(SpotMapper.ToDto);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSpotDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        
        var userId = Guid.Parse(userIdClaim.Value);
        
        var spot = SpotMapper.ToEntity(dto, userId);
        
        var result = await _spotService.CreateSpotAsync(spot);
        
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return CreatedAtAction(nameof(GetAll), new { id = result.Spot?.Id }, result.Spot);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // 1. Récupération de l'ID utilisateur depuis le Token
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);

        // 2. Récupération du spot AVANT la suppression pour vérification
        var spot = await _spotService.GetSpotByIdAsync(id);
    
        if (spot == null) return NotFound("Spot introuvable.");

        // DEBUG : Vérifie bien quelle propriété contient l'ID (UserId ou User_Id ?)
        Console.WriteLine($"Token UserID: {userId}");
        Console.WriteLine($"DB Spot UserID: {spot.UserId}"); 

        // 3. Vérification de sécurité : Est-ce le bon proprio ?
        if (spot.UserId != userId) 
        {
            return StatusCode(403, "C'est pas ton spot, pas touche la mouche t'as jamais pris ta douche");
        }

        // 4. Si c'est le bon, on supprime
        var deleted = await _spotService.DeleteSpotAsync(id, userId); 
    
        if (!deleted) return BadRequest("Erreur lors de la suppression.");

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateSpotDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);
        
        var spotToUpdate = SpotMapper.ToUpdateEntity(dto, id);

        var updated = await _spotService.UpdateSpotAsync(spotToUpdate, userId);

        if (!updated)
        {
            return StatusCode(403, "C'est pas ton spot, pas touche la mouche t'as jamais pris ta douche");
        }
        return NoContent();
    }
}