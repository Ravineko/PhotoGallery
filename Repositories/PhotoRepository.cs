using PhotoGallery.Data;
using PhotoGallery.Models.RepositoryDTOs;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class PhotoRepository : IPhotoRepository
{
    private readonly GalleryDbContext _context;

    public PhotoRepository(GalleryDbContext context)
    {
        _context = context;
    }
    public async Task AddPhotoAsync(Photo photo)
    {
        await _context.Photos.AddAsync(photo);
        await _context.SaveChangesAsync();
    }
}
