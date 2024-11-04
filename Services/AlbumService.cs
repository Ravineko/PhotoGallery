using AutoMapper;
using PhotoGallery.Models.Entities;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Services;

public sealed class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly IMapper _mapper;

    public AlbumService(IAlbumRepository albumRepository, IPhotoRepository photoRepository, IMapper mapper)
    {
        _albumRepository = albumRepository;
        _photoRepository = photoRepository;
        _mapper = mapper;
    }

    public async Task<int> CreateAlbumAsync(AlbumDto albumDto)
    {
        var albumRepoModel = _mapper.Map<Album>(albumDto);
        return await _albumRepository.CreateAlbumAsync(albumRepoModel);
    }

    public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync()
    {
        var albumsRepoModels = await _albumRepository.GetAllAlbumsAsync();
        return _mapper.Map<IEnumerable<AlbumDto>>(albumsRepoModels);
    }

    public async Task<AlbumDto> GetAlbumByIdAsync(int id)
    {
        var albumRepoModel = await _albumRepository.GetAlbumByIdAsync(id);
        return albumRepoModel != null ? _mapper.Map<AlbumDto>(albumRepoModel) : null;
    }

    public async Task<IEnumerable<PhotoDTO>> GetPhotosByAlbumIdAsync(int albumId)
    {
        var photosRepoModels = await _albumRepository.GetPhotosByAlbumIdAsync(albumId);
        return _mapper.Map<IEnumerable<PhotoDTO>>(photosRepoModels);
    }

    public async Task AddPhotoToAlbumAsync(int albumId, int photoId)
    {
        var photo = await _photoRepository.GetPhotoByIdAsync(photoId);
        var album = await _albumRepository.GetAlbumByIdAsync(albumId);

        if (photo != null && album != null)
        {
            await _albumRepository.AddPhotoToAlbumAsync(album.Id, photo.Id);
            var updatedAlbum = await _albumRepository.GetAlbumByIdAsync(album.Id);
            if (updatedAlbum.Photos.Contains(photo))
            {
                Console.WriteLine("Photo successfully added to album.");
            }
            else
            {
                Console.WriteLine("Photo was not added to album.");
            }

            await _albumRepository.SetCoverPathAsync(
                album.Id, photo.FilePath);

            await _albumRepository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Album>> GetAlbumsByUserAsync(int userId)
    {
        return await _albumRepository.GetAlbumsByUserAsync(userId);
    }
}
