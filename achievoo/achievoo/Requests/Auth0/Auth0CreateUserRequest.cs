namespace achievoo.Requests.Auth0;

public class Auth0CreateUserRequest
{
    public string Email { get; set; } =  string.Empty;
    public string Password { get; set; } =  string.Empty;
    public string FirstName { get; set; } =  string.Empty;
    public string LastName { get; set; } =  string.Empty;
    public string JobTitle { get; set; } =  string.Empty;
    public string Role { get; set; } =  string.Empty;
}