using PhotoGallery.Models.Entities;

namespace PhotoGallery.Repositories.Interfaces;

public interface IAlbumRepository
{
    Task<int> CreateAlbumAsync(Album albumDto);
    Task<IEnumerable<Album>> GetAllAlbumsAsync();
    Task<Album> GetAlbumByIdAsync(int id);
    Task<IEnumerable<Photo>> GetPhotosByAlbumIdAsync(int albumId);
    Task SetCoverPathAsync(int albumId, string coverPath);
    Task AddPhotoToAlbumAsync(int albumId, int photoId);
    Task<IEnumerable<Album>> GetAlbumsByUserAsync(int userId);
    Task SaveChangesAsync();
}
