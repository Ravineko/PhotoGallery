namespace PhotoGallery.Configurations;

public class JWTSettings
{
    public string SecretKey { get; set; }

    public int RefreshExpireDays { get; set; }

    public int JWTExpireMinutes { get; set; }
}
