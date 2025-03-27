using achievoo.AuthenticationStateSyncer;
using achievoo.Components;
using achievoo.Services;
using achievoo.Services.Contracts;
using achievoo.Services.Contracts.Supabase;
using achievoo.Services.Supabase;
using achievoo.Support;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Sidio.Sitemap.AspNetCore;
using Sidio.Sitemap.AspNetCore.Middleware;
using Sidio.Sitemap.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddSingleton<SupabaseService>();

builder.Services.AddHttpClient<IAuth0Service, Auth0Service>();

builder.Services.AddScoped<IAuth0Service, Auth0Service>();
builder.Services.AddScoped<ISupabaseEmployeeService, SupabaseEmployeeService>();
builder.Services.AddScoped<IValidationService, ValidationService>();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"]!;
    options.ClientId = builder.Configuration["Auth0:ClientId"]!;
});

builder.Services.ConfigureSameSiteNoneCookies();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor().AddDefaultSitemapServices<HttpContextBaseUrlProvider>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var supabaseService = scope.ServiceProvider.GetRequiredService<SupabaseService>();
    try
    {
        await supabaseService.InitializeAsync();
        Console.WriteLine("SupabaseClient initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing SupabaseClient: {ex.Message}");
    }
}

app.UseSitemap();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/Account/Login", async httpContext =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();

    await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async httpContext =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();

    await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();