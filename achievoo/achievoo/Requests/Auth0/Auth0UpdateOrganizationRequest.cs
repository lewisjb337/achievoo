namespace achievoo.Requests.Auth0;

public class Auth0UpdateOrganizationRequest
{
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = string.Empty;
}