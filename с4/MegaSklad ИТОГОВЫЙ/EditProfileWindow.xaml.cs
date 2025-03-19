using MegaSklad.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace MegaSklad
{
    public partial class EditProfileWindow : Window
    {
        private Пользователи _userProfile;
        private string _selectedFilePath; // Store the path to the selected file
        public EditProfileWindow(Пользователи userProfile)
        {
            InitializeComponent();
            _userProfile = userProfile;
            DataContext = _userProfile;
              if (_userProfile.фото != null)
              {
              string bucketName = "photoprofil";
              string imageName = _userProfile.фото.ToString() + ".png";
                 string fullImageUrl = Task.Run(() => App.SupabaseClient.Storage.From(bucketName).GetPublicUrl(imageName)).Result;
                      BitmapImage bitmapImage = new BitmapImage();
                       bitmapImage.BeginInit();
                       bitmapImage.UriSource = new Uri(fullImageUrl);
                       bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                       bitmapImage.EndInit();
                      ProfileImage.Source = bitmapImage;
                   }
         }

        private async Task<bool> SaveProfile()
        {
            try
            {
                if (!string.IsNullOrEmpty(_selectedFilePath))
                {
                    //Upload
                    string filename = $"{Guid.NewGuid()}.png";

                    byte[] imageBytes = File.ReadAllBytes(_selectedFilePath);

                   var bucket = App.SupabaseClient.Storage.From("photoprofil");
                   var response = await bucket.Upload(imageBytes, filename, new Supabase.Storage.FileOptions { ContentType = "image/png", Upsert = true });

                    if (response != null)
                    {
                        string[] parts = filename.Split('/');
                        string imageName = parts.Length > 1 ? parts[1] : parts[0]; // Use filename if there is no "/"
                        _userProfile.фото = Guid.Parse(imageName.Replace(".png", ""));
                    }
                    else
                   {
                        MessageBox.Show("Ошибка при обновлении фото.");
                        return false;
                   }
                }
                var updateResponse = await App.SupabaseClient
                    .From<Models.Пользователи>()
                    .Where(x => x.id == _userProfile.id) // We compare with ID from `Пользователи`
                    .Set(x => x.имя_пользователя, _userProfile.имя_пользователя)
                    .Set(x => x.email, _userProfile.email)
                    .Set(x => x.телефон, _userProfile.телефон)
                    .Set(x => x.фото, _userProfile.фото)
                    .Update();

                if (updateResponse != null && updateResponse.ResponseMessage != null && updateResponse.ResponseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Профиль успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show($"Ошибка при обновлении профиля: {updateResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedFilePath);
                bitmap.EndInit();
                ProfileImage.Source = bitmap;
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = await SaveProfile();
            if (isSuccess)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }
    }
}