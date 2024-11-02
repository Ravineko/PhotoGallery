using PhotoGallery.Models.Entities;

namespace PhotoGallery.Repositories.Interfaces;

public interface IPhotoRepository
{
    Task AddPhotoAsync(Photo photo);
    Task<List<Photo>> GetAllAsync();
}
