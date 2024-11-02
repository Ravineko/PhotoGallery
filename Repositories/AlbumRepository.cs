using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.Entities;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class AlbumRepository : IAlbumRepository
{
    private readonly GalleryDbContext _context;

    public AlbumRepository(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAlbumAsync(Album albumRepoModel)
    {
        var album = new Album
        {
            Title = albumRepoModel.Title,
            UserId = albumRepoModel.UserId,
            CoverPath = albumRepoModel.CoverPath
        };

        _context.Albums.Add(album);
        await _context.SaveChangesAsync();
        return album.Id;
    }

    public async Task<IEnumerable<Album>> GetAllAlbumsAsync()
    {
        return await _context.Albums
            .Select(a => new Album
            {
                Id = a.Id,
                Title = a.Title,
                CoverPath = a.CoverPath,
                UserId = a.UserId
            })
            .ToListAsync();
    }

    public async Task<Album> GetAlbumByIdAsync(int id)
    {
        var album = await _context.Albums.FindAsync(id);
        if (album == null) return null;

        return new Album
        {
            Id = album.Id,
            Title = album.Title,
            CoverPath = album.CoverPath,
            UserId = album.UserId
        };
    }

    public async Task<IEnumerable<Photo>> GetPhotosByAlbumIdAsync(int albumId)
    {
        return await _context.Photos
            .Where(p => p.Albums.Any(a => a.Id == albumId))
            .Select(p => new Photo
            {
                Id = p.Id,
                FilePath = p.FilePath,
                UserId = p.UserId
            })
            .ToListAsync();
    }

    public async Task SetCoverPathAsync(int albumId, string coverPath)
    {
        var album = await _context.Albums.FindAsync(albumId);
        if (album != null)
        {
            album.CoverPath = coverPath;
            await _context.SaveChangesAsync();
        }
    }
}
