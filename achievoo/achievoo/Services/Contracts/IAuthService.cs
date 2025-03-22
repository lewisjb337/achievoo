using Microsoft.AspNetCore.Components.Authorization;

namespace achievoo.Services.Contracts;

public interface IAuthService
{
    Task<AuthenticationState> GetAuthenticationStateAsync();

    Task<bool> LoginAsync(string email, string password);

    Task<bool> RegisterAsync(string email, string password);

    Task LogoutAsync();
}