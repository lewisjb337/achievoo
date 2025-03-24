using achievoo.Services.Contracts;
using achievoo.Services.Contracts.Supabase;
using Supabase;

namespace achievoo.Services.Supabase;

public class SupabaseEmployeeService(SupabaseService supabaseService) : ISupabaseEmployeeService
{
    private Client? _supabase = supabaseService.SupabaseClient;
}