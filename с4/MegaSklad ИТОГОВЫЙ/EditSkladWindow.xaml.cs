using MegaSklad.Models;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace MegaSklad
{
    public partial class EditSkladWindow : Window
    {
        private Склады _sklad;
        private bool _isNew;

        public EditSkladWindow(Склады sklad)
        {
            InitializeComponent();
            _sklad = sklad;
            _isNew = false;
            DataContext = _sklad;
        }

        public EditSkladWindow(Склады sklad, bool isNew)
        {
            InitializeComponent();
            _sklad = sklad;
            _isNew = isNew;
            DataContext = _sklad;

            if (_isNew)
            {
                Title = "Добавить новый склад";
            }
        }

        private async Task<bool> SaveSklad()
        {
            try
            {
                if (_isNew)
                {
                    var insertResponse = await App.SupabaseClient
                        .From<Models.Склады>()
                        .Insert(_sklad);

                    if (insertResponse != null && insertResponse.ResponseMessage != null && !insertResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при создании склада: {insertResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    var updateResponse = await App.SupabaseClient
                        .From<Models.Склады>()
                        .Where(x => x.id == _sklad.id)
                        .Set(p => p.название_склада, _sklad.название_склада)
                        .Set(p => p.адрес_склада, _sklad.адрес_склада)
                        .Set(p => p.тип_склада, _sklad.тип_склада)
                        .Update();

                    if (updateResponse != null && updateResponse.ResponseMessage != null && !updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при обновлении склада: {updateResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Пожалуйста, введите название склада.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(АдресTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите адрес склада.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ТипTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите тип склада.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isSuccess = await SaveSklad();
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