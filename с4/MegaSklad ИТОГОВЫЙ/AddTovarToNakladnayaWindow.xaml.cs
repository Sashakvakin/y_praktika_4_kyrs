using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class AddTovarToNakladnayaWindow : Window
    {
        private List<Товары> _tovaryData;
        public ТоварыПриходнойНакладной NewTovar { get; set; }
        private TextBlock kolichestvoLabel;

        public AddTovarToNakladnayaWindow()
        {
            InitializeComponent();
            Loaded += AddTovarToNakladnayaWindow_Loaded;
        }

        private async void AddTovarToNakladnayaWindow_Loaded(object sender, RoutedEventArgs e)
        {
            kolichestvoLabel = FindName("KolichestvoTextBlock") as TextBlock;
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

                    // Подписываемся на событие SelectionChanged
                    TovarComboBox.SelectionChanged += TovarComboBox_SelectionChanged;
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

        private void TovarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TovarComboBox.SelectedItem is Товары selectedTovar)
            {
                string edinicaIzmereniya = selectedTovar.единица_измерения;
                UpdateKolichestvoLabel(edinicaIzmereniya);
            }
            else
            {
                UpdateKolichestvoLabel(null);
            }
        }

        private void UpdateKolichestvoLabel(string edinicaIzmereniya)
        {
            if (kolichestvoLabel != null)
            {
                if (string.IsNullOrEmpty(edinicaIzmereniya))
                {
                    kolichestvoLabel.Text = "Количество:";
                }
                else
                {
                    kolichestvoLabel.Text = $"Количество ({edinicaIzmereniya}):";
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (TovarComboBox.SelectedItem == null || string.IsNullOrEmpty(KolichestvoTextBox.Text) || string.IsNullOrEmpty(CenaTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(KolichestvoTextBox.Text, out int kolichestvo))
            {
                MessageBox.Show("Некорректный формат данных в поле Количество.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(CenaTextBox.Text, out decimal cena))
            {
                MessageBox.Show("Некорректный формат данных в поле Цена.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Guid.TryParse(TovarComboBox.SelectedValue.ToString(), out Guid idTovara))
            {
                MessageBox.Show("Некорректный формат данных в поле Товар.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nazvanieTovara = ((Товары)TovarComboBox.SelectedItem).название_товара;

            NewTovar = new ТоварыПриходнойНакладной
            {
                id_товара = idTovara,
                количество = kolichestvo,
                цена = cena,
                название_товара = nazvanieTovara
            };

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}