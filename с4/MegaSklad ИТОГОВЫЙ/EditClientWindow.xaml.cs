using MegaSklad.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MegaSklad
{
    public partial class EditClientWindow : Window
    {
        private Клиенты _klient;
        private bool _isNew;

        public EditClientWindow(Клиенты klient)
        {
            InitializeComponent();
            _klient = klient;
            _isNew = false;
            DataContext = _klient;
        }

        public EditClientWindow(Клиенты klient, bool isNew)
        {
            InitializeComponent();
            _klient = klient;
            _isNew = isNew;
            DataContext = _klient;

            if (_isNew)
            {
                Title = "Добавить нового клиента";
            }
        }

        private async Task<bool> SaveKlient()
        {
            try
            {
                if (_isNew)
                {
                    var insertResponse = await App.SupabaseClient
                        .From<Models.Клиенты>()
                        .Insert(_klient);

                    if (insertResponse != null && insertResponse.ResponseMessage != null && !insertResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при создании клиента: {insertResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    var updateResponse = await App.SupabaseClient
                        .From<Models.Клиенты>()
                        .Where(x => x.id == _klient.id)
                        .Set(p => p.название_клиента, _klient.название_клиента)
                        .Set(p => p.контактное_лицо, _klient.контактное_лицо)
                        .Set(p => p.телефон_клиента, _klient.телефон_клиента)
                        .Set(p => p.email_клиента, _klient.email_клиента)
                        .Set(p => p.адрес_клиента, _klient.адрес_клиента)
                        .Update();

                    if (updateResponse != null && updateResponse.ResponseMessage != null && !updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при обновлении клиента: {updateResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Validation checks
            if (string.IsNullOrWhiteSpace(НазваниеTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите название клиента.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(КонтактноеЛицоTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите контактное лицо.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ТелефонTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите телефон клиента.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный email.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(АдресTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите адрес клиента.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _klient.название_клиента = НазваниеTextBox.Text;
            _klient.контактное_лицо = КонтактноеЛицоTextBox.Text;
            _klient.телефон_клиента = ТелефонTextBox.Text;
            _klient.email_клиента = EmailTextBox.Text;
            _klient.адрес_клиента = АдресTextBox.Text;
            bool isSuccess = await SaveKlient();
            if (isSuccess)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}