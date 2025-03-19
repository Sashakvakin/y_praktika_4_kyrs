using System;
using Xamarin.Forms;
using Supabase;
using System.Threading.Tasks;
using Xamarin.Essentials;
using MSM;

namespace MSM
{
    public partial class App : Application
    {
        public static Supabase.Client SupabaseClient { get; private set; }
        private static string SupabaseUrl = "https://nwqsnwvqgggyrbizxaqm.supabase.co";
        private static string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im53cXNud3ZxZ2dneXJiaXp4YXFtIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc0MDQ4NzM2NSwiZXhwIjoyMDU2MDYzMzY1fQ.m4vWzBmfWEKF6mabMhScrfJsQ-4g0wgE1kMOIxqKpMQ";

        public App()
        {
            InitializeComponent();
            InitializeSupabase();
            MainPage = new NavigationPage(new LoginPage()); // Изначально показываем LoginPage

        }

        private async Task InitializeSupabase()
        {
            try
            {
                SupabaseClient = new Supabase.Client(SupabaseUrl, SupabaseKey, new SupabaseOptions
                {
                    AutoRefreshToken = true,
                });

                await SupabaseClient.InitializeAsync();

                // Проверяем, есть ли сохраненный сеанс
                var session = await SupabaseClient.Auth.RetrieveSessionAsync();

                if (session != null)
                {
                    // Если есть сеанс, автоматически переходим на MainPage
                    string userEmail = await SecureStorage.GetAsync("UserEmail"); // Получаем email из SecureStorage
                    string userRole = await SecureStorage.GetAsync("UserRole");

                    if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(userRole))
                    {
                        MainPage = new NavigationPage(new MainPage(userRole)); // Передаем роль пользователя
                    }
                    else
                    {
                        // Если нет email или роли, переходим на LoginPage
                        MainPage = new NavigationPage(new LoginPage());
                    }
                }
                else
                {
                    // Если нет сеанса, переходим на LoginPage
                    MainPage = new NavigationPage(new LoginPage());
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle the initialization error
                Console.WriteLine($"Supabase initialization error: {ex.Message}");
                await MainPage.DisplayAlert("Error", $"Failed to initialize Supabase: {ex.Message}", "OK");
                MainPage = new NavigationPage(new LoginPage()); // Если произошла ошибка, показываем LoginPage
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}