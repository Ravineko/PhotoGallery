namespace PhotoGallery.Models.RepositoryDTOs;

public sealed class Album
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CoverPath { get; set; }
    public ICollection<Photo> Photos { get; set; }
}
