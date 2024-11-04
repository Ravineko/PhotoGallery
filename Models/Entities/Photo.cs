namespace PhotoGallery.Models.Entities;

public sealed class Photo
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Album> Albums { get; set; } = new List<Album>();
}
