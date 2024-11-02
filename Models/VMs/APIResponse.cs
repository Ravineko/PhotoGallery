namespace PhotoGallery.Models.VMs;

public class APIResponse<T>
{
    public APIResponse()
    {
        ErrorMessages = new List<string>();
    }

    public bool IsSuccess { get; set; } = true;

    public List<string> ErrorMessages { get; set; }
    public T Result { get; set; }

}
