using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using achievoo.Models.Auth0;
using achievoo.Requests.Auth0;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace achievoo.Services;

public class Auth0Service(AuthenticationStateProvider authenticationStateProvider, HttpClient httpClient, IConfiguration configuration) : IAuth0Service
{
    private readonly string _domain = configuration["Auth0:Domain"]!;
    private readonly string _clientId = configuration["Auth0:ClientId"]!;
    private readonly string _clientSecret = configuration["Auth0:ClientSecret"]!;
    private readonly string _managementApiToken = configuration["Auth0:ManagementApiToken"]!;
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
    
    public async Task<string?> GetOnboardingGuidAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        return user.FindFirst("onboarding_guid")?.Value;
    }

    private async Task<Auth0CompanyDetails?> GetCompanyDetailsAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is not { IsAuthenticated: true })
        {
            return null;
        }

        return new Auth0CompanyDetails
        {
            CompanyName = user.FindFirst("company_name")?.Value,
            CompanyIndustry = user.FindFirst("company_industry")?.Value,
            CompanySize = user.FindFirst("company_size")?.Value
        };
    }

    public async Task<string> CreateUserAsync(Auth0CreateUserRequest request)
    {
        var requestUrl = $"https://{_domain}/api/v2/users";

        var company = await GetCompanyDetailsAsync();
        
        var userData = new
        {
            email = request.Email,
            password = "thisisatemporarypassword#4414",
            connection = "Username-Password-Authentication",
            user_metadata = new
            {
                first_name = request.FirstName,
                last_name = request.LastName,
                job_title = request.JobTitle,
                company_name = company!.CompanyName,
                company_industry = company.CompanyIndustry,
                company_size = company.CompanySize,
                roles = new[] { request.Role }
            },
            app_metadata = new
            {
                subscription = new { }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(userData);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _managementApiToken);

        var response = await httpClient.PostAsync(requestUrl, content);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
        
        return user.GetProperty("user_id").GetString() ?? string.Empty;
    }

    public async Task DeleteUserAsync(string userId)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"https://{_domain}/api/v2/users/{userId}");

        response.EnsureSuccessStatusCode();
    }
}