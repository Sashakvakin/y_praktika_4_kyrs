using Supabase;
using System;
using System.Threading.Tasks;
using System.Windows;
using MegaSklad.Models;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async Task RegisterUser(string email, string username, string phone, string password, string role)
        {
            try
            {
                var authResponse = await App.SupabaseClient.Auth.SignUp(email, password);

                if (authResponse.User != null)
                {
                    // Get user's Supabase Auth ID
                    string supabaseAuthId = authResponse.User.Id; // Get supabase_auth_id
                                                                  // Создаем нового пользователя в таблице "Пользователи"
                    var newUser = new Models.Пользователи
                    {
                        supabase_auth_id = supabaseAuthId,  // Save auth ID
                        имя_пользователя = username,
                        email = email,
                        телефон = phone,
                        роль = role
                    };

                    var insertResponse = await App.SupabaseClient
                        .From<Models.Пользователи>()
                        .Insert(newUser);

                    if (insertResponse != null && insertResponse.ResponseMessage != null && insertResponse.ResponseMessage.IsSuccessStatusCode == false)
                    {
                        MessageBox.Show($"Ошибка при создании пользователя: {insertResponse.ResponseMessage.ReasonPhrase}");
                        await App.SupabaseClient.Auth.SignOut();
                        return;
                    }

                    MessageBox.Show("Регистрация прошла успешно! Пожалуйста, подтвердите свой email и обратитесь к администратору за получение прав к системе");
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Ошибка при регистрации");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string username = UsernameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string password = PasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;
            string role = "неизвестно"; // Получаем выбранную роль

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            await RegisterUser(email, username, phone, password, role);
        }

        private void ToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}