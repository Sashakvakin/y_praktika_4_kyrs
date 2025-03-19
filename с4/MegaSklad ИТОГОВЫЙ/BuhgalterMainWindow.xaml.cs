using MegaSklad.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static MegaSklad.KladovshikMainWindow;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MegaSklad
{
    public partial class BuhgalterMainWindow : Window
    {
        private List<Склады> _skladyData;
        private List<OtchetPoOstatkamItem> _otchetPoOstatkamData;
        private List<ПриходныеНакладные2> _prihodnieNakladnieData = new List<ПриходныеНакладные2>();
        private List<РасходныеНакладные2> _rashodnieNakladnieData = new List<РасходныеНакладные2>();

        private Пользователи _userProfile;
        public Пользователи UserProfile
        {
            get { return _userProfile; }
            set
            {
                _userProfile = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _profileImageSource;
        public BitmapImage ProfileImageSource
        {
            get { return _profileImageSource; }
            set
            {
                _profileImageSource = value;
                OnPropertyChanged();
            }
        }

        public BuhgalterMainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += BuhgalterMainWindow_Loaded;
        }

        private async void BuhgalterMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUserProfileData();
            await LoadSkladyDataOstatki();
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide();
                // Создание и открытие окна авторизации ДО выхода (это важно!)
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();


                // Выход из аккаунта Supabase
                await App.SupabaseClient.Auth.SignOut();

                // Закрытие текущего окна
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе из аккаунта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task LoadUserProfileData()
        {
            try
            {
                Console.WriteLine("LoadUserProfileData called");
                var userId = App.SupabaseClient.Auth.CurrentUser.Id;

                var userResponse = await App.SupabaseClient
                    .From<Models.Пользователи>()
                    .Where(x => x.supabase_auth_id == userId)
                    .Select("*")
                    .Get();

                if (userResponse.Models != null && userResponse.Models.Any())
                {
                    UserProfile = userResponse.Models.FirstOrDefault();
                    // Load image
                    await LoadProfileImage();
                    //Set all data
                    NameTextBlock.Text = $"Имя: {UserProfile.имя_пользователя}";
                    EmailTextBlock.Text = $"Email: {UserProfile.email}";
                    PhoneTextBlock.Text = $"Телефон: {UserProfile.телефон}";
                    RoleTextBlock.Text = $"Роль: {UserProfile.роль}";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить информацию о пользователе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ProfileImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                    //Set default data

                    NameTextBlock.Text = $"Имя: Неизвестно";
                    EmailTextBlock.Text = $"Email:  Неизвестно";
                    PhoneTextBlock.Text = $"Телефон:  Неизвестно";
                    RoleTextBlock.Text = $"Роль:  Неизвестно";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке информации о пользователе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ProfileImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                //Set default data
                NameTextBlock.Text = $"Имя: Ошибка";
                EmailTextBlock.Text = $"Email:  Ошибка";
                PhoneTextBlock.Text = $"Телефон: Ошибка";
                RoleTextBlock.Text = $"Роль: Ошибка";
            }
        }

        private async Task LoadProfileImage()
        {
            if (UserProfile.фото != null && !string.IsNullOrEmpty(UserProfile.фото.ToString()))
            {
                try
                {
                    string imageName = UserProfile.фото.ToString() + ".png";
                    string fullImageUrl = await Task.Run(() => App.SupabaseClient.Storage.From("photoprofil").GetPublicUrl(imageName));

                    if (!string.IsNullOrEmpty(fullImageUrl))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(fullImageUrl);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        ProfileImageSource = bitmapImage;

                    }
                    else
                    {
                        ProfileImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке QR-кода: {ex.Message}");
                    ProfileImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                }
            }
            else
            {
                ProfileImageSource = new BitmapImage(new Uri("pack://application:,,,/user.png"));
            }
            ProfileImage.Source = ProfileImageSource; // установить источник для ProfileImage 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow(UserProfile);
            if (editProfileWindow.ShowDialog() == true)
            {
                LoadProfileImage(); // reloads the image

            }
        }

        private async void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            string menuName = ((Button)sender).Tag.ToString();

            // Скрываем все панели
            ProfilePanel.Visibility = Visibility.Collapsed;
            OtchetyPoSkladamPanel.Visibility = Visibility.Collapsed;
            OtchetPoOborotamPanel.Visibility = Visibility.Collapsed;
            OtchetPoOstatkamPanel.Visibility = Visibility.Collapsed;
            InventarizatsiyaPanel.Visibility = Visibility.Collapsed;


            switch (menuName)
            {
                case "Профиль":
                    ProfilePanel.Visibility = Visibility.Visible;
                    await LoadUserProfileData();
                    break;
                case "ОтчетыПоСкладам":
                    OtchetyPoSkladamPanel.Visibility = Visibility.Visible;
                    await LoadSkladyData();
                    UpdateDataGrid(await GetOtchetPoSkladamData());
                    break;
                case "ОтчетПоОборотам":
                    OtchetPoOborotamPanel.Visibility = Visibility.Visible;
                    FormOtchetPoOborotamButton_Click(null, null);
                    break;
                case "ОтчетПоОстаткам":
                    OtchetPoOstatkamPanel.Visibility = Visibility.Visible;
                    break;
                case "Инвентаризация":
                    InventarizatsiyaPanel.Visibility = Visibility.Visible;
                    await LoadSkladyDataInvent();
                    break;
                default:
                    break;
            }
        }
        private async Task LoadSkladyData()
        {
            try
            {
                var skladyResponse = await App.SupabaseClient
                    .From<Models.Склады>()
                    .Select("*")
                    .Get();

                if (skladyResponse.Models != null)
                {
                    _skladyData = skladyResponse.Models.ToList();

                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о складах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Загружает report
        private async Task UpdateOstatkiDataGrid()
        {
            // 1. Проверка выбора склада
            if (SkladOstatkiComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение Id склада
            Guid idSklada = ((Models.Склады)SkladOstatkiComboBox.SelectedItem).id;

            // 3. Загрузка данных об остатках
            List<OtchetPoOstatkamItem> ostatkiData = await GetOtchetPoOstatkamData(idSklada);

            // 4. Обновление DataGrid
            OstatkiDataGrid.ItemsSource = null;
            OstatkiDataGrid.ItemsSource = ostatkiData;
            // 5. Очистить столбцы перед добавлением новых
            OstatkiDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn();
            nazvanieColumn.Header = "Название товара";
            nazvanieColumn.Binding = new Binding("название_товара");
            OstatkiDataGrid.Columns.Add(nazvanieColumn);

            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn();
            kolichestvoColumn.Header = "Количество";
            kolichestvoColumn.Binding = new Binding("количество");
            OstatkiDataGrid.Columns.Add(kolichestvoColumn);
        }
        private async void FormOtchetPoOborotamButton_Click(object sender, RoutedEventArgs e)
        {
            List<OtchetPoOborotamItem> oborotData = await GetOtchetPoOborotamData();
            UpdateOborotDataGrid(oborotData);
        }
        private async Task<List<OtchetPoOborotamItem>> GetOtchetPoOborotamData()
        {
            List<OtchetPoOborotamItem> oborotData = new List<OtchetPoOborotamItem>();

            try
            {
                // a. Get all products
                var tovaryResponse = await App.SupabaseClient.From<Models.Товары>().Get();
                List<Models.Товары> tovary = tovaryResponse?.Models;
                if (tovary == null)
                {
                    MessageBox.Show("Не удалось загрузить данные о товарах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return oborotData;
                }

                // b. Get приходные и расходные operations
                var prihodResponse = await App.SupabaseClient.From<Models.ТоварыПриходнойНакладной2>().Get();
                var rashodResponse = await App.SupabaseClient.From<Models.ТоварыРасходнойНакладной2>().Get();

                List<Models.ТоварыПриходнойНакладной2> prihodTovary = prihodResponse?.Models;
                List<Models.ТоварыРасходнойНакладной2> rashodTovary = rashodResponse?.Models;

                if (prihodTovary == null) { MessageBox.Show("Не удалось загрузить данные о приходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return oborotData; }
                if (rashodTovary == null) { MessageBox.Show("Не удалось загрузить данные о расходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return oborotData; }

                //4. Form Отчет
                foreach (var tovar in tovary)
                {
                    Guid idTovara = tovar.id;

                    // a.  Получаем приход и расход по товару
                    var prihodZaPeriod = prihodTovary.Where(y => y.id_товара == idTovara
                    ).Sum(x => x.количество);

                    var rashodZaPeriod = rashodTovary.Where(y => y.id_товара == idTovara
                     ).Sum(x => x.количество);
                    // c. Сreate OtchetPoOborotamItem
                    OtchetPoOborotamItem oborotItem = new OtchetPoOborotamItem
                    {
                        название_товара = tovar.название_товара,
                        приход = prihodZaPeriod,
                        расход = rashodZaPeriod
                    };
                    oborotData.Add(oborotItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }

            return oborotData;
        }
        private void UpdateOborotDataGrid(List<OtchetPoOborotamItem> data)
        {
            ContentDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn();
            nazvanieColumn.Header = "Название товара";
            nazvanieColumn.Binding = new Binding("название_товара");
            ContentDataGrid.Columns.Add(nazvanieColumn);

            DataGridTextColumn prihodColumn = new DataGridTextColumn();
            prihodColumn.Header = "Приход";
            prihodColumn.Binding = new Binding("приход");
            ContentDataGrid.Columns.Add(prihodColumn);

            DataGridTextColumn rashodColumn = new DataGridTextColumn();
            rashodColumn.Header = "Расход";
            rashodColumn.Binding = new Binding("расход");
            ContentDataGrid.Columns.Add(rashodColumn);

            ContentDataGrid.ItemsSource = data;
        }
        private async Task<List<OtchetPoSkladuItem>> GetOtchetPoSkladamData()
        {
            List<OtchetPoSkladuItem> otchetData = new List<OtchetPoSkladuItem>();

            try
            {
                var skladyResponse = await App.SupabaseClient
                    .From<Models.Склады>()
                    .Select("*")
                    .Get();

                if (skladyResponse.Models != null)
                {
                    // Группируем склады по типу склада
                    var groupedSklady = skladyResponse.Models
                        .GroupBy(s => s.тип_склада);

                    // Формируем отчет
                    foreach (var group in groupedSklady)
                    {
                        otchetData.Add(new OtchetPoSkladuItem
                        {
                            тип_склада = group.Key,
                            количество = group.Count()
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при формировании данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при формировании данных: {ex}");
            }

            return otchetData;
        }
        private void UpdateDataGrid(List<OtchetPoSkladuItem> data)
        {
            SkladyDataGrid.Columns.Clear();

            DataGridTextColumn tipSkladaColumn = new DataGridTextColumn();
            tipSkladaColumn.Header = "Тип склада";
            tipSkladaColumn.Binding = new Binding("тип_склада");
            SkladyDataGrid.Columns.Add(tipSkladaColumn);

            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn();
            kolichestvoColumn.Header = "Количество";
            kolichestvoColumn.Binding = new Binding("количество");
            SkladyDataGrid.Columns.Add(kolichestvoColumn);

            SkladyDataGrid.ItemsSource = data;
        }

        private async Task<bool> LoadSkladyDataOstatki()
        {
            bool returnValue = true;
            try
            {
                var skladyResponse = await App.SupabaseClient
                    .From<Models.Склады>()
                    .Select("*")
                    .Get();

                if (skladyResponse.Models != null)
                {
                    _skladyData = skladyResponse.Models.ToList();
                    SkladOstatkiComboBox.ItemsSource = _skladyData;
                    SkladOstatkiComboBox.DisplayMemberPath = "название_склада";
                    SkladOstatkiComboBox.SelectedValuePath = "id";
                    SkladOstatkiComboBox.SelectionChanged += SkladOstatkiComboBox_SelectionChanged;
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    returnValue = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о складах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                returnValue = false;
            }
            return returnValue;
        }

        private async void FormOtchetPoOstatkamButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка выбора склада
            if (SkladOstatkiComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение Id склада
            Guid idSklada = ((Models.Склады)SkladOstatkiComboBox.SelectedItem).id;

            // 3. Загрузка данных об остатках
            List<OtchetPoOstatkamItem> ostatkiData = await GetOtchetPoOstatkamData(idSklada);

            // 4. Обновление DataGrid
            UpdateOstatkiDataGrid(ostatkiData);
        }
        private async Task<List<OtchetPoOstatkamItem>> GetOtchetPoOstatkamData(Guid idSklada)
        {
            List<OtchetPoOstatkamItem> otchetData = new List<OtchetPoOstatkamItem>();

            try
            {
                // a. Получение данных об остатках на складе
                var ostatkiResponse = await App.SupabaseClient
                    .From<Models.ОстаткиТоваров>()
                    .Get();

                if (ostatkiResponse.Models != null)
                {
                    // b. Фильтруем остатки по выбранному складу
                    var filteredOstatki = ostatkiResponse.Models
                        .Where(x => x.id_склада == idSklada);

                    // c. Формируем отчет
                    foreach (var ostatok in filteredOstatki)
                    {
                        otchetData.Add(new OtchetPoOstatkamItem
                        {
                            название_товара = await GetTovarName(ostatok.id_товара),
                            количество = ostatok.количество
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные об остатках складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при формировании данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при формировании данных: {ex}");
            }
            return otchetData;
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
        private void UpdateOstatkiDataGrid(List<OtchetPoOstatkamItem> data)
        {
            OstatkiDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn();
            nazvanieColumn.Header = "Название товара";
            nazvanieColumn.Binding = new Binding("название_товара");
            OstatkiDataGrid.Columns.Add(nazvanieColumn);

            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn();
            kolichestvoColumn.Header = "Количество";
            kolichestvoColumn.Binding = new Binding("количество");
            OstatkiDataGrid.Columns.Add(kolichestvoColumn);

            OstatkiDataGrid.ItemsSource = data;
        }
        // Вспомогательный класс для хранения информации об обороте товара
        public class OtchetPoOborotamItem
        {
            public string название_товара { get; set; }
            public int приход { get; set; }
            public int расход { get; set; }
        }
        public class OtchetPoSkladuItem
        {
            public string тип_склада { get; set; }
            public int количество { get; set; }
        }
        // Вспомогательный класс для хранения информации об остатках
        public class OtchetPoOstatkamItem
        {
            public string название_товара { get; set; }
            public int количество { get; set; }

        }
        private async void SkladOstatkiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await UpdateOstatkiDataGrid();
        }
        //Добавленные функции для создания report по инвентаризации

        private async void FormInventarizatsiyaButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка обязательных полей
            if (InventSkladComboBox.SelectedItem == null || InventDataNachalaDatePicker.SelectedDate == null || InventDataOkonchaniyaDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите склад, дату начала и дату окончания.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение id_склада
            if (!Guid.TryParse(InventSkladComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Получение даты начала и окончания
            DateTime dataNachala = InventDataNachalaDatePicker.SelectedDate.Value;
            DateTime dataOkonchaniya = InventDataOkonchaniyaDatePicker.SelectedDate.Value;

            if (dataNachala > dataOkonchaniya)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 4. Получение всех товаров на выбранном складе
                var tovaryResponse = await App.SupabaseClient.From<Models.Товары>().Get();
                var prihodResponse = await App.SupabaseClient.From<Models.ПриходныеНакладные>().Get();
                var rashodResponse = await App.SupabaseClient.From<Models.РасходныеНакладные>().Get();
                var tovaryPrihodResponse = await App.SupabaseClient.From<Models.ТоварыПриходнойНакладной2>().Get();
                var tovaryRashodResponse = await App.SupabaseClient.From<Models.ТоварыРасходнойНакладной2>().Get();

                // 5. Создание списка для отображения данных инвентаризации
                List<ТоварыИнвентаризацииViewModel> inventTovary = new List<ТоварыИнвентаризацииViewModel>();

                // 6. Подготовка коллекций данных, нужных для выборки
                List<Models.Товары> tovary = tovaryResponse?.Models;
                List<Models.ПриходныеНакладные> prihodNaladnie = prihodResponse?.Models;
                List<Models.РасходныеНакладные> rashodNaladnie = rashodResponse?.Models;
                List<Models.ТоварыПриходнойНакладной2> tovaryPrihod = tovaryPrihodResponse?.Models;
                List<Models.ТоварыРасходнойНакладной2> tovaryRashod = tovaryRashodResponse?.Models;

                List<Models.ПриходныеНакладные> naladniePrihod = prihodResponse?.Models;
                List<Models.РасходныеНакладные> naladnieRashod = rashodResponse?.Models;


                if (tovary == null) { MessageBox.Show("Не удалось загрузить данные о товарах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (prihodNaladnie == null) { MessageBox.Show("Не удалось загрузить данные о приходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (rashodNaladnie == null) { MessageBox.Show("Не удалось загрузить данные о расходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (tovaryPrihod == null) { MessageBox.Show("Не удалось загрузить данные о товарах в приходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                if (tovaryRashod == null) { MessageBox.Show("Не удалось загрузить данные о товарах в расходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }



                // 7. Формирование выходных данных
                foreach (var tovar in tovary)
                {
                    Guid idTovara = tovar.id;

                    // a. Фильтруем все операции по товару и месяцу
                    var prihodZaPeriod = tovaryPrihod.Where(x =>
                        naladniePrihod.Any(y => y.id == x.id_приходной_накладной && y.id_склада == idSklada && y.дата_поступления >= dataNachala && y.дата_поступления <= dataOkonchaniya)
                        && x.id_товара == idTovara
                    ).Sum(x => x.количество);

                    var rashodZaPeriod = tovaryRashod.Where(x =>
                         naladnieRashod.Any(y => y.id == x.id_расходной_накладной && y.id_склада == idSklada && y.дата_отгрузки >= dataNachala && y.дата_отгрузки <= dataOkonchaniya)
                         && x.id_товара == idTovara
                     ).Sum(x => x.количество);

                    int expectedQuantity = prihodZaPeriod - rashodZaPeriod;

                    // b. Создание объекта для отображения в DataGrid
                    ТоварыИнвентаризацииViewModel inventTovar = new ТоварыИнвентаризацииViewModel
                    {
                        id_товара = tovar.id,
                        название_товара = tovar.название_товара,
                        ожидаемое_количество = expectedQuantity,
                        фактическое_количество = expectedQuantity, // Дублируем значение
                        расхождение = "Отсутствует", // По умолчанию
                        примечания = "", // По умолчанию,
                        id_инвентаризации = Guid.NewGuid()
                    };

                    inventTovary.Add(inventTovar);
                }

                // 8. Отображение данных в DataGrid
                TovaryInventDataGrid.ItemsSource = inventTovary;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }
        //Теперь мы в праве сохранить report
        private async void SaveInventarizatsiyaButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на заполненность обязательных полей
            if (InventSkladComboBox.SelectedItem == null || InventDataNachalaDatePicker.SelectedDate == null || InventDataOkonchaniyaDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение id_склада
            if (!Guid.TryParse(InventSkladComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Получение даты начала и окончания
            DateTime dataNachala = InventDataNachalaDatePicker.SelectedDate.Value;
            DateTime dataOkonchaniya = InventDataOkonchaniyaDatePicker.SelectedDate.Value;

            if (dataNachala > dataOkonchaniya)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 4. Get the data
            List<ТоварыИнвентаризацииViewModel> inventTovary = TovaryInventDataGrid.ItemsSource as List<ТоварыИнвентаризацииViewModel>;

            if (inventTovary == null || !inventTovary.Any())
            {
                MessageBox.Show("Нет данных для сохранения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                // a. Get the current user's ID
                var userId = App.SupabaseClient.Auth.CurrentUser.Id;

                // 5. Create a new Инвентаризации object
                Инвентаризации newInventarizatsiya = new Инвентаризации
                {
                    id = inventTovary.First().id_инвентаризации,
                    id_склада = idSklada,
                    дата_проведения = DateTime.UtcNow, // set the current date and time
                    ответственный_пользователь_id = Guid.Parse(userId), // the logged in user
                    результат = "Расхождений нет", // set the result to "Расхождений нет"
                    дата_начала = dataNachala,
                    дата_окончания = dataOkonchaniya,
                    ссылка_на_документ = null // we'll add this later
                };

                var insertResponse = await App.SupabaseClient
                        .From<Models.Инвентаризации>()
                        .Insert(newInventarizatsiya);

                if (insertResponse != null && insertResponse.Models != null && insertResponse.Models.Any())
                {
                    Guid idInventarizacii = insertResponse.Models.First().id;

                    // 6. Iterate through the DataGrid rows
                    foreach (var tovarInvent in inventTovary)
                    {
                        // 7. Create a new Товары_инвентаризации object
                        int? rashozhdenie = null;
                        if (!string.IsNullOrEmpty(tovarInvent.расхождение) && int.TryParse(tovarInvent.расхождение, out int parsedRashozhdenie))
                        {
                            rashozhdenie = parsedRashozhdenie;
                        }

                        Товары_инвентаризации newTovarInvent = new Товары_инвентаризации
                        {
                            id = Guid.NewGuid(),
                            id_инвентаризации = idInventarizacii,
                            id_товара = tovarInvent.id_товара,
                            фактическое_количество = tovarInvent.фактическое_количество,
                            ожидаемое_количество = tovarInvent.ожидаемое_количество,
                            расхождение = rashozhdenie,
                            примечания = tovarInvent.примечания
                        };

                        var insertTovInventResponse = await App.SupabaseClient
                         .From<Models.Товары_инвентаризации>()
                         .Insert(newTovarInvent);

                        if (insertTovInventResponse?.ResponseMessage?.IsSuccessStatusCode == false)
                        {
                            MessageBox.Show($"Ошибка при создании товаров инвентаризации: {insertTovInventResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine($"Ошибка при создании товаров инвентаризации: {insertTovInventResponse.ResponseMessage.ReasonPhrase}");
                            return;
                        }
                    }
                    MessageBox.Show("Товары инвентаризации успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка при создании отчета: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Ошибка при создании отчета: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}");
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }
        private async Task LoadSkladyDataInvent()
        {
            try
            {
                var skladyResponse = await App.SupabaseClient
                    .From<Models.Склады>()
                    .Select("*")
                    .Get();

                if (skladyResponse.Models != null)
                {
                    _skladyData = skladyResponse.Models.ToList();
                    InventSkladComboBox.ItemsSource = _skladyData;
                    InventSkladComboBox.DisplayMemberPath = "название_склада";
                    InventSkladComboBox.SelectedValuePath = "id";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о складах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о складах: {ex}");
            }
        }
    }
    public class ТоварыИнвентаризацииViewModel
    {
        public Guid id_товара { get; set; }
        public string название_товара { get; set; }
        public int ожидаемое_количество { get; set; }
        public int? фактическое_количество { get; set; }
        public string расхождение { get; set; } // Изменили тип на string
        public string примечания { get; set; }

        public Guid id_инвентаризации { get; set; }
        public DateTime? дата_начала { get; set; }
        public DateTime? дата_окончания { get; set; }
    }

}