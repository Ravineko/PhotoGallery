namespace PhotoGallery.Models.ServiceDTOs;

public sealed class AlbumDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CoverPath { get; set; }
    public int UserId { get; set; }
}
