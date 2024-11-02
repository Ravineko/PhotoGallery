namespace PhotoGallery.Models.ServiceDTOs;

public sealed class PhotoDTO
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public int UserId { get; set; }
}
