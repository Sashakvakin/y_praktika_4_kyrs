using System;
using System.Configuration;
using System.Windows;
using Supabase;

namespace MegaSklad
{
    public partial class App : Application
    {
        public static Supabase.Client SupabaseClient { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string supabaseUrl = ConfigurationManager.AppSettings["SupabaseUrl"];
            string supabaseKey = ConfigurationManager.AppSettings["SupabaseAnonKey"];

            if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
            {
                MessageBox.Show("Ошибка: Не удалось найти Supabase URL и ключ в App.config");
                Shutdown();
                return;
            }

            // options больше не нужны
            SupabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);

            try
            {
                await SupabaseClient.InitializeAsync();

                if (SupabaseClient.Auth.CurrentUser != null)
                {
                    // Если пользователь уже аутентифицирован, открываем MainWindow
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    // Если пользователь не аутентифицирован, открываем LoginWindow
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации Supabase: {ex.Message}");
                Shutdown();
                return;
            }
        }
    }
}