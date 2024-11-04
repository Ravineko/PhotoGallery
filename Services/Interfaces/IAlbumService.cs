using PhotoGallery.Models.Entities;
using PhotoGallery.Models.ServiceDTOs;

namespace PhotoGallery.Services.Interfaces;

public interface IAlbumService
{
    Task AddPhotoToAlbumAsync(int albumId, int photoId);
    Task<int> CreateAlbumAsync(AlbumDto albumDto);
    Task<AlbumDto> GetAlbumByIdAsync(int id);
    Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync();
    Task<IEnumerable<PhotoDTO>> GetPhotosByAlbumIdAsync(int albumId);
    Task<IEnumerable<Album>> GetAlbumsByUserAsync(int userId);
}
