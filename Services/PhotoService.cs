using Microsoft.Extensions.Options;
using PhotoGallery.Configurations;
using PhotoGallery.Models.RepositoryDTOs;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Services;

public sealed class PhotoService : IPhotoService
{
    private readonly ImageSettings _settings;
    private readonly IPhotoRepository _photoRepository;

    public PhotoService(
        IOptions<ImageSettings> options,
        IPhotoRepository photoRepository)
    {
        _settings = options.Value;
        _photoRepository = photoRepository;
    }
    public Task DislikePhotoAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task LikePhotoAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UploadPhotoAsync(IFormFile file, int userId)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(), _settings.ImageStoragePath);
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var photo = new Photo
        {
            /*            Title = file.FileName,
                        AlbumId = albumId,*/
            UserId = userId,
            FilePath = $"/images/{file.FileName}"
        };

        await _photoRepository.AddPhotoAsync(photo);

        return photo.FilePath;
    }
}
