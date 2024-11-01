namespace PhotoGallery.Models.RepositoryDTOs;

public sealed class Photo
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public ICollection<Album> Albums { get; set; }
}
