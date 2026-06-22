using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Interfaces.Services;
using SP.Presentation.Dtos;
using SP.Presentation.Mappers;

namespace SP.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SpotsController : ControllerBase
{
    private readonly ISpotService _spotService;
    
    public SpotsController(ISpotService spotService)
    {
        _spotService = spotService;
    }

    // ========================================================
    //  TOUTES LES ROUTES FIXES (TEXTUELS) TOUT EN HAUT
    // ========================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var spots = await _spotService.GetAllSpotsAsync();
        var dtos = spots.Select(SpotMapper.ToDto);
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var spot = await _spotService.GetSpotByIdAsync(id);
    
        if (spot == null)
        {
            return NotFound(new { message = "Ce spot n'existe pas ou a été supprimé." });
        }

        var dto = SpotMapper.ToDto(spot);

        return Ok(dto);
    }

    [AllowAnonymous]
    [HttpGet("tags")] 
    public async Task<IActionResult> GetTags()
    {
        var tags = await _spotService.GetTagsByTypeAsync("spot");
    
        var dtos = tags.Select(t => new TagDto 
        { 
            Id = t.TagsId, 
            Name = t.Name 
        }).ToList();
    
        return Ok(dtos);
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
    
    [HttpGet("favorites")]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Tu n'es pas connecté.");
        }

        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return BadRequest("Format de l'ID utilisateur invalide.");
        }

        var favoriteSpots = await _spotService.GetFavoriteSpotsByUserIdAsync(userId);
        var dtos = favoriteSpots.Select(SpotMapper.ToDto);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSpotDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
    
        var userId = Guid.Parse(userIdClaim.Value);
        var spot = SpotMapper.ToEntity(dto, userId);
        var result = await _spotService.CreateSpotAsync(spot, dto.TagIds);
    
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var spotDto = SpotMapper.ToDto(result.Spot);
        return CreatedAtAction(nameof(GetAll), new { id = spotDto.Id }, spotDto);
    }

    // ========================================================
    // TOUTES LES ROUTES DYNAMIQUE AVEC {id} EN DESSOUS
    // ========================================================

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);

        var spot = await _spotService.GetSpotByIdAsync(id);
        if (spot == null) return NotFound("Spot introuvable.");

        if (spot.UserId != userId) 
        {
            return Forbid();
        }

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

        await _spotService.UpdateSpotAsync(existingSpot);
        return NoContent();
    }

    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Tu n'es pas connecté.");
        if (!Guid.TryParse(userIdClaim, out Guid userId)) return BadRequest("Format ID invalide.");

        var spot = await _spotService.GetSpotByIdAsync(id);
        if (spot == null) return NotFound("Spot introuvable.");

        var (isFavorite, message) = await _spotService.ToggleFavoriteSpotAsync(userId, id);
        return Ok(new { isFavorite, message });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> AdminDeleteSpot(Guid id)
    {
        var spot = await _spotService.GetSpotByIdAsync(id);
        if (spot == null) return NotFound();
    
        var deleted = await _spotService.DeleteSpotAsync(id, spot.UserId);
    
        if (!deleted) return BadRequest();
        return NoContent(); 
    }
    [HttpPost("{id}/vote")]
    public async Task<IActionResult> VoteSpot(Guid id, [FromQuery] bool isUpvote)
    {
        try
        {
            // 1. On appelle le service qui bosse avec l'entité du Domaine
            var updatedSpotEntity = await _spotService.VoteSpotAsync(id, isUpvote);

            // 2. 👑 CORRECTIF : Utilisation du mapper statique conforme au reste de ton code
            var spotDto = SpotMapper.ToDto(updatedSpotEntity);

            // 3. On répond au Front avec le DTO clean
            return Ok(spotDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur interne lors du vote.", error = ex.Message });
        }
    }
}