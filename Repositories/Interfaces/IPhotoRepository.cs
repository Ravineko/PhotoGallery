using PhotoGallery.Models.RepositoryDTOs;

namespace PhotoGallery.Repositories.Interfaces;

public interface IPhotoRepository
{
    Task AddPhotoAsync(Photo photo);
}
