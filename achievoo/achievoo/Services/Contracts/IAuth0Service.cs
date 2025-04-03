using achievoo.Models.Auth0.Organization;
using achievoo.Requests.Auth0;

namespace achievoo.Services.Contracts;

public interface IAuth0Service
{
    Task<bool> DeleteUserByEmailAsync(string email);

    Task<bool> CheckIfUserHasOrganizationAsync();
    
    Task<List<Auth0Organization>> GetUserOrganizationsAsync();
    
    Task<string?> GetCurrentUserOrganizationIdAsync();

    Task<bool> CreateOrganizationAsync(Auth0CreateOrganizationRequest request);
    
    Task<bool> InviteUserToOrganizationAsync(string organizationId, string userEmail);
}