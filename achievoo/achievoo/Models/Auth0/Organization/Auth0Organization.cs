namespace achievoo.Models.Auth0.Organization;

public class Auth0Organization
{
    public string Id { get; set; }  = string.Empty;
    public string Name { get; set; }  = string.Empty;
    public string DisplayName { get; set; }  = string.Empty;
    public Branding Branding { get; set; }   = new();
}