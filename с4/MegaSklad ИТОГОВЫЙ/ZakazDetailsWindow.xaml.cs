using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class ZakazDetailsWindow : Window
    {
        private Guid _zakazId;
        private string _tipZakaza;

        public ZakazDetailsWindow(Guid zakazId, string tipZakaza)
        {
            InitializeComponent();
            _zakazId = zakazId;
            _tipZakaza = tipZakaza;
            Loaded += ZakazDetailsWindow_Loaded;
        }

        private async void ZakazDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTovaryData();
        }

        private async Task LoadTovaryData()
        {
            try
            {
                List<object> tovaryList = new List<object>();

                if (_tipZakaza == "ЗаказыПоставщикам")
                {
                    var tovaryResponse = await App.SupabaseClient
                        .From<Models.ТоварыЗаказаПоставщику>()
                        .Get(); // Получаем все данные

                    if (tovaryResponse.Models != null)
                    {
                        // Фильтруем в C#
                        var filteredTovary = tovaryResponse.Models
                        .Where(x => x.id_заказа_поставщику == _zakazId);

                        // Формируем tovaryList в C# коде
                        foreach (var tovar in filteredTovary)
                        {
                            Товары newTovar = await GetTovarById(tovar.id_товара);
                            if (newTovar != null)
                            {
                                tovaryList.Add(new ТоварыЗаказаПоставщикуДетали
                                {
                                    название_товара = newTovar.название_товара,
                                    количество = tovar.количество
                                });
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить данные о товарах заказа поставщикам.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (_tipZakaza == "ЗаказыКлиентам")
                {
                    var tovaryResponse = await App.SupabaseClient
                        .From<Models.ТоварыЗаказаКлиенту>()
                        .Get(); // Получаем все данные

                    if (tovaryResponse.Models != null)
                    {
                        // Фильтруем в C#
                        var filteredTovary = tovaryResponse.Models
                        .Where(x => x.id_заказа_клиенту == _zakazId);

                        // Формируем tovaryList в C# коде
                        foreach (var tovar in filteredTovary)
                        {
                            Товары newTovar = await GetTovarById(tovar.id_товара);
                            if (newTovar != null)
                            {
                                tovaryList.Add(new ТоварыЗаказаКлиентуДетали
                                {
                                    название_товара = newTovar.название_товара,
                                    количество = tovar.количество
                                });
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить данные о товарах заказа клиентам.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                TovaryDataGrid.ItemsSource = tovaryList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о товарах заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о товарах заказа: {ex}");
            }
        }
        private async Task<Товары> GetTovarById(Guid idTovara)
        {
            try
            {
                var tovarResponse = await App.SupabaseClient
                    .From<Models.Товары>()
                    .Where(x => x.id == idTovara)
                    .Get();

                if (tovarResponse.Models != null && tovarResponse.Models.Any())
                {
                    return tovarResponse.Models.FirstOrDefault();
                }
                else
                {
                    Console.WriteLine($"Товар с ID {idTovara} не найден.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных о товаре: {ex}");
                return null;
            }
        }

    }
    public class ТоварыЗаказаПоставщикуДетали
    {
        public string название_товара { get; set; }
        public int количество { get; set; }
    }

    public class ТоварыЗаказаКлиентуДетали
    {
        public string название_товара { get; set; }
        public int количество { get; set; }
    }
}