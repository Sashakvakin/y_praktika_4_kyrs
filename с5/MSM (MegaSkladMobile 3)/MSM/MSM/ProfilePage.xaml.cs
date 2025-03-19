using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Linq;
using MSM.Models;
using MSM;
using System.Xml;

namespace MSM
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                string userEmail = await SecureStorage.GetAsync("UserEmail");

                if (string.IsNullOrEmpty(userEmail))
                {
                    await DisplayAlert("Ошибка", $"Почта не найдена", "OK");
                    return;
                }

                var userResponse = await App.SupabaseClient
                    .From<Models.Пользователи>()
                    .Select("*")
                    .Where(x => x.email == userEmail)
                    .Get();

                if (userResponse?.Models != null && userResponse.Models.Any())
                {
                    var user = userResponse.Models.FirstOrDefault();
                    EmailLabel.Text = user.email;
                    RoleLabel.Text = user.роль;
                    RoleOnPassLabel.Text = user.роль;
                    NameLabel.Text = user.имя_пользователя;

                    FullNameLabel.Text = user.имя_пользователя; // Имя и фамилия


                    if (user.email != "kladovshik@gmail.com")
                    {
                        PassSection.IsVisible = false;
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные о пользователе", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить информацию о пользователе: {ex.Message}", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            SecureStorage.RemoveAll();
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}