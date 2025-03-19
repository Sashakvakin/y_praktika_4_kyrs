using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MegaSklad
{
    public partial class RashodnayaNakladnayaDetailsWindow : Window
    {
        public RashodnayaNakladnayaDetailsWindow(Guid nakladnayaId)
        {
            InitializeComponent();
            Loaded += RashodnayaNakladnayaDetailsWindow_Loaded;
            _nakladnayaId = nakladnayaId;
        }

        public Guid _nakladnayaId;

        private async void RashodnayaNakladnayaDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTovaryData();
        }

        private async Task LoadTovaryData()
        {
            try
            {
                var tovaryResponse = await App.SupabaseClient
                    .From<Models.ТоварыРасходнойНакладной2>()
                    .Get(); // Get all data

                if (tovaryResponse.Models != null)
                {
                    // Filter data in C#
                    List<ТоварыРасходнойНакладной3> tovaryRashodList = new List<ТоварыРасходнойНакладной3>();

                    foreach (var item in tovaryResponse.Models.Where(x => x.id_расходной_накладной == _nakladnayaId))
                    {
                        tovaryRashodList.Add(new ТоварыРасходнойНакладной3()
                        {
                            название_товара = await GetTovarName(item.id_товара),
                            количество = item.количество,
                            цена = item.цена
                        });
                    }
                    TovaryDataGrid.ItemsSource = tovaryRashodList;
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о товарах в накладной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    TovaryDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о товарах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о товарах: {ex}");
            }
        }
        private async Task<string> GetTovarName(Guid idTovara)
        {
            try
            {
                var tovarResponse = await App.SupabaseClient
                    .From<Models.Товары>()
                    .Where(x => x.id == idTovara)
                    .Select("название_товара")
                    .Get();

                if (tovarResponse.Models != null && tovarResponse.Models.Any())
                {
                    return tovarResponse.Models.First().название_товара;
                }
                else
                {
                    Console.WriteLine($"Товар с ID {idTovara} не найден.");
                    return "Неизвестный товар";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке названия товара: {ex}");
                return "Неизвестный товар";
            }
        }
    }
}