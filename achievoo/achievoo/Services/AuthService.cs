using System.Security.Claims;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Supabase;

namespace achievoo.Services;

public class AuthService(Client supabase, ProtectedSessionStorage sessionStorage) : AuthenticationStateProvider, IAuthService
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var accessToken = await sessionStorage.GetAsync<string>("access_token");
        var refreshToken = await sessionStorage.GetAsync<string>("refresh_token");

        if (!accessToken.Success || !refreshToken.Success)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        
        await supabase.Auth.SetSession(accessToken.Value!, refreshToken.Value!, true);
        
        var user = supabase.Auth.CurrentUser;

        return user != null ? 
            CreateAuthState(user) : 
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var session = await supabase.Auth.SignIn(email, password);

        if (session?.User == null)
        {
            return false;
        }
        
        if (session.AccessToken != null)
        {
            await sessionStorage.SetAsync("authToken", session.AccessToken);
        }
            
        NotifyAuthenticationStateChanged(Task.FromResult(CreateAuthState(session.User)));
            
        return true;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var user = await supabase.Auth.SignUp(email, password);
        
        return user?.User != null;
    }

    public async Task LogoutAsync()
    {
        await supabase.Auth.SignOut();
        await sessionStorage.DeleteAsync("authToken");
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
    
    private AuthenticationState CreateAuthState(Supabase.Gotrue.User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email!)
        };
        
        var identity = new ClaimsIdentity(claims, "SupabaseAuth");
        var principal = new ClaimsPrincipal(identity);
        
        return new AuthenticationState(principal);
    }
}