namespace achievoo.Models.Auth0.Organization;

public class Branding
{
    public string LogoUrl { get; set; } = string.Empty;
    public Colors Colors { get; set; }  = new();
}