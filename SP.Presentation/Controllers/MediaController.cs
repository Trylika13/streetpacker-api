using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Interfaces.Services;

namespace SP.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier envoyé.");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("Le fichier doit être une image.");

        try
        {
            using var stream = file.OpenReadStream();
            var imageUrl = await _mediaService.UploadImageAsync(stream, file.FileName, file.ContentType);

            return Ok(new { url = imageUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur lors de l'upload : {ex.Message}");
        }
    }
}
