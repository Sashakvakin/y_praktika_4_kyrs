using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class AddTovarToRashodnayaNakladnayaWindow : Window
    {
        private List<Товары> _tovaryData;
        public ТоварыРасходнойНакладной NewTovar { get; set; }
        private Guid _idSklada;  // ID склада
        private TextBlock kolichestvoLabel;
        private int _availableQuantity = 0;

        public AddTovarToRashodnayaNakladnayaWindow(Guid idSklada)
        {
            InitializeComponent();
            _idSklada = idSklada; // Store sklad ID
            Loaded += AddTovarToRashodnayaNakladnayaWindow_Loaded;
        }

        private async void AddTovarToRashodnayaNakladnayaWindow_Loaded(object sender, RoutedEventArgs e)
        {
            kolichestvoLabel = FindName("KolichestvoTextBlock") as TextBlock;
            await LoadTovaryData();
            TovarComboBox.SelectionChanged += TovarComboBox_SelectionChanged;
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

                    if (TovarComboBox.SelectedItem is Товары selectedTovar)
                    {
                        await UpdateKolichestvoLabel(selectedTovar.id);
                    }

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

        private async void TovarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TovarComboBox.SelectedItem is Товары selectedTovar)
            {
                await UpdateKolichestvoLabel(selectedTovar.id);
            }
        }

        private async Task UpdateKolichestvoLabel(Guid tovarId)
        {
            // Отладочная информация
            Console.WriteLine($"UpdateKolichestvoLabel: _idSklada = {_idSklada}, tovarId = {tovarId}");

            try
            {
                var ostatokResponse = await App.SupabaseClient
                    .From<Models.ОстаткиТоваров>()
                    .Get();

                if (ostatokResponse?.Models != null && ostatokResponse.Models.Any())
                {
                    Console.WriteLine($"Всего остатков загружено: {ostatokResponse.Models.Count}");

                    ОстаткиТоваров latestOstatok = ostatokResponse.Models
                        .Where(x => x.id_склада == _idSklada && x.id_товара == tovarId)
                        .OrderByDescending(x => x.дата_обновления)
                        .FirstOrDefault();

                    if (latestOstatok != null)
                    {
                        _availableQuantity = latestOstatok.количество;
                        Console.WriteLine($"Найдено: Id склада {latestOstatok.id_склада}, Id товара: {latestOstatok.id_товара}, Количество: {_availableQuantity}");
                        if (kolichestvoLabel != null)
                        {
                            kolichestvoLabel.Text = $"Количество ({_availableQuantity}):";
                        }
                    }
                    else
                    {
                        _availableQuantity = 0;
                        Console.WriteLine($"Остатки для склада {_idSklada} и товара {tovarId} не найдены.");
                        if (kolichestvoLabel != null)
                        {
                            kolichestvoLabel.Text = "Количество (0):";
                        }
                    }
                }
                else
                {
                    _availableQuantity = 0;
                    Console.WriteLine("Не удалось загрузить остатки.");
                    if (kolichestvoLabel != null)
                    {
                        kolichestvoLabel.Text = "Количество (0):";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке остатков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Ошибка при загрузке остатков: {ex}");

                _availableQuantity = 0;
                if (kolichestvoLabel != null)
                {
                    kolichestvoLabel.Text = "Количество:";
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

            if (_availableQuantity == 0)
            {
                MessageBox.Show("Невозможно добавить товар, так как его нет на складе.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(KolichestvoTextBox.Text, out int kolichestvo))
            {
                MessageBox.Show("Некорректный формат данных в поле Количество.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (kolichestvo == 0)
            {
                MessageBox.Show("Невозможно добавить 0 единиц товара.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (kolichestvo > _availableQuantity)
            {
                MessageBox.Show($"Невозможно добавить {kolichestvo} единиц товара. На складе доступно только {_availableQuantity}.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (!decimal.TryParse(CenaTextBox.Text, out decimal cena))
            {
                MessageBox.Show("Некорректный формат данных в поле Цена.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TovarComboBox.SelectedItem is Товары selectedTovar)
            {
                NewTovar = new ТоварыРасходнойНакладной
                {
                    id_товара = selectedTovar.id,
                    количество = kolichestvo,
                    цена = cena,
                    название_товара = selectedTovar.название_товара
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