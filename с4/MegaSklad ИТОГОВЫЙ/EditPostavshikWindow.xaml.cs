using MegaSklad.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MegaSklad
{
    public partial class EditPostavshikWindow : Window
    {
        private Поставщики _postavshik;
        private bool _isNew;

        public EditPostavshikWindow(Поставщики postavshik)
        {
            InitializeComponent();
            _postavshik = postavshik;
            _isNew = false;
            DataContext = _postavshik;
        }

        public EditPostavshikWindow(Поставщики postavshik, bool isNew)
        {
            InitializeComponent();
            _postavshik = postavshik;
            _isNew = isNew;
            DataContext = _postavshik;

            if (_isNew)
            {
                Title = "Добавить нового поставщика";
            }
        }

        private async Task<bool> SavePostavshik()
        {
            try
            {
                if (_isNew)
                {
                    var insertResponse = await App.SupabaseClient
                        .From<Models.Поставщики>()
                        .Insert(_postavshik);

                    if (insertResponse != null && insertResponse.ResponseMessage != null && !insertResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при создании поставщика: {insertResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    var updateResponse = await App.SupabaseClient
                        .From<Models.Поставщики>()
                        .Where(x => x.id == _postavshik.id)
                        .Set(p => p.название_поставщика, _postavshik.название_поставщика)
                        .Set(p => p.ИНН_поставщика, _postavshik.ИНН_поставщика)
                        .Set(p => p.КПП_поставщика, _postavshik.КПП_поставщика)
                        .Set(p => p.контактное_лицо, _postavshik.контактное_лицо)
                        .Set(p => p.телефон_поставщика, _postavshik.телефон_поставщика)
                        .Set(p => p.email_поставщика, _postavshik.email_поставщика)
                        .Set(p => p.адрес_поставщика, _postavshik.адрес_поставщика)
                        .Update();

                    if (updateResponse != null && updateResponse.ResponseMessage != null && !updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при обновлении поставщика: {updateResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (string.IsNullOrWhiteSpace(НазваниеTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите название поставщика.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ИННTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите ИНН поставщика.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(КППTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите КПП поставщика.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(КонтактноеЛицоTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите контактное лицо.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidPhoneNumber(ТелефонTextBox.Text))// ADDED CHECK
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный email.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(АдресTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите адрес поставщика.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _postavshik.название_поставщика = НазваниеTextBox.Text;
            _postavshik.ИНН_поставщика = ИННTextBox.Text;
            _postavshik.КПП_поставщика = КППTextBox.Text;
            _postavshik.контактное_лицо = КонтактноеЛицоTextBox.Text;
            _postavshik.телефон_поставщика = ТелефонTextBox.Text;
            _postavshik.email_поставщика = EmailTextBox.Text;
            _postavshik.адрес_поставщика = АдресTextBox.Text;

            bool isSuccess = await SavePostavshik();
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

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            //Regex для российских номеров телефонов
            string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }
    }
}