using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Interfaces.Services;
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
        // Récupération de l'ID utilisateur depuis le Token
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);

        // Récupération du spot AVANT la suppression pour vérification
        var spot = await _spotService.GetSpotByIdAsync(id);
    
        if (spot == null) return NotFound("Spot introuvable.");

        // Vérification de sécurité : Est-ce le bon user_id ?
        if (spot.UserId != userId) 
        {
            return Forbid();
        }

        // Si c'est le bon, on supprime
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
    
        var existingSpot = await _spotService.GetSpotByIdAsync(id);
        if (existingSpot == null) return NotFound("Spot introuvable.");

        if (existingSpot.UserId != userId)
        {
            return Forbid();
        }

        existingSpot.Title = dto.Title;
        existingSpot.Description = dto.Description;
        existingSpot.ImageUrl = dto.ImageUrl;
        existingSpot.LastVerifiedAt = DateTime.UtcNow;

        // Sauvegarde
        await _spotService.UpdateSpotAsync(existingSpot);

        return NoContent();
    }

    [HttpGet("my-spots")]
    public async Task<IActionResult> GetMySpots()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Tu n'es pas connecté");
        }

        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return BadRequest("Format de l'ID utilisateur invalide dans le token.");
        }
        
        var spots = await _spotService.GetSpotsByUserIdAsync(userId);
        var dtos = spots.Select(SpotMapper.ToDto);
        return Ok(dtos);
    }
    
    // GESTION DES FAVORIS
    
    [HttpGet("favorites")]
    public async Task<IActionResult> GetMyFavorites()
    {
        // 1. Récupération et validation de l'ID utilisateur depuis le Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Tu n'es pas connecté.");
        }

        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return BadRequest("Format de l'ID utilisateur invalide.");
        }

        // 2. Récupération des entités via le service
        var favoriteSpots = await _spotService.GetFavoriteSpotsByUserIdAsync(userId);
        
        // 3. Mapping en DTO (On réutilise ton SpotMapper existant)
        var dtos = favoriteSpots.Select(SpotMapper.ToDto);
        
        return Ok(dtos);
    }

    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Tu n'es pas connecté.");
        if (!Guid.TryParse(userIdClaim, out Guid userId)) return BadRequest("Format ID invalide.");

        var spot = await _spotService.GetSpotByIdAsync(id);
        if (spot == null) return NotFound("Spot introuvable.");

        // Récupération directe du Tuple renvoyé par le service
        var (isFavorite, message) = await _spotService.ToggleFavoriteSpotAsync(userId, id);

        return Ok(new { isFavorite, message });
    }
}