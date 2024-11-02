using AutoMapper;
using Microsoft.Extensions.Options;
using PhotoGallery.Configurations;
using PhotoGallery.Models.Entities;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Services;

public sealed class PhotoService : IPhotoService
{
    private readonly ImageSettings _settings;
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public PhotoService(
        IOptions<ImageSettings> options,
        IPhotoRepository photoRepository,
        IMapper mapper)
    {
        _settings = options.Value;
        _photoRepository = photoRepository;
        _mapper = mapper;
    }
    public Task DislikePhotoAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<PhotoDTO>> GetAllPhotos()
    {
        var photos = await _photoRepository.GetAllAsync();
        return _mapper.Map<List<PhotoDTO>>(photos);
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
            UserId = userId,
            FilePath = $"/images/{file.FileName}"
        };

        await _photoRepository.AddPhotoAsync(photo);

        return photo.FilePath;
    }
}
