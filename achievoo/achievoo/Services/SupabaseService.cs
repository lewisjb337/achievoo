using Supabase;

namespace achievoo.Services;

public class SupabaseService
{
    public Client SupabaseClient { get; }

    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:Key"];

        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        SupabaseClient = new Client(url, key, new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        });
    }

    public async Task InitializeAsync()
    {
        await SupabaseClient.InitializeAsync();
    }
}