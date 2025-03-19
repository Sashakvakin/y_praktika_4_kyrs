using MegaSklad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Mail;
using System.Net;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using Supabase.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MegaSklad
{
    public partial class ManagerMainWindow : Window
    {
        private List<Поставщики> _postavshikiData;
        private List<Склады> _skladyData;
        private List<Клиенты> _klientiData;
        private List<ТоварыЗаказаПоставщикуViewModel> _tovaryVzakaze = new List<ТоварыЗаказаПоставщикуViewModel>();
        private List<ТоварыЗаказаКлиентуViewModel> _tovaryClientVzakaze = new List<ТоварыЗаказаКлиентуViewModel>();
        private List<TovaryNaSkladeViewModel> _tovaryNaSkladeData = new List<TovaryNaSkladeViewModel>();
        private List<TovaryNaSkladeViewModel> _filteredTovaryNaSkladeData = new List<TovaryNaSkladeViewModel>();

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ManagerMainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ManagerMainWindow_Loaded;
        }

        private async void ManagerMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUserProfileData();
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
        private async void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            string menuName = ((Button)sender).Tag.ToString();

            ZakazPostavshikuPanel.Visibility = Visibility.Collapsed;
            ZakazClientuPanel.Visibility = Visibility.Collapsed;
            ProsmotrSkladaPanel.Visibility = Visibility.Collapsed;
            ProsmotrNakladnihPanel.Visibility = Visibility.Collapsed;
            ControlZakazovPanel.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Collapsed; // Скрываем панель профиля

            switch (menuName)
            {
                case "ЗаказПоставщику":
                    ZakazPostavshikuPanel.Visibility = Visibility.Visible;
                    await LoadPostavshikiData();
                    await LoadSkladyData();
                    break;
                case "ЗаказКлиенту":
                    ZakazClientuPanel.Visibility = Visibility.Visible;
                    await LoadKlientiData();
                    await LoadSkladyClientData();
                    break;
                case "Склады":
                    ProsmotrSkladaPanel.Visibility = Visibility.Visible;
                    await LoadSkladyDataInvent();
                    break;
                case "ПриходныеНакладные":
                case "РасходныеНакладные":
                    ProsmotrNakladnihPanel.Visibility = Visibility.Visible;
                    await LoadNakladnieData();
                    break;
                case "Заказы":
                    ControlZakazovPanel.Visibility = Visibility.Visible;
                    await LoadZakazyPostavshikamData(); // Загружаем данные по умолчанию
                    break;
                case "Клиенты":
                    OpenAddClientWindow();
                    break;
                case "Поставщики":
                    OpenAddPostavshikWindow();
                    break;
                case "Профиль":
                    ProfilePanel.Visibility = Visibility.Visible;
                    await LoadUserProfileData();
                    break;
                default:
                    break;
            }
        }


        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow(UserProfile);
            if (editProfileWindow.ShowDialog() == true)
            {
                LoadProfileImage(); // reloads the image

            }
        }
        private async void OpenAddClientWindow()
        {
            Клиенты newKlient = new Клиенты();
            EditClientWindow addWindow = new EditClientWindow(newKlient, true); // true для режима "добавить"
            if (addWindow.ShowDialog() == true)
            {
                //Клиент успешно добавлен - обновить список клиентов (если требуется)
            }
        }

        private async void OpenAddPostavshikWindow()
        {
            Поставщики newPostavshik = new Поставщики();
            EditPostavshikWindow addWindow = new EditPostavshikWindow(newPostavshik, true); // true для режима "добавить"
            if (addWindow.ShowDialog() == true)
            {
                //Поставщик успешно добавлен - обновить список поставщиков (если требуется)
            }
        }

        private async Task LoadPostavshikiData()
        {
            try
            {
                var postavshikiResponse = await App.SupabaseClient
                    .From<Models.Поставщики>()
                    .Select("*")
                    .Get();

                if (postavshikiResponse.Models != null)
                {
                    _postavshikiData = postavshikiResponse.Models.ToList();
                    PostavshikComboBox.ItemsSource = _postavshikiData;
                    PostavshikComboBox.DisplayMemberPath = "название_поставщика";
                    PostavshikComboBox.SelectedValuePath = "id";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о поставщиках.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о поставщиках: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о поставщиках: {ex}");
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
                    SkladComboBox.ItemsSource = _skladyData;
                    SkladComboBox.DisplayMemberPath = "название_склада";
                    SkladComboBox.SelectedValuePath = "id";
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

        private async void AddTovarToZakazButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Check Sklad
            if (SkladComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Guid.TryParse(SkladComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addTovarWindow = new AddTovarZakazPostavshikyWindow();
            if (addTovarWindow.ShowDialog() == true)
            {
                if (addTovarWindow.NewTovar != null)
                {
                    _tovaryVzakaze.Add(addTovarWindow.NewTovar);
                    UpdateTovaryDataGrid();
                }
            }
        }

        // ManagerMainWindow.xaml.cs
        private async void SaveZakazButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на заполненность обязательных полей
            if (PostavshikComboBox.SelectedItem == null || SkladComboBox.SelectedItem == null || DataOzhidaemoyPostavkiDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение id_поставщика и id_склада
            if (PostavshikComboBox.SelectedValue == null || !Guid.TryParse(PostavshikComboBox.SelectedValue.ToString(), out Guid idPostavshika))
            {
                MessageBox.Show("Некорректный формат данных в поле Поставщик.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SkladComboBox.SelectedValue == null || !Guid.TryParse(SkladComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Создание объекта ЗаказПоставщику
            try
            {
                ЗаказПоставщику newZakaz = new ЗаказПоставщику
                {
                    id = Guid.NewGuid(),
                    id_поставщика = idPostavshika,
                    дата_заказа = DateTime.Now,
                    дата_ожидаемой_поставки = DataOzhidaemoyPostavkiDatePicker.SelectedDate,
                    id_склада = idSklada,
                    статус = "Создан", // По умолчанию
                    примечания = PrimechanieTextBox.Text
                };

                // 4. Вставка данных в таблицу Заказы_поставщикам
                var insertResponse = await App.SupabaseClient
                    .From<Models.ЗаказПоставщику>()
                    .Insert(newZakaz);

                if (insertResponse != null && insertResponse.Models != null && insertResponse.Models.Any())
                {
                    Guid zakazId = insertResponse.Models.First().id;

                    // 5. Сохранение товаров в таблицу Товары_заказа_поставщику
                    foreach (var tovarVzakaze in _tovaryVzakaze)
                    {
                        ТоварыЗаказаПоставщику newTovarZakazDTO = new ТоварыЗаказаПоставщику
                        {
                            id = Guid.NewGuid(),
                            id_заказа_поставщику = zakazId,
                            id_товара = tovarVzakaze.id_товара, // Get id
                            количество = tovarVzakaze.количество
                        };

                        var insertTovarResponse = await App.SupabaseClient
                            .From<Models.ТоварыЗаказаПоставщику>()
                            .Insert(newTovarZakazDTO);

                        if (insertTovarResponse?.ResponseMessage?.IsSuccessStatusCode == false)
                        {
                            MessageBox.Show($"Ошибка при создании строки заказа: {insertTovarResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine($"Ошибка при создании строки заказа: {insertTovarResponse.ResponseMessage.ReasonPhrase}");
                        }
                    }

                    MessageBox.Show("Заказ поставщику успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show($"Ошибка при создании заказа: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Ошибка при создании заказа: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }

        private void ClearForm()
        {
            PostavshikComboBox.SelectedItem = null;
            SkladComboBox.SelectedItem = null;
            DataOzhidaemoyPostavkiDatePicker.SelectedDate = null;
            PrimechanieTextBox.Text = string.Empty;
            _tovaryVzakaze.Clear();
            UpdateTovaryDataGrid();
        }

        private void UpdateTovaryDataGrid()
        {
            TovaryDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn { Header = "Название товара", Binding = new Binding("название_товара") };
            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn { Header = "Количество", Binding = new Binding("количество") };
            DataGridTextColumn cenaColumn = new DataGridTextColumn { Header = "Цена за единицу", Binding = new Binding("цена_за_единицу") };

            TovaryDataGrid.Columns.Add(nazvanieColumn);
            TovaryDataGrid.Columns.Add(kolichestvoColumn);
            TovaryDataGrid.Columns.Add(cenaColumn);

            TovaryDataGrid.ItemsSource = null;
            TovaryDataGrid.ItemsSource = _tovaryVzakaze;
        }
        private async Task LoadKlientiData()
        {
            try
            {
                var klientiResponse = await App.SupabaseClient
                    .From<Models.Клиенты>()
                    .Select("*")
                    .Get();

                if (klientiResponse.Models != null)
                {
                    _klientiData = klientiResponse.Models.ToList();
                    KlientComboBox.ItemsSource = _klientiData;
                    KlientComboBox.DisplayMemberPath = "название_клиента";
                    KlientComboBox.SelectedValuePath = "id";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о клиентах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о клиентах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о клиентах: {ex}");
            }
        }

        private async Task LoadSkladyClientData()
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
                    SkladClientComboBox.ItemsSource = _skladyData;
                    SkladClientComboBox.DisplayMemberPath = "название_склада";
                    SkladClientComboBox.SelectedValuePath = "id";
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

        private async void AddTovarToZakazClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addTovarWindow = new AddTovarZakazClientWindow();
            if (addTovarWindow.ShowDialog() == true)
            {
                if (addTovarWindow.NewTovar != null)
                {
                    _tovaryClientVzakaze.Add(addTovarWindow.NewTovar);
                    UpdateTovaryClientDataGrid();
                }
            }
        }

        private async void SaveZakazClientButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на заполненность обязательных полей
            if (KlientComboBox.SelectedItem == null || SkladClientComboBox.SelectedItem == null || DataOzhidaemoyOtgruzkiDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение id_клиента и id_склада
            if (KlientComboBox.SelectedValue == null || !Guid.TryParse(KlientComboBox.SelectedValue.ToString(), out Guid idKlienta))
            {
                MessageBox.Show("Некорректный формат данных в поле Клиент.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SkladClientComboBox.SelectedValue == null || !Guid.TryParse(SkladClientComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Создание объекта ЗаказКлиенту
            try
            {
                ЗаказКлиенту newZakaz = new ЗаказКлиенту
                {
                    id = Guid.NewGuid(),
                    id_клиента = idKlienta,
                    дата_заказа = DateTime.Now,
                    дата_ожидаемой_отгрузки = DataOzhidaemoyOtgruzkiDatePicker.SelectedDate,
                    id_склада = idSklada,
                    статус = "Создан", // По умолчанию
                    примечания = PrimechanieClientTextBox.Text
                };

                // 4. Вставка данных в таблицу Заказы_клиентам
                var insertResponse = await App.SupabaseClient
                    .From<Models.ЗаказКлиенту>()
                    .Insert(newZakaz);

                if (insertResponse != null && insertResponse.Models != null && insertResponse.Models.Any())
                {
                    Guid zakazId = insertResponse.Models.First().id;

                    // 5. Сохранение товаров в таблицу Товары_заказа_клиенту
                    foreach (var tovarVzakaze in _tovaryClientVzakaze)
                    {
                        ТоварыЗаказаКлиенту newTovarZakazDTO = new ТоварыЗаказаКлиенту
                        {
                            id = Guid.NewGuid(),
                            id_заказа_клиенту = zakazId,
                            id_товара = tovarVzakaze.id_товара,
                            количество = tovarVzakaze.количество
                        };

                        var insertTovarResponse = await App.SupabaseClient
                            .From<Models.ТоварыЗаказаКлиенту>()
                            .Insert(newTovarZakazDTO);

                        if (insertTovarResponse?.ResponseMessage?.IsSuccessStatusCode == false)
                        {
                            MessageBox.Show($"Ошибка при создании строки заказа: {insertTovarResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine($"Ошибка при создании строки заказа: {insertTovarResponse.ResponseMessage.ReasonPhrase}");
                        }
                    }

                    MessageBox.Show("Заказ клиенту успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearClientForm();
                }
                else
                {
                    MessageBox.Show($"Ошибка при создании заказа: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Ошибка при создании заказа: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }

        private void ClearClientForm()
        {
            KlientComboBox.SelectedItem = null;
            SkladClientComboBox.SelectedItem = null;
            DataOzhidaemoyOtgruzkiDatePicker.SelectedDate = null;
            PrimechanieClientTextBox.Text = string.Empty;
            _tovaryClientVzakaze.Clear();
            UpdateTovaryClientDataGrid();
        }

        private void UpdateTovaryClientDataGrid()
        {
            TovaryClientDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn { Header = "Название товара", Binding = new Binding("название_товара") };
            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn { Header = "Количество", Binding = new Binding("количество") };
            DataGridTextColumn cenaColumn = new DataGridTextColumn { Header = "Цена за единицу", Binding = new Binding("цена_за_единицу") };

            TovaryClientDataGrid.Columns.Add(nazvanieColumn);
            TovaryClientDataGrid.Columns.Add(kolichestvoColumn);
            TovaryClientDataGrid.Columns.Add(cenaColumn);

            TovaryClientDataGrid.ItemsSource = null;
            TovaryClientDataGrid.ItemsSource = _tovaryClientVzakaze;
        }
        private async void SkladProsmotrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkladProsmotrComboBox.SelectedItem is Models.Склады selectedSklad)
            {
                await LoadTovaryNaSkladeData(selectedSklad.id);
            }
            else
            {
                TovaryNaSkladeDataGrid.ItemsSource = null;
            }
        }
        private async Task LoadTovaryNaSkladeData(Guid idSklada)
        {
            try
            {
                var ostatkiResponse = await App.SupabaseClient
                    .From<Models.ОстаткиТоваров>()
                    .Get(); // Get all

                if (ostatkiResponse.Models != null)
                {
                    // Filter in C#
                    _tovaryNaSkladeData = (await Task.WhenAll(ostatkiResponse.Models
                        .Where(x => x.id_склада == idSklada) // Filter by sklad ID
                        .Select(async o => new TovaryNaSkladeViewModel
                        {
                            название_товара = await GetTovarName(o.id_товара),
                            количество = o.количество
                        }))).ToList();

                    ApplySearchFilter();
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные об остатках товаров на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    TovaryNaSkladeDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных об остатках: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных об остатках: {ex}");
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

        private void PoiskTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            string searchText = PoiskTextBox.Text;

            if (string.IsNullOrEmpty(searchText))
            {
                // If the search text is empty, display all data
                TovaryNaSkladeDataGrid.ItemsSource = _tovaryNaSkladeData;
            }
            else
            {
                // If the search text is not empty, filter the data
                _filteredTovaryNaSkladeData = _tovaryNaSkladeData.Where(t =>
                    t.название_товара.ToLower().Contains(searchText.ToLower())
                ).ToList();
                TovaryNaSkladeDataGrid.ItemsSource = _filteredTovaryNaSkladeData;
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
                    SkladProsmotrComboBox.ItemsSource = _skladyData;
                    SkladProsmotrComboBox.DisplayMemberPath = "название_склада";
                    SkladProsmotrComboBox.SelectedValuePath = "id";
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
        private async Task LoadNakladnieData()
        {
            try
            {
                string selectedTipNakladnoy = ((ComboBoxItem)TipNakladnoyComboBox.SelectedItem).Content.ToString();

                if (selectedTipNakladnoy == "Приходные")
                {
                    // Load Prihodnie Nakladnie
                    var prihodResponse = await App.SupabaseClient
                        .From<Models.ПриходныеНакладные>()
                        .Select("*")
                        .Get();

                    if (prihodResponse.Models != null)
                    {
                        _prihodnieNakladnieData = (await Task.WhenAll(prihodResponse.Models.Select(async n => new ПриходныеНакладные2()
                        {
                            id = n.id,
                            дата_поступления = n.дата_поступления,
                            название_поставщика = await GetPostavshikName(n.id_поставщика),
                            название_склада = await GetSkladName(n.id_склада),
                            общая_сумма = n.общая_сумма,
                            примечания = n.примечания
                        }))).ToList();
                        PrihodnieNakladnieDataGrid.ItemsSource = _prihodnieNakladnieData;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить данные о приходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        PrihodnieNakladnieDataGrid.ItemsSource = null;
                    }
                    RashodnieNakladnieDataGrid.Visibility = Visibility.Collapsed;
                    PrihodnieNakladnieDataGrid.Visibility = Visibility.Visible;
                }
                else if (selectedTipNakladnoy == "Расходные")
                {
                    // Load Rashodnie Nakladnie
                    var rashodResponse = await App.SupabaseClient
                        .From<Models.РасходныеНакладные>()
                        .Select("*")
                        .Get();

                    if (rashodResponse.Models != null)
                    {
                        _rashodnieNakladnieData = (await Task.WhenAll(rashodResponse.Models.Select(async n => new РасходныеНакладные2()
                        {
                            id = n.id,
                            дата_отгрузки = n.дата_отгрузки,
                            название_клиента = await GetKlientName(n.id_клиента),
                            название_склада = await GetSkladName(n.id_склада),
                            общая_сумма = n.общая_сумма,
                            примечания = n.примечания
                        }))).ToList();
                        RashodnieNakladnieDataGrid.ItemsSource = _rashodnieNakladnieData;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить данные о расходных накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        RashodnieNakladnieDataGrid.ItemsSource = null;
                    }
                    PrihodnieNakladnieDataGrid.Visibility = Visibility.Collapsed;
                    RashodnieNakladnieDataGrid.Visibility = Visibility.Visible;
                }

                // Apply date filter
                ApplyDateFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке накладных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке накладных: {ex}");
            }
        }
        private async Task<string> GetPostavshikName(Guid idPostavshika)
        {
            try
            {
                var postavshikResponse = await App.SupabaseClient
                    .From<Models.Поставщики>()
                    .Where(x => x.id == idPostavshika)
                    .Select("название_поставщика")
                    .Get();

                if (postavshikResponse.Models != null && postavshikResponse.Models.Any())
                {
                    return postavshikResponse.Models.First().название_поставщика;
                }
                else
                {
                    return "Неизвестный поставщик";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке названия поставщика: {ex}");
                return "Неизвестный поставщик";
            }
        }
        private async Task<string> GetKlientName(Guid idKlienta)
        {
            try
            {
                var klientResponse = await App.SupabaseClient
                    .From<Models.Клиенты>()
                    .Where(x => x.id == idKlienta)
                    .Select("название_клиента")
                    .Get();

                if (klientResponse.Models != null && klientResponse.Models.Any())
                {
                    return klientResponse.Models.First().название_клиента;
                }
                else
                {
                    return "Неизвестный клиент";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке названия клиента: {ex}");
                return "Неизвестный клиент";
            }
        }
        private async Task<string> GetSkladName(Guid idSklada)
        {
            try
            {
                var skladResponse = await App.SupabaseClient
                    .From<Models.Склады>()
                    .Where(x => x.id == idSklada)
                    .Select("название_склада")
                    .Get();

                if (skladResponse.Models != null && skladResponse.Models.Any())
                {
                    return skladResponse.Models.First().название_склада;
                }
                else
                {
                    return "Неизвестный склад";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке названия склада: {ex}");
                return "Неизвестный склад";
            }
        }

        private void TipNakladnoyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reload data on selection
            LoadNakladnieData();
        }

        private void DataNakladnoyDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reapply filter
            ApplyDateFilter();
        }

        private void ApplyDateFilter()
        {
            string selectedTipNakladnoy = ((ComboBoxItem)TipNakladnoyComboBox.SelectedItem).Content.ToString(); // Объявляем здесь

            if (DataNakladnoyDatePicker.SelectedDate == null)
            {
                // No date selected, show all data
                if (selectedTipNakladnoy == "Приходные")
                {
                    PrihodnieNakladnieDataGrid.ItemsSource = _prihodnieNakladnieData;
                }
                else
                {
                    RashodnieNakladnieDataGrid.ItemsSource = _rashodnieNakladnieData;
                }
                return;
            }

            DateTime selectedDate = DataNakladnoyDatePicker.SelectedDate.Value.Date;

            if (selectedTipNakladnoy == "Приходные")
            {
                PrihodnieNakladnieDataGrid.ItemsSource = _prihodnieNakladnieData
                    .Where(n => n.дата_поступления.Date == selectedDate)
                    .ToList();
            }
            else
            {
                RashodnieNakladnieDataGrid.ItemsSource = _rashodnieNakladnieData
                    .Where(n => n.дата_отгрузки.Date == selectedDate)
                    .ToList();
            }
        }
        private void PrihodButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid nakladnayaId)
            {
                PrihodnayaNakladnayaDetailsWindow detailsWindow = new PrihodnayaNakladnayaDetailsWindow(nakladnayaId);
                detailsWindow.Show();
            }
        }
        private void RashodButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid nakladnayaId)
            {
                RashodnayaNakladnayaDetailsWindow detailsWindow = new RashodnayaNakladnayaDetailsWindow(nakladnayaId);
                detailsWindow.Show();
            }
        }
        //Загрузка заказов поставщикам
        //Загрузка заказов поставщикам
        private async Task LoadZakazyPostavshikamData()
        {
            try
            {
                var zakazyResponse = await App.SupabaseClient
                    .From<Models.ЗаказПоставщику>()
                    .Select("*")
                    .Get();

                if (zakazyResponse.Models != null)
                {
                    // Преобразование данных, чтобы включить название поставщика и склада
                    List<ЗаказПоставщикуViewModel> заказыViewModel = (await Task.WhenAll(zakazyResponse.Models.Select(async z => new ЗаказПоставщикуViewModel()
                    {
                        id = z.id,
                        дата_заказа = z.дата_заказа,
                        дата_ожидаемой_поставки = z.дата_ожидаемой_поставки,
                        название_поставщика = await GetPostavshikName(z.id_поставщика),
                        название_склада = await GetSkladName(z.id_склада),
                        статус = z.статус,
                        примечания = z.примечания
                    }))).ToList();

                    // Отображение данных
                    UpdateZakazyDataGrid(заказыViewModel, "ЗаказыПоставщикам");
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о заказах поставщикам.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о заказах поставщикам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о заказах поставщикам: {ex}");
            }
        }

        // Загрузка заказов клиентам
        private async Task LoadZakazyKlientamData()
        {
            try
            {
                var zakazyResponse = await App.SupabaseClient
                    .From<Models.ЗаказКлиенту>()
                    .Select("*")
                    .Get();

                if (zakazyResponse.Models != null)
                {
                    // Преобразование данных, чтобы включить название клиента и склада
                    List<ЗаказКлиентуViewModel> заказыViewModel = (await Task.WhenAll(zakazyResponse.Models.Select(async z => new ЗаказКлиентуViewModel()
                    {
                        id = z.id,
                        дата_заказа = z.дата_заказа,
                        дата_ожидаемой_отгрузки = z.дата_ожидаемой_отгрузки,
                        название_клиента = await GetKlientName(z.id_клиента),
                        название_склада = await GetSkladName(z.id_склада),
                        статус = z.статус,
                        примечания = z.примечания
                    }))).ToList();

                    // Отображение данных
                    UpdateZakazyDataGrid(заказыViewModel, "ЗаказыКлиентам");
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о заказах клиентам.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о заказах клиентам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при загрузке данных о заказах клиентам: {ex}");
            }
        }

        private void UpdateZakazyDataGrid<T>(List<T> data, string dataType)
        {
            ZakazyDataGrid.Columns.Clear(); // Очистка старых столбцов

            DataGridTextColumn номерColumn = new DataGridTextColumn();
            номерColumn.Header = "Номер";
            номерColumn.Binding = new Binding("id");
            номерColumn.Width = 75; // Устанавливаем ширину столбца
            ZakazyDataGrid.Columns.Add(номерColumn);

            DataGridTextColumn датаЗаказаColumn = new DataGridTextColumn();
            датаЗаказаColumn.Header = "Дата заказа";
            датаЗаказаColumn.Binding = new Binding("дата_заказа") { StringFormat = "dd.MM.yyyy" };
            ZakazyDataGrid.Columns.Add(датаЗаказаColumn);

            DataGridTextColumn датаОжидаемойОтгрузкиColumn = new DataGridTextColumn();

            // Изменение в названии столбца в зависимости от типа заказа
            датаОжидаемойОтгрузкиColumn.Header = (dataType == "ЗаказыПоставщикам") ? "Дата поставки" : "Дата отгрузки";
            датаОжидаемойОтгрузкиColumn.Binding = new Binding(
                (dataType == "ЗаказыПоставщикам") ? "дата_ожидаемой_поставки" : "дата_ожидаемой_отгрузки")
            { StringFormat = "dd.MM.yyyy" };
            ZakazyDataGrid.Columns.Add(датаОжидаемойОтгрузкиColumn);

            if (dataType == "ЗаказыПоставщикам")
            {
                DataGridTextColumn поставщикColumn = new DataGridTextColumn();
                поставщикColumn.Header = "Поставщик";
                поставщикColumn.Binding = new Binding("название_поставщика");
                ZakazyDataGrid.Columns.Add(поставщикColumn);
            }
            else if (dataType == "ЗаказыКлиентам")
            {
                DataGridTextColumn клиентColumn = new DataGridTextColumn();
                клиентColumn.Header = "Клиент";
                клиентColumn.Binding = new Binding("название_клиента");
                ZakazyDataGrid.Columns.Add(клиентColumn);
            }

            DataGridTextColumn складColumn = new DataGridTextColumn();
            складColumn.Header = "Склад";
            складColumn.Binding = new Binding("название_склада");
            ZakazyDataGrid.Columns.Add(складColumn);

            DataGridTextColumn статусColumn = new DataGridTextColumn();
            статусColumn.Header = "Статус";
            статусColumn.Binding = new Binding("статус");
            ZakazyDataGrid.Columns.Add(статусColumn);

            DataGridTextColumn примечанияColumn = new DataGridTextColumn();
            примечанияColumn.Header = "Примечание";
            примечанияColumn.Binding = new Binding("примечания");
            ZakazyDataGrid.Columns.Add(примечанияColumn);

            // Добавляем кнопку "Подробнее"
            DataGridTemplateColumn podrobneeColumn = new DataGridTemplateColumn();
            podrobneeColumn.Header = "Действия";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.SetValue(ContentProperty, "Подробнее");
            buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(PodrobneeButton_Click));
            buttonFactory.SetBinding(TagProperty, new Binding("id")); // Передаем ID заказа
            buttonTemplate.VisualTree = buttonFactory;
            podrobneeColumn.CellTemplate = buttonTemplate;
            ZakazyDataGrid.Columns.Add(podrobneeColumn);

            // Добавляем кнопку "Изменить статус"
            DataGridTemplateColumn izmenitStatusColumn = new DataGridTemplateColumn();
            izmenitStatusColumn.Header = "Статус";
            DataTemplate buttonStatusTemplate = new DataTemplate();
            FrameworkElementFactory buttonStatusFactory = new FrameworkElementFactory(typeof(Button));
            buttonStatusFactory.SetValue(ContentProperty, "Изменить статус");
            buttonStatusFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(IzmenitStatusButton_Click));
            buttonStatusFactory.SetBinding(TagProperty, new Binding("id")); // Передаем ID заказа
            buttonStatusTemplate.VisualTree = buttonStatusFactory;
            izmenitStatusColumn.CellTemplate = buttonStatusTemplate;
            ZakazyDataGrid.Columns.Add(izmenitStatusColumn);

            ZakazyDataGrid.ItemsSource = data;
        }
        private async void IzmenitStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag is Guid zakazId)
            {
                // Создаем и открываем окно для изменения статуса
                EditZakazStatusWindow editWindow = new EditZakazStatusWindow();
                if (editWindow.ShowDialog() == true)
                {
                    // Получаем новый статус и обновляем базу данных
                    string newStatus = editWindow.NewStatus;
                    await UpdateZakazStatus(zakazId, newStatus);
                }
            }
        }
        private async Task UpdateZakazStatus(Guid zakazId, string newStatus)
        {
            try
            {
                if (ControlZakazovPanel.Visibility == Visibility.Visible)
                {
                    RadioButton selectedRadioButton = ControlZakazovPanel.Children.OfType<StackPanel>()
                      .FirstOrDefault()
                      ?.Children.OfType<RadioButton>()
                      .FirstOrDefault(r => r.IsChecked == true);
                    if (selectedRadioButton.Tag.ToString() == "ЗаказыПоставщикам")
                    {
                        var updateResponse = await App.SupabaseClient
                      .From<Models.ЗаказПоставщику>()
                      .Where(x => x.id == zakazId)
                      .Set(x => x.статус, newStatus)
                      .Update();

                        if (updateResponse != null && updateResponse.ResponseMessage != null && updateResponse.ResponseMessage.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Статус заказа успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadZakazyPostavshikamData(); // Refresh DataGrid
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка при обновлении статуса заказа: {updateResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        return;
                    }
                    if (selectedRadioButton.Tag.ToString() == "ЗаказыКлиентам")
                    {
                        var updateResponse = await App.SupabaseClient
                      .From<Models.ЗаказКлиенту>()
                      .Where(x => x.id == zakazId)
                      .Set(x => x.статус, newStatus)
                      .Update();

                        if (updateResponse != null && updateResponse.ResponseMessage != null && updateResponse.ResponseMessage.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Статус заказа успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadZakazyKlientamData(); // Refresh DataGrid
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка при обновлении статуса заказа: {updateResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при обновлении статуса заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка при обновлении статуса заказа: {ex}");
            }
        }

        // ManagerMainWindow.xaml.cs

        private async void PodrobneeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag is Guid zakazId)
            {
                // Открываем новое окно с товарами заказа
                ZakazDetailsWindow detailsWindow = new ZakazDetailsWindow(zakazId, GetTipZakaza());
                detailsWindow.Show();
            }
        }

        // Вспомогательный метод для получения типа заказа
        private string GetTipZakaza()
        {
            // Получаем выбранный RadioButton
            RadioButton selectedRadioButton = ControlZakazovPanel.Children.OfType<StackPanel>()
                    .FirstOrDefault()
                    ?.Children.OfType<RadioButton>()
                    .FirstOrDefault(r => r.IsChecked == true);

            // Возвращаем тип заказа или null, если RadioButton не выбран
            return selectedRadioButton?.Tag?.ToString();
        }
        private async void TipZakazaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.IsChecked == true)
            {
                string tipZakaza = radioButton.Tag.ToString();
                if (tipZakaza == "ЗаказыПоставщикам")
                {
                    await LoadZakazyPostavshikamData();
                }
                else if (tipZakaza == "ЗаказыКлиентам")
                {
                    await LoadZakazyKlientamData();
                }
            }
        }
        private void ApplyTovaryNaSkladeSearchFilter()
        {
            string searchText = PoiskTextBox.Text;

            if (string.IsNullOrEmpty(searchText))
            {
                // If the search text is empty, display all data
                TovaryNaSkladeDataGrid.ItemsSource = _tovaryNaSkladeData;
            }
            else
            {
                // If the search text is not empty, filter the data
                _filteredTovaryNaSkladeData = _tovaryNaSkladeData.Where(t =>
                    t.название_товара.ToLower().Contains(searchText.ToLower())
                ).ToList();
                TovaryNaSkladeDataGrid.ItemsSource = _filteredTovaryNaSkladeData;
            }
        }


    }

    public class ТоварыЗаказаПоставщикуViewModel
    {
        public Guid id_товара { get; set; } // add id_товара
        public string название_товара { get; set; }
        public int количество { get; set; }
    }

    public class ТоварыЗаказаКлиентуViewModel
    {
        public Guid id_товара { get; set; }
        public string название_товара { get; set; }
        public int количество { get; set; }
    }
    public class TovaryNaSkladeViewModel
    {
        public string название_товара { get; set; }
        public int количество { get; set; }
    }
    public class ЗаказПоставщикуViewModel
    {
        public Guid id { get; set; }
        public DateTime дата_заказа { get; set; }
        public DateTime? дата_ожидаемой_поставки { get; set; }
        public string название_поставщика { get; set; }
        public string название_склада { get; set; }
        public string статус { get; set; }
        public string примечания { get; set; }
    }
    public class ЗаказКлиентуViewModel
    {
        public Guid id { get; set; }
        public DateTime дата_заказа { get; set; }
        public DateTime? дата_ожидаемой_отгрузки { get; set; }
        public string название_клиента { get; set; }
        public string название_склада { get; set; }
        public string статус { get; set; }
        public string примечания { get; set; }
    }
}