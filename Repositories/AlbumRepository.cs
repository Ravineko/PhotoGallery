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

    public async Task AddPhotoToAlbumAsync(int albumId, int photoId)
    {
        var album = await _context.Albums
             .Include(a => a.Photos)
             .FirstOrDefaultAsync(a => a.Id == albumId);

        if (album == null)
        {
            throw new Exception($"Album with ID {albumId} not found.");
        }

        var photo = await _context.Photos.FindAsync(photoId);
        if (photo == null)
        {
            throw new Exception($"Photo with ID {photoId} not found.");
        }

        if (!album.Photos.Any(p => p.Id == photoId))
        {
            album.Photos.Add(photo);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Photo with ID {photoId} added to album with ID {albumId}.");
        }
        else
        {
            Console.WriteLine($"Photo with ID {photoId} is already in album with ID {albumId}.");
        }
    }

    public async Task<IEnumerable<Album>> GetAlbumsByUserAsync(int userId)
    {
        return await _context.Albums
            .Where(album => album.UserId == userId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
