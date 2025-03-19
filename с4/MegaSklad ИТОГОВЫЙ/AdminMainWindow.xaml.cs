using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MegaSklad.Models;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace MegaSklad
{
    public partial class AdminMainWindow : Window, INotifyPropertyChanged
    {
        private List<Склады> _skladyData = new List<Склады>();
        private List<Товары> _tovaryData = new List<Товары>();
        private List<Категории> _kategoriiData = new List<Категории>();
        private List<Клиенты> _klientiData = new List<Клиенты>();
        private List<Поставщики> _postavshikiData = new List<Поставщики>();
        private List<Пользователи> _usersData = new List<Пользователи>();
        private string _currentDataType;
        private bool _isLoading = false;

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

        public AdminMainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += AdminMainWindow_Loaded;
        }

        private async void AdminMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUserProfileData();
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
        private async void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string menuName = clickedButton.Tag.ToString();

            // Скрываем все панели и DataGrid
            ProfilePanel.Visibility = Visibility.Collapsed;
            ContentDataGrid.Visibility = Visibility.Collapsed;
            DataGridButtonsPanel.Visibility = Visibility.Collapsed;
            UsersDataGrid.Visibility = Visibility.Collapsed;
            UsersDataGridButtonsPanel.Visibility = Visibility.Collapsed;

            switch (menuName)
            {
                case "Профиль":
                    ProfilePanel.Visibility = Visibility.Visible;
                    await LoadUserProfileData();
                    break;

                case "Склады":
                    await LoadSkladyData();
                    ContentDataGrid.Visibility = Visibility.Visible;
                    DataGridButtonsPanel.Visibility = Visibility.Visible;
                    break;

                case "Товары":
                    await LoadTovaryData();
                    ContentDataGrid.Visibility = Visibility.Visible;
                    DataGridButtonsPanel.Visibility = Visibility.Visible;
                    break;

                case "Клиенты":
                    await LoadKlientiData();
                    ContentDataGrid.Visibility = Visibility.Visible;
                    DataGridButtonsPanel.Visibility = Visibility.Visible;
                    break;

                case "Поставщики":
                    await LoadPostavshikiData();
                    ContentDataGrid.Visibility = Visibility.Visible;
                    DataGridButtonsPanel.Visibility = Visibility.Visible;
                    break;

                case "Пользователи":
                    UsersDataGrid.Visibility = Visibility.Visible;
                    UsersDataGridButtonsPanel.Visibility = Visibility.Visible;
                    await LoadUsersData();
                    break;

                default:
                    ClearDataGrid();
                    break;
            }

            _currentDataType = menuName; // Update current data type
        }

        private async Task LoadUserProfileData()
        {
            try
            {
                var userId = App.SupabaseClient.Auth.CurrentUser.Id;

                var userResponse = await App.SupabaseClient
                    .From<Models.Пользователи>()
                    .Where(x => x.supabase_auth_id == userId)
                    .Select("*")
                    .Get();

                if (userResponse.Models != null && userResponse.Models.Any())
                {
                    UserProfile = userResponse.Models.FirstOrDefault();
                    if (UserProfile.фото == null)
                    {
                        ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                        return;
                    }

                    string bucketName = "photoprofil";
                    string imageName = UserProfile.фото.ToString() + ".png";

                    string fullImageUrl = await Task.Run(() => App.SupabaseClient.Storage.From(bucketName).GetPublicUrl(imageName));

                    if (string.IsNullOrEmpty(fullImageUrl))
                    {
                        ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                        return;
                    }

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(fullImageUrl);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    ProfileImage.Source = bitmapImage;
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить информацию о пользователе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                    ClearDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке информации о пользователе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/user.png"));
                ClearDataGrid();
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow(UserProfile);
            if (editProfileWindow.ShowDialog() == true)
            {
                LoadUserProfileData();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task LoadSkladyData()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                var skladyResponse = await App.SupabaseClient
                    .From<Models.Склады>()
                    .Select("*")
                    .Get();

                if (skladyResponse.Models != null)
                {
                    _skladyData = skladyResponse.Models.ToList();
                    UpdateDataGrid(_skladyData, "Склады");
                    _currentDataType = "Склады";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ContentDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о складах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ContentDataGrid.ItemsSource = null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadTovaryData()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                await LoadKategoriiData();

                var tovaryResponse = await App.SupabaseClient
                    .From<Models.Товары>()
                    .Select("*")
                    .Get();

                if (tovaryResponse.Models != null)
                {
                    _tovaryData = tovaryResponse.Models.ToList();
                    UpdateDataGrid(_tovaryData, "Товары");
                    _currentDataType = "Товары";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о товарах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ContentDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о товарах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ContentDataGrid.ItemsSource = null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadKlientiData()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                var klientiResponse = await App.SupabaseClient
                    .From<Models.Клиенты>()
                    .Select("*")
                    .Get();

                if (klientiResponse.Models != null)
                {
                    _klientiData = klientiResponse.Models.ToList();
                    UpdateDataGrid(_klientiData, "Клиенты");
                    _currentDataType = "Клиенты";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о клиентах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ContentDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о клиентах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ContentDataGrid.ItemsSource = null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadPostavshikiData()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                var postavshikiResponse = await App.SupabaseClient
                    .From<Models.Поставщики>()
                    .Select("*")
                    .Get();

                if (postavshikiResponse.Models != null)
                {
                    _postavshikiData = postavshikiResponse.Models.ToList();
                    UpdateDataGrid(_postavshikiData, "Поставщики");
                    _currentDataType = "Поставщики";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о поставщиках.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ContentDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о поставщиках: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ContentDataGrid.ItemsSource = null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadKategoriiData()
        {
            try
            {
                var kategoriiResponse = await App.SupabaseClient
                    .From<Models.Категории>()
                    .Select("*")
                    .Get();

                if (kategoriiResponse.Models != null)
                {
                    _kategoriiData = kategoriiResponse.Models.ToList();
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о категориях.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о категориях: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDataGrid<T>(List<T> data, string dataType)
        {
            ContentDataGrid.Columns.Clear();

            if (dataType == "Склады")
            {
                AddColumn("Название склада", "название_склада");
                AddColumn("Адрес склада", "адрес_склада");
                AddColumn("Тип склада", "тип_склада");
            }
            else if (dataType == "Товары")
            {
                AddColumn("Название товара", "название_товара");
                AddColumn("Артикул товара", "артикул_товара");
                AddColumn("Цена", "цена");
                AddColumn("Минимальный остаток", "минимальный_остаток");
                AddColumn("Единица измерения", "единица_измерения");
                DataGridTextColumn категорияColumn = new DataGridTextColumn();
                категорияColumn.Header = "Категория";
                категорияColumn.Binding = new Binding("id_категории")
                {
                    Converter = new CategoryIdToNameConverter(_kategoriiData)
                };
                ContentDataGrid.Columns.Add(категорияColumn);
                AddColumn("Описание", "описание");
            }
            else if (dataType == "Клиенты")
            {
                AddColumn("Название клиента", "название_клиента");
                AddColumn("Контактное лицо", "контактное_лицо");
                AddColumn("Телефон", "телефон_клиента");
                AddColumn("Email", "email_клиента");
                AddColumn("Адрес", "адрес_клиента");
            }
            else if (dataType == "Поставщики")
            {
                AddColumn("Название поставщика", "название_поставщика");
                AddColumn("ИНН", "ИНН_поставщика");
                AddColumn("КПП", "КПП_поставщика");
                AddColumn("Контактное лицо", "контактное_лицо");
                AddColumn("Телефон", "телефон_поставщика");
                AddColumn("Email", "email_поставщика");
                AddColumn("Адрес", "адрес_поставщика");
            }

            ContentDataGrid.ItemsSource = data;
        }

        private void AddColumn(string header, string binding)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = header;
            column.Binding = new Binding(binding);
            ContentDataGrid.Columns.Add(column);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDataType == "Склады")
            {
                Склады newSklad = new Склады();
                EditSkladWindow addWindow = new EditSkladWindow(newSklad, true);
                if (addWindow.ShowDialog() == true)
                {
                    await LoadSkladyData();
                }
            }
            else if (_currentDataType == "Товары")
            {
                Товары newTovar = new Товары();
                EditTovarWindow addWindow = new EditTovarWindow(newTovar, true, _kategoriiData);
                if (addWindow.ShowDialog() == true)
                {
                    await LoadTovaryData();
                }
            }
            else if (_currentDataType == "Клиенты")
            {
                Клиенты newKlient = new Клиенты();
                EditClientWindow addWindow = new EditClientWindow(newKlient, true);
                if (addWindow.ShowDialog() == true)
                {
                    await LoadKlientiData();
                }
            }
            else if (_currentDataType == "Поставщики")
            {
                Поставщики newPostavshik = new Поставщики();
                EditPostavshikWindow addWindow = new EditPostavshikWindow(newPostavshik, true);
                if (addWindow.ShowDialog() == true)
                {
                    await LoadPostavshikiData();
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDataType == "Склады")
            {
                if (ContentDataGrid.SelectedItem is Models.Склады selectedSklad)
                {
                    EditSkladWindow editWindow = new EditSkladWindow(selectedSklad);
                    if (editWindow.ShowDialog() == true)
                    {
                        await LoadSkladyData();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите склад для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentDataType == "Товары")
            {
                if (ContentDataGrid.SelectedItem is Models.Товары selectedTovar)
                {
                    EditTovarWindow editWindow = new EditTovarWindow(selectedTovar, _kategoriiData);
                    if (editWindow.ShowDialog() == true)
                    {
                        await LoadTovaryData();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите товар для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentDataType == "Клиенты")
            {
                if (ContentDataGrid.SelectedItem is Models.Клиенты selectedKlient)
                {
                    EditClientWindow editWindow = new EditClientWindow(selectedKlient);
                    if (editWindow.ShowDialog() == true)
                    {
                        await LoadKlientiData();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите клиента для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentDataType == "Поставщики")
            {
                if (ContentDataGrid.SelectedItem is Models.Поставщики selectedPostavshik)
                {
                    EditPostavshikWindow editWindow = new EditPostavshikWindow(selectedPostavshik);
                    if (editWindow.ShowDialog() == true)
                    {
                        await LoadPostavshikiData();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите поставщика для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDataType == "Склады")
            {
                if (ContentDataGrid.SelectedItem is Models.Склады selectedSklad)
                {
                    MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить склад '{selectedSklad.название_склада}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            await App.SupabaseClient
                                 .From<Models.Склады>()
                                 .Where(x => x.id == selectedSklad.id)
                                 .Delete();

                            MessageBox.Show("Склад успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            await LoadSkladyData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите склад для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentDataType == "Товары")
            {
                if (ContentDataGrid.SelectedItem is Models.Товары selectedTovar)
                {
                    MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selectedTovar.название_товара}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            await App.SupabaseClient
                                 .From<Models.Товары>()
                                 .Where(x => x.id == selectedTovar.id)
                                 .Delete();

                            MessageBox.Show("Товар успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            await LoadTovaryData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите товар для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentDataType == "Клиенты")
            {
                if (ContentDataGrid.SelectedItem is Models.Клиенты selectedKlient)
                {
                    MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить клиента '{selectedKlient.название_клиента}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            await App.SupabaseClient
                                 .From<Models.Клиенты>()
                                 .Where(x => x.id == selectedKlient.id)
                                 .Delete();

                            MessageBox.Show("Клиент успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            await LoadKlientiData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите клиента для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (_currentDataType == "Поставщики")
            {
                if (ContentDataGrid.SelectedItem is Models.Поставщики selectedPostavshik)
                {
                    MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить поставщика '{selectedPostavshik.название_поставщика}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            await App.SupabaseClient
                                 .From<Models.Поставщики>()
                                 .Where(x => x.id == selectedPostavshik.id)
                                 .Delete();

                            MessageBox.Show("Поставщик успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            await LoadPostavshikiData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите поставщика для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Очищает DataGrid
        private void ClearDataGrid()
        {
            ContentDataGrid.ItemsSource = null;
            ContentDataGrid.Items.Clear();
            ContentDataGrid.Columns.Clear();
        }

        // Очищает столбцы DataGrid
        private void ClearDataGridColumns()
        {
            ContentDataGrid.Columns.Clear();
        }

        private async Task LoadUsersData()
        {
            try
            {
                var usersResponse = await App.SupabaseClient
                    .From<Models.Пользователи>()
                    .Select("*")
                    .Get();

                if (usersResponse.Models != null)
                {
                    _usersData = usersResponse.Models.ToList();
                    UpdateUsersDataGrid(_usersData); // Обновляем DataGrid
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о пользователях.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    UsersDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о пользователях: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                UsersDataGrid.ItemsSource = null;
            }
        }

        // Обновление DataGrid для отображения пользователей
        private void UpdateUsersDataGrid(List<Пользователи> data)
        {
            UsersDataGrid.Columns.Clear();
            AddUserColumn("Имя пользователя", "имя_пользователя");
            AddUserColumn("Email", "email");
            AddUserColumn("Телефон", "телефон");
            AddUserColumn("Роль", "роль");
            UsersDataGrid.ItemsSource = data;
        }

        private void AddUserColumn(string header, string binding)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = header;
            column.Binding = new Binding(binding);
            UsersDataGrid.Columns.Add(column);
        }

        // Обработчики нажатия на кнопки "Добавить", "Редактировать", "Удалить"
        private async void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            Пользователи newUser = new Пользователи();
            EditUserWindow addUserWindow = new EditUserWindow(newUser, true); // true для режима "добавить"
            if (addUserWindow.ShowDialog() == true)
            {
                await LoadUsersData(); // Refresh the user list
            }
        }
        private async void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is Models.Пользователи selectedUser)
            {
                EditUserWindow editUserWindow = new EditUserWindow(selectedUser);
                if (editUserWindow.ShowDialog() == true)
                {
                    await LoadUsersData();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is Models.Пользователи selectedUser)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя '{selectedUser.имя_пользователя}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await App.SupabaseClient
                             .From<Models.Пользователи>()
                             .Where(x => x.id == selectedUser.id)
                             .Delete();

                        MessageBox.Show("Пользователь успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        await LoadUsersData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    public class CategoryIdToNameConverter : IValueConverter
    {
        private List<Категории> _kategoriiData;

        public CategoryIdToNameConverter(List<Категории> kategoriiData)
        {
            _kategoriiData = kategoriiData;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Guid categoryId)
            {
                Категории category = _kategoriiData?.FirstOrDefault(k => k.id == categoryId);
                return category?.название_категории ?? "Неизвестная категория";
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}