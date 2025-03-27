namespace achievoo.Models.Auth0;

public class Auth0User
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
}