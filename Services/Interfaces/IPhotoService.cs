namespace PhotoGallery.Services.Interfaces;

public interface IPhotoService
{
    Task<string> UploadPhotoAsync(IFormFile file, int userId);
    Task LikePhotoAsync(int id);
    Task DislikePhotoAsync(int id);

}
