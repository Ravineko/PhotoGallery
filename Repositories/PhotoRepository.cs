using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.Entities;
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

    public async Task<List<Photo>> GetAllAsync()
    {
        var photos = await _context.Photos.ToListAsync();
        return photos;
    }
}
