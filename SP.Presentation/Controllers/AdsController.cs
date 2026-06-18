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
public class AdsController :  ControllerBase
{
    private readonly IAdService _adService;
    private readonly IUserService _userService;
    
    public AdsController(IAdService adService,  IUserService userService)
        {
            _adService = adService;
            _userService = userService;
        }
    

    [HttpGet]
    public async Task<IActionResult> GetAds()
    {
        var ads = await _adService.GetAllAdsAsync();
        var dtos = ads.Select(AdMapper.ToDto);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAd(CreateAdDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        Guid userId = Guid.Parse(userIdClaim.Value);
    
        // 1. On va chercher l'utilisateur pour connaître ses infos de contact
        var user = await _userService.GetByIdAsync(userId); 
        if (user == null) return NotFound("Utilisateur introuvable.");

        // 2. Choix du canal : WhatsApp si renseigné, sinon Email de secours
        string finalContact = !string.IsNullOrWhiteSpace(user.WhatsAppUrl) 
            ? user.WhatsAppUrl 
            : user.Email;

        // 3. On transforme le DTO en entité
        var ad = AdMapper.ToEntity(dto, userId, finalContact);
    
        // 🌟 On force le ContactLink avec la donnée fraîchement récupérée
        // ad.ContactLink = finalContact; 

        // 4. Envoi au service
        var result = await _adService.CreateAdAsync(ad);

        if (!result.success)
        {
            return BadRequest(result.errorMessage);
        }
    
        return Ok(new { message = "Annonce créée avec succès" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAd(Guid id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        Guid userId = Guid.Parse(userIdClaim.Value);

        var ad = await _adService.GetAdByIdAsync(id);
            if (ad == null) return  NotFound();

            if (ad.UserId != userId)
            {
                return Forbid();
            }
            
            var deleted =  await _adService.DeleteAdAsync(userId, id);
            
            if (!deleted) return  BadRequest();
            return NoContent();

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAd(Guid id, UpdateAdDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);

        var existingAd = await _adService.GetAdByIdAsync(id);
        if (existingAd == null) return NotFound("Annonce introuvable.");
        
        System.Diagnostics.Debug.WriteLine($"Token User: {userId}");
        System.Diagnostics.Debug.WriteLine($"Ad Owner: {existingAd.UserId}");

        if (existingAd.UserId != userId)
        {
            return Forbid();
        }

        if (existingAd.UserId != userId)
        {
            return Forbid();
        }

        // On applique les modifications du DTO sur l'entité existante
        existingAd.Title = dto.Title;
        existingAd.Description = dto.Description;
        existingAd.Price = dto.Price;
        existingAd.LocationArea = dto.LocationArea;
        existingAd.ContactLink = dto.ContactLink;
        existingAd.IsActive = dto.IsActive;
        
        await _adService.UpdateAdAsync(existingAd);

        return NoContent();
        
    }
    
    [HttpGet("my-ads")]
    public async Task<IActionResult> GetMyAds()
    {
        // 1. Récupération sécurisée du userId via le token JWT
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        Guid userId = Guid.Parse(userIdClaim.Value);

        // 2. Appel de ton service en lui passant le bon userId
        // Note : Assure-toi que cette méthode existe dans ton IAdService !
        var myAds = await _adService.GetAdsByUserIdAsync(userId); 
    
        // 3. Mapping propre vers les DTOs
        var dtos = myAds.Select(AdMapper.ToDto);
    
        return Ok(dtos);
    }
    
    
    // GESTION DES FAVORIS  
    
    [HttpGet("favorites")]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Tu n'es pas connecté.");
        if (!Guid.TryParse(userIdClaim, out Guid userId)) return BadRequest("Format ID invalide.");

        var favoriteAds = await _adService.GetFavoriteAdsByUserIdAsync(userId);
        
        // Utilise ton AdMapper pour renvoyer des DTOs propres au front
        var dtos = favoriteAds.Select(AdMapper.ToDto); 
        return Ok(dtos);
    }

    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Tu n'es pas connecté.");
        if (!Guid.TryParse(userIdClaim, out Guid userId)) return BadRequest("Format ID invalide.");

        var ad = await _adService.GetAdByIdAsync(id); // Vérifie si l'annonce existe
        if (ad == null) return NotFound("Annonce introuvable.");

        var (isFavorite, message) = await _adService.ToggleFavoriteAdAsync(userId, id);

        return Ok(new { isFavorite, message });
    }
}
    
