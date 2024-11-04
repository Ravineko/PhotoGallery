namespace PhotoGallery.Models.Entities;

public sealed class Album
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CoverPath { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
