using MegaSklad.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class EditUserWindow : Window
    {
        private Пользователи _user;
        private bool _isNew;

        public EditUserWindow(Пользователи user)
        {
            InitializeComponent();
            _user = user;
            _isNew = false;
            DataContext = _user;
            if (string.IsNullOrEmpty(_user.роль))
            {
                RoleComboBox.SelectedIndex = 0;
            }
            else
            {
                //Find index
                for (int i = 0; i < RoleComboBox.Items.Count; i++)
                {
                    ComboBoxItem item = RoleComboBox.Items[i] as ComboBoxItem;
                    if (item.Content.ToString() == _user.роль)
                    {
                        RoleComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public EditUserWindow(Пользователи user, bool isNew)
        {
            InitializeComponent();
            _user = user;
            _isNew = isNew;
            DataContext = _user;
            if (_isNew)
            {
                Title = "Добавить нового пользователя";
            }
            if (string.IsNullOrEmpty(_user.роль))
            {
                RoleComboBox.SelectedIndex = 0;
            }
            else
            {
                //Find index
                for (int i = 0; i < RoleComboBox.Items.Count; i++)
                {
                    ComboBoxItem item = RoleComboBox.Items[i] as ComboBoxItem;
                    if (item.Content.ToString() == _user.роль)
                    {
                        RoleComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private async Task<bool> SaveUser()
        {
            try
            {
                _user.роль = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();

                if (_isNew)
                {
                    var insertResponse = await App.SupabaseClient
                        .From<Models.Пользователи>()
                        .Insert(_user);

                    if (insertResponse != null && insertResponse.ResponseMessage != null && !insertResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при создании пользователя: {insertResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    var updateResponse = await App.SupabaseClient
                        .From<Models.Пользователи>()
                        .Where(x => x.id == _user.id)
                        .Set(p => p.имя_пользователя, _user.имя_пользователя)
                        .Set(p => p.email, _user.email)
                        .Set(p => p.телефон, _user.телефон)
                        .Set(p => p.роль, _user.роль)
                        .Update();

                    if (updateResponse != null && updateResponse.ResponseMessage != null && !updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при обновлении пользователя: {updateResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = await SaveUser();
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