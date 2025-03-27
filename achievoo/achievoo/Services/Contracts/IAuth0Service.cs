using achievoo.Models.Auth0;
using achievoo.Requests.Auth0;

namespace achievoo.Services.Contracts;

public interface IAuth0Service
{
    Task<string?> GetOnboardingGuidAsync();

    Task<string> CreateUserAsync(Auth0CreateUserRequest request);

    Task DeleteUserAsync(string userId);
}