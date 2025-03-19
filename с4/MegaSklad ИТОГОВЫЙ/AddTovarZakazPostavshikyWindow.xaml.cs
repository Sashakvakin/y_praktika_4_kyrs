// AddTovarZakazPostavshikyWindow.xaml.cs
using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class AddTovarZakazPostavshikyWindow : Window
    {
        private List<Товары> _tovaryData;
        public ТоварыЗаказаПоставщикуViewModel NewTovar { get; set; }

        public AddTovarZakazPostavshikyWindow()
        {
            InitializeComponent();
            Loaded += AddTovarZakazPostavshikyWindow_Loaded;
        }

        private async void AddTovarZakazPostavshikyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTovaryData();
        }

        private async Task LoadTovaryData()
        {
            try
            {
                var tovaryResponse = await App.SupabaseClient
                    .From<Models.Товары>()
                    .Select("*")
                    .Get();

                if (tovaryResponse.Models != null)
                {
                    _tovaryData = tovaryResponse.Models.ToList();
                    TovarComboBox.ItemsSource = _tovaryData;
                    TovarComboBox.DisplayMemberPath = "название_товара";
                    TovarComboBox.SelectedValuePath = "id";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о товарах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о товарах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о товарах: {ex}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            if (!int.TryParse(KolichestvoTextBox.Text, out int kolichestvo))
            {
                MessageBox.Show("Некорректный формат данных в поле Количество.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (TovarComboBox.SelectedItem is Товары selectedTovar)
            {
                NewTovar = new ТоварыЗаказаПоставщикуViewModel
                {
                    id_товара = selectedTovar.id, // Get ID
                    название_товара = selectedTovar.название_товара,
                    количество = kolichestvo,
                };

                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Некорректный формат данных в поле Товар.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}