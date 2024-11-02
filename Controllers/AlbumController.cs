using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Controllers;
[ApiController]
[Route("api/albums")]
public class AlbumController : ControllerBase
{
    private readonly IAlbumService _albumService;

    public AlbumController(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlbum([FromBody] AlbumDto albumDto)
    {
        var albumId = await _albumService.CreateAlbumAsync(albumDto);
        return CreatedAtAction(nameof(GetAlbumById), new { id = albumId }, albumId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAlbums()
    {
        var albums = await _albumService.GetAllAlbumsAsync();
        return Ok(albums);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlbumById(int id)
    {
        var album = await _albumService.GetAlbumByIdAsync(id);
        return album != null ? Ok(album) : NotFound();
    }

    [HttpGet("{albumId}/photos")]
    public async Task<IActionResult> GetPhotosByAlbumId(int albumId)
    {
        var photos = await _albumService.GetPhotosByAlbumIdAsync(albumId);
        return Ok(photos);
    }

    [HttpPost("{albumId}/photos/{photoId}")]
    public async Task<IActionResult> AddPhotoToAlbum(int albumId, int photoId)
    {
        await _albumService.AddPhotoToAlbumAsync(albumId, photoId);
        return NoContent();
    }
}

