using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Linq;
using System;
using MSM;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace MSM
{
    public partial class MainPage : Xamarin.Forms.TabbedPage // Явно указываем Xamarin.Forms.TabbedPage
    {
        public MainPage(string role)
        {
            InitializeComponent();
            Title = $"MegaSklad ({role})";

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom); // Перемещаем панель навигации вниз

            Children.Clear();

            // Создаем стиль для NavigationPage
            Style navigationPageStyle = new Style(typeof(NavigationPage));
            navigationPageStyle.Setters.Add(new Setter { Property = NavigationPage.BarTextColorProperty, Value = Color.Black });
            navigationPageStyle.Setters.Add(new Setter { Property = NavigationPage.BarBackgroundColorProperty, Value = Color.White });
            navigationPageStyle.Setters.Add(new Setter { Property = NavigationPage.TitleViewProperty, Value = CreateTitleView() });

            // Создаем NavigationPage для каждой вкладки и применяем стиль
            Children.Add(new NavigationPage(new ProfilePage()) { Title = "Профиль", IconImageSource = "profile.png", Style = navigationPageStyle });
            Children.Add(new NavigationPage(new WarehousesPage()) { Title = "Склады", IconImageSource = "warehouse.png", Style = navigationPageStyle });
            Children.Add(new NavigationPage(new ProductsPage()) { Title = "Товары", IconImageSource = "products.png", Style = navigationPageStyle });
            Children.Add(new NavigationPage(new ScanPage()) { Title = "Сканировать", IconImageSource = "scan.png", Style = navigationPageStyle });

            RemoveScanTabIfNeeded();
        }

        // Функция для создания TitleView
        private View CreateTitleView()
        {
            return new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), // Выберите нужный размер шрифта
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
            };
        }

        private async Task RemoveScanTabIfNeeded()
        {
            try
            {
                string userEmail = await SecureStorage.GetAsync("UserEmail");

                if (userEmail != "kladovshik@gmail.com")
                {
                    var scanPage = Children.FirstOrDefault(page => page is NavigationPage && page.Title == "Сканировать");

                    if (scanPage != null)
                    {
                        Children.Remove(scanPage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении вкладки Scan: {ex.Message}");
            }
        }
    }
}