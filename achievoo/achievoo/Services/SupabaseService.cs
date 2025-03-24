using Supabase;

namespace achievoo.Services;

public class SupabaseService
{
    public Client? SupabaseClient { get; }
    
    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:Key"];

        if (url == null || key == null)
        {
            return;
        }
        
        SupabaseClient = new Client(url, key, new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        });

        SupabaseClient.InitializeAsync().Wait();
    }
}