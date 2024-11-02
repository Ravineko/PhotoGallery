namespace PhotoGallery.Configurations;

public class EmailSettings
{
    public string ApiKey { get; set; }

    public string SendGridPromotionTemplateId { get; set; }

    public string FromEmailAddress { get; set; }

    public string FromEmailName { get; set; }

    public string Domain { get; set; }
}
