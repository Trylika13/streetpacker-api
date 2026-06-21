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
public class AdsController : ControllerBase
{
    private readonly IAdService _adService;
    private readonly IUserService _userService;
    
    public AdsController(IAdService adService, IUserService userService)
    {
        _adService = adService;
        _userService = userService;
    }

    // ========================================================
    //  TOUTES LES ROUTES FIXES (TEXTUELLES) TOUT EN HAUT
    // ========================================================

    [HttpGet]
    public async Task<IActionResult> GetAds()
    {
        var ads = await _adService.GetAllAdsAsync();
        var dtos = ads.Select(AdMapper.ToDto);
        return Ok(dtos);
    }

    [AllowAnonymous]
    [HttpGet("tags")] 
    public async Task<IActionResult> GetMarketplaceTags()
    {
        var tags = await _adService.GetAdTagsAsync();
    
        var dtos = tags.Select(t => new TagDto 
        { 
            Id = t.TagsId, 
            Name = t.Name 
        }).ToList();
    
        return Ok(dtos);
    }
    
    [HttpGet("my-ads")]
    public async Task<IActionResult> GetMyAds()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        Guid userId = Guid.Parse(userIdClaim.Value);

        var myAds = await _adService.GetAdsByUserIdAsync(userId); 
        var dtos = myAds.Select(AdMapper.ToDto);
    
        return Ok(dtos);
    }
    
    [HttpGet("favorites")]
    public async Task<IActionResult> GetMyFavorites()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Tu n'es pas connecté.");
        if (!Guid.TryParse(userIdClaim, out Guid userId)) return BadRequest("Format ID invalide.");

        var favoriteAds = await _adService.GetFavoriteAdsByUserIdAsync(userId);
        var dtos = favoriteAds.Select(AdMapper.ToDto); 
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAd(CreateAdDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        Guid userId = Guid.Parse(userIdClaim.Value);

        var user = await _userService.GetByIdAsync(userId); 
        if (user == null) return NotFound("Utilisateur introuvable.");

        string finalContact = !string.IsNullOrWhiteSpace(user.WhatsAppUrl) 
            ? user.WhatsAppUrl 
            : user.Email;

        var ad = AdMapper.ToEntity(dto, userId, finalContact);

        var result = await _adService.CreateAdAsync(ad, dto.TagIds);

        if (!result.success)
        {
            return BadRequest(result.errorMessage);
        }

        var adDto = AdMapper.ToDto(result.ad);
        return CreatedAtAction(nameof(GetAds), new { id = adDto.AdId }, adDto);
    }
    
    // ========================================================
    // TOUTES LES ROUTES DYNAMIQUES AVEC {id} EN DESSOUS
    // ========================================================
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAd(Guid id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        Guid userId = Guid.Parse(userIdClaim.Value);

        var ad = await _adService.GetAdByIdAsync(id);
        if (ad == null) return NotFound();

        if (ad.UserId != userId)
        {
            return Forbid();
        }
            
        var deleted = await _adService.DeleteAdAsync(userId, id);
            
        if (!deleted) return BadRequest();
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

        existingAd.Title = dto.Title;
        existingAd.Description = dto.Description;
        existingAd.Price = dto.Price;
        existingAd.LocationArea = dto.LocationArea;
        existingAd.ContactLink = dto.ContactLink;
        existingAd.IsActive = dto.IsActive;
        
        await _adService.UpdateAdAsync(existingAd);

        return NoContent();
    }

    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Tu n'es pas connecté.");
        if (!Guid.TryParse(userIdClaim, out Guid userId)) return BadRequest("Format ID invalide.");

        var ad = await _adService.GetAdByIdAsync(id);
        if (ad == null) return NotFound("Annonce introuvable.");

        var (isFavorite, message) = await _adService.ToggleFavoriteAdAsync(userId, id);

        return Ok(new { isFavorite, message });
    }
}