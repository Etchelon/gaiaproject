using Supabase;

namespace SupabaseGenericRepository;

/// <summary>
/// Supabase context for managing database connections and operations.
/// </summary>
public class SupabaseContext
{
    private readonly Client _client;

    public SupabaseContext(Client client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public Client Client => _client;
}
