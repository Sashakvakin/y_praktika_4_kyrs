using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MegaSklad.Models;
using System.Threading.Tasks;

namespace MegaSklad
{
    public class TovarIdToNameConverter : IValueConverter
    {
        private static Dictionary<Guid, string> _tovarNames = new Dictionary<Guid, string>();

        static TovarIdToNameConverter()
        {
            LoadTovarNamesAsync();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid tovarId)
            {
                if (_tovarNames.ContainsKey(tovarId))
                {
                    return _tovarNames[tovarId];
                }
                else
                {
                    Console.WriteLine($"Название товара с ID {tovarId} не найдено в кэше.");
                    return DependencyProperty.UnsetValue; // Trigger re-evaluation
                }

            }
            Console.WriteLine($"Неверный тип значения: {value?.GetType()}");
            return "Неизвестный товар";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static async Task LoadTovarNamesAsync()
        {
            try
            {
                var tovarResponse = await App.SupabaseClient
                    .From<Товары>()
                    .Select("id, название_товара")
                    .Get();

                if (tovarResponse.Models != null)
                {
                    _tovarNames = tovarResponse.Models.ToDictionary(t => t.id, t => t.название_товара);

                    //Вывод содержимого для проверки
                    foreach (var tovar in _tovarNames)
                    {
                        Console.WriteLine($"Товар ID: {tovar.Key}, Название: {tovar.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("Не удалось загрузить данные о товарах для конвертера.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных о товарах для конвертера: {ex}");
            }
        }
    }
}