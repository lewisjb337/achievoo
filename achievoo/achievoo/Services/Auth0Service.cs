using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using achievoo.Models.Auth0.Organization;
using achievoo.Requests.Auth0;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace achievoo.Services;

public class Auth0Service(HttpClient httpClient, IConfiguration configuration, AuthenticationStateProvider authenticationStateProvider) : IAuth0Service
{
    private readonly string _domain = configuration["Auth0:Domain"]!;
    private readonly string _clientId = configuration["Auth0:ClientId"]!;
    private readonly string _clientSecret = configuration["Auth0:ClientSecret"]!;
    private string? _accessToken;

    private async Task AuthenticateAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken))
        {
            return;
        }

        var response = await httpClient.PostAsync($"https://{_domain}/oauth/token",
            new StringContent(JsonSerializer.Serialize(new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                audience = $"https://{_domain}/api/v2/",
                grant_type = "client_credentials"
            }), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
        
        var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        _accessToken = jsonResponse.GetProperty("access_token").GetString();
    }
    
    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        await AuthenticateAsync();
        
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        
        return httpClient;
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        var userEmail = await GetCurrentUserEmailAsync();

        if (string.IsNullOrEmpty(userEmail))
        {
            return string.Empty;
        }

        var client = await GetAuthenticatedClientAsync();
        var searchUrl = $"https://{_domain}/api/v2/users-by-email?email={Uri.EscapeDataString(userEmail)}";
        
        var searchResponse = await client.GetAsync(searchUrl);
        
        if (!searchResponse.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        var jsonResponse = await searchResponse.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

        return users.GetArrayLength() == 0 ? string.Empty :
            users[0].GetProperty("user_id").GetString()!;
    }

    private async Task<string?> GetCurrentUserEmailAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        return user.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
    }

    public async Task<bool> DeleteUserByEmailAsync(string email)
    {
        var client = await GetAuthenticatedClientAsync();

        var searchUrl = $"https://{_domain}/api/v2/users-by-email?email={Uri.EscapeDataString(email)}";
        var searchResponse = await client.GetAsync(searchUrl);
    
        if (!searchResponse.IsSuccessStatusCode)
        {
            return false;
        }

        var jsonResponse = await searchResponse.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

        if (users.GetArrayLength() == 0)
        {
            return false;
        }

        var userId = users[0].GetProperty("user_id").GetString();
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        var deleteUrl = $"https://{_domain}/api/v2/users/{userId}";
        var deleteResponse = await client.DeleteAsync(deleteUrl);

        return deleteResponse.IsSuccessStatusCode;
    }
    
    public async Task<List<Auth0Organization>> GetUserOrganizationsAsync()
    {
        var userEmail = await GetCurrentUserEmailAsync();

        if (string.IsNullOrEmpty(userEmail))
        {
            return [];
        }

        var client = await GetAuthenticatedClientAsync();

        var userId = await GetCurrentUserIdAsync();
        var organizationsUrl = $"https://{_domain}/api/v2/users/{userId}/organizations";

        var organizationsResponse = await client.GetAsync(organizationsUrl);

        if (!organizationsResponse.IsSuccessStatusCode)
        {
            return [];
        }

        var organizationsJson = await organizationsResponse.Content.ReadAsStringAsync();
        var organizations = JsonSerializer.Deserialize<JsonElement>(organizationsJson);

        var result = new List<Auth0Organization>();

        foreach (var org in organizations.EnumerateArray())
        {
            var orgId = org.GetProperty("id").GetString();
            var orgName = org.GetProperty("name").GetString();
            var orgDisplayName = org.GetProperty("display_name").GetString();
            var orgBranding = org.GetProperty("branding");

            var branding = new Branding
            {
                LogoUrl = orgBranding.GetProperty("logo_url").GetString()!,
                Colors = new Colors
                {
                    Primary = orgBranding.GetProperty("colors").GetProperty("primary").GetString()!,
                    PageBackground = orgBranding.GetProperty("colors").GetProperty("page_background").GetString()!
                }
            };

            result.Add(new Auth0Organization
            {
                Id = orgId!,
                Name = orgName!,
                DisplayName = orgDisplayName!,
                Branding = branding
            });
        }

        return result;
    }
    
    public async Task<string?> GetCurrentUserOrganizationIdAsync()
    {
        var userEmail = await GetCurrentUserEmailAsync();
        
        if (string.IsNullOrEmpty(userEmail))
        {
            return null;
        }

        var client = await GetAuthenticatedClientAsync();
        var searchUrl = $"https://{configuration["Auth0:Domain"]}/api/v2/users-by-email?email={Uri.EscapeDataString(userEmail)}";
        var searchResponse = await client.GetAsync(searchUrl);
    
        if (!searchResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var jsonResponse = await searchResponse.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
    
        if (users.GetArrayLength() == 0)
        {
            return null;
        }

        var userId = users[0].GetProperty("user_id").GetString();
    
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var organizationsUrl = $"https://{configuration["Auth0:Domain"]}/api/v2/users/{userId}/organizations";
        var organizationsResponse = await client.GetAsync(organizationsUrl);

        if (!organizationsResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var organizationsJson = await organizationsResponse.Content.ReadAsStringAsync();
        var organizations = JsonSerializer.Deserialize<JsonElement>(organizationsJson);

        return organizations.EnumerateArray().Any() ? organizations[0].GetProperty("id").GetString() : null;
    }
    
    private async Task<bool> CheckIfUserHasOrganizationAsync()
    {
        var userEmail = await GetCurrentUserEmailAsync();

        if (string.IsNullOrEmpty(userEmail))
        {
            return false;
        }

        var organizationId = await GetCurrentUserOrganizationIdAsync();

        return !string.IsNullOrEmpty(organizationId);
    }

    private async Task<bool> IsOrganizationNameUniqueAsync(string name)
    {
        var client = await GetAuthenticatedClientAsync();
        var url = $"https://{_domain}/api/v2/organizations?name={Uri.EscapeDataString(name)}";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var organizations = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

        return organizations.GetArrayLength() == 0;
    }

    public async Task<bool> CreateOrganizationAsync(Auth0CreateOrganizationRequest request)
    {
        if (await CheckIfUserHasOrganizationAsync())
        {
            return false;
        }

        if (!await IsOrganizationNameUniqueAsync(request.Name))
        {
            return false; 
        }

        var client = await GetAuthenticatedClientAsync();

        var organizationData = new
        {
            name = request.Name,
            display_name = request.DisplayName,
            branding = new
            {
                logo_url = request.LogoUrl,
                colors = new
                {
                    primary = request.PrimaryColor,
                    page_background = request.BackgroundColor
                }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(organizationData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var requestUrl = $"https://{_domain}/api/v2/organizations";

        var response = await client.PostAsync(requestUrl, content);

        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> InviteUserToOrganizationAsync(string organizationId, string userEmail)
    {
        var requestUrl = $"https://{_domain}/api/v2/organizations/{organizationId}/invitations";

        var invitationData = new
        {
            inviter = new
            {
                name = await GetCurrentUserEmailAsync()
            },
            invitee = new
            {
                email = userEmail
            },
            client_id = _clientId,
            send_invitation_email = true
        };
        
        var jsonRequest = JsonSerializer.Serialize(invitationData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsync(requestUrl, content);

        return response.IsSuccessStatusCode;
    }
}