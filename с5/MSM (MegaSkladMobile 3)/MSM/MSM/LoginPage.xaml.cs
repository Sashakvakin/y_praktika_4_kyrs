using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using MSM.Models;
using System.Linq;

namespace MSM
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            CheckRememberMe();
        }

        private async Task CheckRememberMe()
        {
            try
            {
                string email = await SecureStorage.GetAsync("RememberedEmail");

                if (!string.IsNullOrEmpty(email))
                {
                    // Автоматическая авторизация
                    bool loginSuccess = await AuthenticateUser(email);
                    if (loginSuccess)
                    {
                        return; // Прерываем дальнейшую загрузку страницы логина
                    }
                    else
                    {
                        // Если автоматическая авторизация не удалась, отображаем ошибку или позволяем пользователю войти вручную
                        await DisplayAlert("Ошибка", "Не удалось автоматически войти. Пожалуйста, введите данные вручную.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок SecureStorage
                Console.WriteLine($"Ошибка при проверке RememberMe: {ex.Message}");
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            //string password = PasswordEntry.Text; //Больше не используем, но Entry остается

            bool loginSuccess = await AuthenticateUser(email);
            if (!loginSuccess) return;

            // Запоминаем пользователя, если чекбокс отмечен
            if (RememberMeCheckBox.IsChecked)
            {
                try
                {
                    await SecureStorage.SetAsync("RememberedEmail", email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении RememberMe: {ex.Message}");
                }
            }
            else
            {
                // Удаляем учетные данные, если чекбокс не отмечен
                try
                {
                    SecureStorage.Remove("RememberedEmail");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении RememberMe: {ex.Message}");
                }
            }
        }

        private async Task<bool> AuthenticateUser(string email)
        {
            try
            {
                // Ваша логика аутентификации (замените на свой код)
                var authResponse = await App.SupabaseClient
                    .From<Models.Пользователи>()
                    .Select("*")
                    .Where(x => x.email == email)
                    .Get();

                if (authResponse?.Models != null && authResponse.Models.Any())
                {
                    var user = authResponse.Models.FirstOrDefault();

                    // Сохраняем email пользователя в SecureStorage
                    await SecureStorage.SetAsync("UserEmail", email);
                    await SecureStorage.SetAsync("UserRole", user.роль);

                    // Переход на MainPage
                    App.Current.MainPage = new NavigationPage(new MainPage(user.роль));
                    return true;
                }
                else
                {
                    await DisplayAlert("Ошибка", "Неверный email", "OK");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка авторизации: {ex.Message}", "OK");
                return false;
            }
        }
    }
}