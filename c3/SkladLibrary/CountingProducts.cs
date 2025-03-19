using System;
using System.Collections.Generic;
using System.Linq;
using Supabase;
using Postgrest.Models;
using System.Configuration;
using Postgrest.Attributes;
using System.Text;

namespace SkladLibrary
{
    [Table("Товары")]
    public class Товар : BaseModel
    {
        public Guid id_товара { get; set; }
        public string название_товара { get; set; }
        public Guid id_категории { get; set; }
        public decimal цена { get; set; }
        public int количество { get; set; } // Количество на складе
        public Guid id_склада { get; set; } // ID склада
    }

    [Table("Категории")]
    public class Категория : BaseModel
    {
        public Guid id_категории { get; set; }
        public string название_категории { get; set; }
    }

    [Table("Склады")]
    public class Склад : BaseModel
    {
        public Guid id_склада { get; set; }
        public string название_склада { get; set; }
    }

    public class CountingProducts
    {
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private List<Склад> _склады; // Кэшируем список складов
        private List<Категория> _категории; // Кэшируем список категорий

        public CountingProducts()
        {
            // Получаем URL и ключ из App.config
            _supabaseUrl = ConfigurationManager.AppSettings["SupabaseUrl"];
            _supabaseKey = ConfigurationManager.AppSettings["SupabaseAnonKey"];

            // Проверяем, что значения не пустые
            if (string.IsNullOrEmpty(_supabaseUrl) || string.IsNullOrEmpty(_supabaseKey))
            {
                throw new Exception("Supabase URL и ключ не настроены в App.config!");
            }
        }

        private Supabase.Client GetSupabaseClient()
        {
            Supabase.Client client = new Supabase.Client(_supabaseUrl, _supabaseKey);
            client.InitializeAsync().Wait(); // Синхронно ждем инициализации
            return client;
        }

        private List<Товар> GetТовары()
        {
            // Create a new supabase client
            Supabase.Client client = GetSupabaseClient();

            var response = client
                .From<Товар>()
                .Get().Result; // Синхронно ждем результат

            if (response.ResponseMessage.IsSuccessStatusCode && response.Models != null)
            {
                List<Товар> товары = response.Models;
                return товары;
            }
            else
            {
                Console.WriteLine($"Ошибка при загрузке товаров: {response.ResponseMessage.ReasonPhrase}");
                return new List<Товар>(); // Возвращаем пустой список
            }
        }

        private List<Склад> GetСклады()
        {
            if (_склады == null)
            {
                Supabase.Client client = GetSupabaseClient();
                var response = client
                    .From<Склад>()
                    .Get().Result;

                if (response.ResponseMessage.IsSuccessStatusCode && response.Models != null)
                {
                    _склады = response.Models;
                }
                else
                {
                    Console.WriteLine($"Ошибка при загрузке складов: {response.ResponseMessage.ReasonPhrase}");
                    _склады = new List<Склад>();
                }
            }
            return _склады;
        }

        private List<Категория> GetКатегории()
        {
            if (_категории == null)
            {
                Supabase.Client client = GetSupabaseClient();
                var response = client
                    .From<Категория>()
                    .Get().Result;

                if (response.ResponseMessage.IsSuccessStatusCode && response.Models != null)
                {
                    _категории = response.Models;
                }
                else
                {
                    Console.WriteLine($"Ошибка при загрузке категорий: {response.ResponseMessage.ReasonPhrase}");
                    _категории = new List<Категория>();
                }
            }
            return _категории;
        }


        private string GetWarehouseName(Guid warehouseId)
        {
            List<Склад> склады = GetСклады();
            var warehouse = склады.FirstOrDefault(s => s.id_склада == warehouseId);
            return warehouse?.название_склада ?? "Неизвестный склад";
        }

        private string GetCategoryName(Guid categoryId)
        {
            List<Категория> категории = GetКатегории();
            var category = категории.FirstOrDefault(c => c.id_категории == categoryId);
            return category?.название_категории ?? "Неизвестная категория";
        }


        // 1. Подсчет количества товаров по складам (синхронный) - с перегрузкой

        // Общее количество товаров по складам
        public Dictionary<string, int> CountProductsByWarehouse()
        {
            List<Товар> товары = GetТовары();

            return товары.GroupBy(t => t.id_склада)
                .ToDictionary(g => GetWarehouseName(g.Key), g => g.Sum(t => t.количество));
        }

        // Общее количество товаров на определенном складе (сумма позиций)
        public int CountProductsByWarehouse(Guid складId)
        {
            List<Товар> товары = GetТовары();
            return товары.Where(t => t.id_склада == складId).Sum(t => t.количество);
        }

        // 2. Подсчет суммы стоимости товаров - с перегрузкой

        // Сумма стоимости товаров по складам
        public Dictionary<string, decimal> CalculateTotalValueByWarehouse()
        {
            List<Товар> товары = GetТовары();

            return товары.GroupBy(t => t.id_склада)
                .ToDictionary(g => GetWarehouseName(g.Key), g => g.Sum(t => t.цена * t.количество));
        }

        // Сумма стоимости товаров на определенном складе
        public decimal CalculateTotalValueByWarehouse(Guid складId)
        {
            List<Товар> товары = GetТовары();
            return товары.Where(t => t.id_склада == складId).Sum(t => t.цена * t.количество);
        }


        // 3. Подсчет товара по категориям - с перегрузкой

        // Подсчет товара по категориям по складам
        public Dictionary<string, Dictionary<string, int>> CountProductsByCategoryByWarehouse()
        {
            List<Товар> товары = GetТовары();

            return товары.GroupBy(t => t.id_склада)
                .ToDictionary(
                    g => GetWarehouseName(g.Key),
                    g => g.GroupBy(t => t.id_категории)
                          .ToDictionary(g2 => GetCategoryName(g2.Key), g2 => g2.Sum(t => t.количество))
                );
        }

        // Подсчет товара по категориям на определенном складе
        public Dictionary<string, int> CountProductsByCategoryByWarehouse(Guid складId)
        {
            List<Товар> товары = GetТовары();

            return товары.Where(t => t.id_склада == складId)
                .GroupBy(t => t.id_категории)
                .ToDictionary(g => GetCategoryName(g.Key), g => g.Sum(t => t.количество));
        }
        public string GetAnalytics()
        {
            StringBuilder sb = new StringBuilder();

            // 1. Подсчет количества товаров
            sb.AppendLine("--- Подсчет количества товаров ---");
            // 1.1 По складам
            Dictionary<string, int> productCountsByWarehouse = CountProductsByWarehouse();
            sb.AppendLine("По складам:");
            foreach (var warehouseCount in productCountsByWarehouse)
            {
                sb.AppendLine($"Склад {warehouseCount.Key}: {warehouseCount.Value} товаров");
            }

            // 1.2 На складе
            //Guid firstWarehouseId = productCountsByWarehouse.Keys.FirstOrDefault(); // Получаем ID первого склада для примера
            Guid firstWarehouseId = GetСклады().FirstOrDefault()?.id_склада ?? Guid.Empty;
            if (firstWarehouseId != Guid.Empty)
            {
                int productCountOnWarehouse = CountProductsByWarehouse(firstWarehouseId);
                sb.AppendLine($"На складе {GetWarehouseName(firstWarehouseId)}: {productCountOnWarehouse} товаров");
            }
            else
            {
                sb.AppendLine("Нет данных о складах.");
            }

            // 2. Подсчет суммы стоимости товаров
            sb.AppendLine("\n--- Подсчет суммы стоимости товаров ---");
            // 2.1 По складам
            Dictionary<string, decimal> totalValueByWarehouse = CalculateTotalValueByWarehouse();
            sb.AppendLine("По складам:");
            foreach (var warehouseValue in totalValueByWarehouse)
            {
                sb.AppendLine($"Склад {warehouseValue.Key}: {warehouseValue.Value} стоимость");
            }

            // 2.2 На складе
            if (firstWarehouseId != Guid.Empty)
            {
                decimal totalValueOnWarehouse = CalculateTotalValueByWarehouse(firstWarehouseId);
                sb.AppendLine($"На складе {GetWarehouseName(firstWarehouseId)}: {totalValueOnWarehouse} стоимость");
            }
            else
            {
                sb.AppendLine("Нет данных о складах.");
            }


            // 3. Подсчет товара по категориям
            sb.AppendLine("\n--- Подсчет товара по категориям ---");
            // 3.1 По складам
            Dictionary<string, Dictionary<string, int>> productCountsByCategoryByWarehouse = CountProductsByCategoryByWarehouse();
            sb.AppendLine("По складам:");
            foreach (var warehouseCategoryCounts in productCountsByCategoryByWarehouse)
            {
                sb.AppendLine($"Склад {warehouseCategoryCounts.Key}:");
                foreach (var categoryCount in warehouseCategoryCounts.Value)
                {
                    sb.AppendLine($"  Категория {categoryCount.Key}: {categoryCount.Value} товаров");
                }
            }

            // 3.2 На складе
            if (firstWarehouseId != Guid.Empty)
            {
                Dictionary<string, int> productCountsByCategoryOnWarehouse = CountProductsByCategoryByWarehouse(firstWarehouseId);
                sb.AppendLine($"На складе {GetWarehouseName(firstWarehouseId)}:");
                foreach (var categoryCount in productCountsByCategoryOnWarehouse)
                {
                    sb.AppendLine($"  Категория {categoryCount.Key}: {categoryCount.Value} товаров");
                }
            }
            else
            {
                sb.AppendLine("Нет данных о складах.");
            }

            return sb.ToString();
        }
    }
}