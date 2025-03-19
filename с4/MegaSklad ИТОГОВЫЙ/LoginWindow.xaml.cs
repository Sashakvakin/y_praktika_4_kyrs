using Supabase;
using System;
using System.Threading.Tasks;
using System.Windows;
using MegaSklad.Models;
using System.Linq;

namespace MegaSklad
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            //EmailTextBox.Text = "admin@gmail.com"; // Тестовые данные
            //PasswordTextBox.Password = "111111"; // Тестовые данные
        }

        private async Task LoginUser(string email, string password)
        {
            try
            {
                Console.WriteLine($"Попытка входа пользователя с email: {email}");

                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Supabase client не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var authResponse = await App.SupabaseClient.Auth.SignIn(email, password);

                if (authResponse.User != null)
                {
                    string authEmail = authResponse.User.Email; // Get the email of authenticated user
                    string userId = authResponse.User.Id; // Get the id of authenticated user
                    Console.WriteLine($"Пользователь успешно аутентифицирован в Supabase Auth. Email пользователя: {authEmail}, ID пользователя: {userId}"); // Add This

                    var userResponse = await App.SupabaseClient
                        .From<Models.Пользователи>()
                        .Select("*")
                        .Where(x => x.email == authEmail) // Search user via EMAIL
                        .Get();

                    if (userResponse.Models != null && userResponse.Models.Any())
                    {
                        var user = userResponse.Models.FirstOrDefault(); // returns the first element
                        string role = user.роль; // Gets the user role
                        string id = user.id; // Gets the user id

                        Console.WriteLine($"Пользователь найден в таблице Пользователи. Роль: {role}, ID: {id}");

                        OpenMainWindow(role);
                        this.Close(); // Closes the login window
                    }
                    else
                    {
                        Console.WriteLine("Пользователь не найден в таблице Пользователи.");
                        MessageBox.Show("Пользователь не найден в таблице Пользователи.");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка при входе в Supabase Auth.");
                    MessageBox.Show($"Ошибка при входе");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;

            await LoginUser(email, password);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close(); // Закрываем окно логина
        }

        private void OpenMainWindow(string role)
        {
            Window mainWindow = null;

            switch (role)
            {
                case "администратор":
                    mainWindow = new AdminMainWindow();
                    break;
                case "бухгалтер":
                    mainWindow = new BuhgalterMainWindow();
                    break;
                case "менеджер по продажам":
                    mainWindow = new ManagerMainWindow();
                    break;
                case "кладовщик":
                    mainWindow = new KladovshikMainWindow();
                    break;
                default:
                    MessageBox.Show($"У вас нет прав. Пожалуйста, обратитесь к администратору.");
                    return;
            }

            if (mainWindow != null)
            {
                mainWindow.Show();
            }
        }
    }
}