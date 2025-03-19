using Supabase;
using System;
using System.Threading.Tasks;

namespace MegaSklad
{
    public class SupabaseService
    {
        public Supabase.Client Supabase { get; set; }

        public SupabaseService(string supabaseUrl, string supabaseKey)
        {
            Supabase = new Supabase.Client(supabaseUrl, supabaseKey);
        }

        public async Task InitializeAsync()
        {
            try
            {
                await Supabase.InitializeAsync();
                Console.WriteLine($"Supabase Client initialized: {Supabase.Auth.CurrentUser != null}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Supabase: {ex.Message}");
                throw;
            }
        }
    }
}