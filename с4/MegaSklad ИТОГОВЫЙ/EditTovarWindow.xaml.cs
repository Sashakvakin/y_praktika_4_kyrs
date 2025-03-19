using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MegaSklad
{
    public partial class EditTovarWindow : Window
    {
        private Товары _tovar;
        private bool _isNew;
        private List<Категории> _kategoriiData;

        public EditTovarWindow(Товары tovar, List<Категории> kategoriiData)
        {
            InitializeComponent();
            _tovar = tovar;
            _isNew = false;
            _kategoriiData = kategoriiData;
            DataContext = _tovar;
            InitializeComboBox();
        }

        public EditTovarWindow(Товары tovar, bool isNew, List<Категории> kategoriiData)
        {
            InitializeComponent();
            _tovar = tovar;
            _isNew = isNew;
            _kategoriiData = kategoriiData;
            DataContext = _tovar;
            InitializeComboBox();

            if (_isNew)
            {
                Title = "Добавить новый товар";
            }
        }

        private void InitializeComboBox()
        {
            КатегорияComboBox.ItemsSource = _kategoriiData;
            КатегорияComboBox.SelectedValuePath = "id";
            КатегорияComboBox.DisplayMemberPath = "название_категории";
            КатегорияComboBox.SelectedValue = _tovar.id_категории;
        }

        private async Task<bool> SaveTovar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(НазваниеTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, введите название товара.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(АртикулTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, введите артикул товара.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (КатегорияComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите категорию.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(ЕдиницаИзмеренияTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, введите единицу измерения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                decimal cena;
                if (!decimal.TryParse(ЦенаTextBox.Text, out cena))
                {
                    MessageBox.Show("Некорректный формат цены.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                int minimalniyOstatok;
                if (!int.TryParse(МинимальныйОстатокTextBox.Text, out minimalniyOstatok))
                {
                    MessageBox.Show("Некорректный формат минимального остатка.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                _tovar.id_категории = (Guid)КатегорияComboBox.SelectedValue;

                if (_isNew)
                {
                    var insertResponse = await App.SupabaseClient
                        .From<Models.Товары>()
                        .Insert(_tovar);

                    if (insertResponse != null && insertResponse.ResponseMessage != null && !insertResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при создании товара: {insertResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    var updateResponse = await App.SupabaseClient
                        .From<Models.Товары>()
                        .Where(x => x.id == _tovar.id)
                        .Set(p => p.название_товара, _tovar.название_товара)
                        .Set(p => p.артикул_товара, _tovar.артикул_товара)
                        .Set(p => p.штрихкод_товара, _tovar.штрихкод_товара)
                        .Set(p => p.id_категории, _tovar.id_категории)
                        .Set(p => p.единица_измерения, _tovar.единица_измерения)
                        .Set(p => p.цена, _tovar.цена)
                        .Set(p => p.минимальный_остаток, _tovar.минимальный_остаток)
                        .Set(p => p.описание, _tovar.описание)
                        .Update();

                    if (updateResponse != null && updateResponse.ResponseMessage != null && !updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Ошибка при обновлении товара: {updateResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            bool isSuccess = await SaveTovar();
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