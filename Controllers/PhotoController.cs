using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Controllers;

[Route("api/photos")]
public class PhotoController : BaseController
{
    private readonly IPhotoService _photoService;

    public PhotoController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        var photoUrl = await _photoService.UploadPhotoAsync(file, UserId.Value);
        return Ok(new { photoUrl });
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikePhoto(int id)
    {
        await _photoService.LikePhotoAsync(id);
        return Ok();
    }

    [HttpPost("{id}/dislike")]
    public async Task<IActionResult> DislikePhoto(int id)
    {
        await _photoService.DislikePhotoAsync(id);
        return Ok();
    }
}
