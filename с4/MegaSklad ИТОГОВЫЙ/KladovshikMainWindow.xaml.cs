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
    public partial class KladovshikMainWindow : Window
    {
        private List<Поставщики> _postavshikiData;
        private List<Склады> _skladyData;
        private List<Клиенты> _klientiData;
        private List<ТоварыПриходнойНакладной> _tovaryVnakladnoy = new List<ТоварыПриходнойНакладной>();
        private List<ТоварыРасходнойНакладной> _tovaryRashVnakladnoy = new List<ТоварыРасходнойНакладной>();
        private decimal _obshayaSumma = 0;
        private decimal _rashObshayaSumma = 0;

        private Пользователи _userProfile;
        public Пользователи UserProfile
        {
            get { return _userProfile; }
            set
            {
                if (_userProfile != value) // Добавили проверку на изменение значения
                {
                    _userProfile = value;
                    OnPropertyChanged(); // Вызываем OnPropertyChanged при изменении UserProfile
                }
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

        public KladovshikMainWindow()
        {
            InitializeComponent();
            DataContext = this; // Важно!
            Loaded += KladovshikMainWindow_Loaded;
        }

        private async void KladovshikMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUserProfileData();
            await LoadPostavshikiData();
            await LoadSkladyData();
            await LoadKlientiData();
            UpdateObshayaSumma();
            UpdateTovaryDataGrid();
            UpdateRashObshayaSumma();
            UpdateTovaryRashDataGrid();

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
        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editProfileWindow = new EditProfileWindow(UserProfile);
            if (editProfileWindow.ShowDialog() == true)
            {
                LoadProfileImage(); // reloads the image

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                Console.WriteLine($"Ошибка при загрузке данных о поставщиках: {ex}");
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
                    RashSkladComboBox.ItemsSource = _skladyData;
                    RashSkladComboBox.DisplayMemberPath = "название_склада";
                    RashSkladComboBox.SelectedValuePath = "id";
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные о складах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных о складах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Ошибка при загрузке данных о складах: {ex}");
            }
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
                Console.WriteLine($"Ошибка при загрузке данных о клиентах: {ex}");
            }
        }

        //private Button profileButton;
        private StackPanel profilePanel;
        private System.Windows.Controls.Image profileImage;

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            string menuName = ((Button)sender).Tag.ToString();

            ProfilePanel.Visibility = Visibility.Collapsed;
            PrihodnayaNakladnayaPanel.Visibility = Visibility.Collapsed;
            RashodnayaNakladnayaPanel.Visibility = Visibility.Collapsed;
            InventarizatsiyaPanel.Visibility = Visibility.Collapsed;
            ShtrikhkodPanel.Visibility = Visibility.Collapsed;

            switch (menuName)
            {
                case "Профиль":
                    ProfilePanel.Visibility = Visibility.Visible;
                    LoadUserProfileData();
                    break;
                case "ПриходнаяНакладная":
                    PrihodnayaNakladnayaPanel.Visibility = Visibility.Visible;
                    break;
                case "РасходнаяНакладная":
                    RashodnayaNakladnayaPanel.Visibility = Visibility.Visible;
                    break;
                case "Инвентаризация":
                    InventarizatsiyaPanel.Visibility = Visibility.Visible;
                    LoadSkladyDataInvent();
                    break;
                case "Штрихкод":
                    ShtrikhkodPanel.Visibility = Visibility.Visible;
                    LoadTovarShtrihData();
                    break;
                default:
                    break;
            }
        }

        private async Task LoadTovarShtrihData()
        {
            try
            {
                var tovaryResponse = await App.SupabaseClient
                    .From<Models.Товары>()
                    .Select("*")
                    .Get();

                if (tovaryResponse.Models != null)
                {
                    ShtrihTovarComboBox.ItemsSource = tovaryResponse.Models.ToList();
                    ShtrihTovarComboBox.DisplayMemberPath = "название_товара";
                    ShtrihTovarComboBox.SelectedValuePath = "id";
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

        private void PrintQrCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShtrihTovarComboBox.SelectedItem is Товары selectedTovar && QrCodeImage.Source != null)
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Create FixedDocument
                    FixedDocument fixedDoc = new FixedDocument();
                    fixedDoc.DocumentPaginator.PageSize = new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                    // Create FixedPage
                    FixedPage fixedPage = new FixedPage();
                    fixedPage.Width = fixedDoc.DocumentPaginator.PageSize.Width;
                    fixedPage.Height = fixedDoc.DocumentPaginator.PageSize.Height;

                    // 1. Create TextBlock for the product name
                    TextBlock textBlockName = new TextBlock();
                    textBlockName.Text = selectedTovar.название_товара;
                    textBlockName.FontSize = 14;
                    textBlockName.FontWeight = FontWeights.Bold; // Set font to bold
                    textBlockName.TextWrapping = TextWrapping.Wrap;
                    textBlockName.Width = 400; // Set a fixed width for TextBlock
                    textBlockName.TextAlignment = TextAlignment.Center; // Center the text

                    FixedPage.SetLeft(textBlockName, (fixedPage.Width - textBlockName.Width) / 2);
                    FixedPage.SetTop(textBlockName, 10); // Smaller top margin

                    // 2. Create Image for printing
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = QrCodeImage.Source;
                    image.Width = 250; // Фиксированная ширина
                    image.Height = 250; // Фиксированная высота
                    FixedPage.SetLeft(image, (fixedPage.Width - image.Width) / 2); // Центрируем по горизонтали
                    FixedPage.SetTop(image, FixedPage.GetTop(textBlockName) + 25); // Отступ сверху

                    // 3. Create TextBlock for the product barcode
                    TextBlock textBlockBarcode = new TextBlock();
                    textBlockBarcode.Text = selectedTovar.артикул_товара; //Display id instead
                    textBlockBarcode.FontSize = 12; // Adjust font size
                    textBlockBarcode.TextWrapping = TextWrapping.Wrap;
                    textBlockBarcode.Width = 400; // Set a fixed width for TextBlock
                    textBlockBarcode.TextAlignment = TextAlignment.Center; // Center the text

                    FixedPage.SetLeft(textBlockBarcode, (fixedPage.Width - textBlockBarcode.Width) / 2); // Start from the image
                    FixedPage.SetTop(textBlockBarcode, FixedPage.GetTop(image) + image.Height + 5); // 10 pixels below the image

                    // Add image to the page
                    fixedPage.Children.Add(image);
                    fixedPage.Children.Add(textBlockName);
                    fixedPage.Children.Add(textBlockBarcode);
                    // Add the page to the document
                    fixedDoc.Pages.Add(new PageContent() { Child = fixedPage });

                    // Send to printer
                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Печать QR-кода");
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите товар и дождитесь загрузки QR-кода.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async void ShtrihTovarComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShtrihTovarComboBox.SelectedItem is Models.Товары selectedTovar)
            {
                // 1. Проверяем, есть ли штрихкод у товара
                if (!string.IsNullOrEmpty(selectedTovar.штрихкод_товара))
                {
                    // 2. Если штрихкод есть, загружаем и отображаем QR-код
                    try
                    {
                        string filePath = selectedTovar.штрихкод_товара;
                        Uri fileUri = new Uri(Task.Run(() => App.SupabaseClient.Storage.From("barcode").GetPublicUrl(filePath)).Result, UriKind.Absolute); //await SupabaseService.Storage.GetPublicUrl(filePath); //new Uri(baseurl + '/' + filePath, UriKind.Absolute);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = fileUri; //new Uri(filePath, UriKind.Absolute);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        QrCodeImage.Source = bitmapImage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке QR-кода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        QrCodeImage.Source = null;
                    }
                }
                else
                {
                    // 3. Если штрихкода нет, очищаем изображение
                    QrCodeImage.Source = null;
                }
            }
            else
            {
                QrCodeImage.Source = null;
            }
        }
        //Метод для генерации и сохранения QR-кода
        private async void GenerateAndSaveQrCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShtrihTovarComboBox.SelectedItem is Товары selectedTovar)
            {
                try
                {
                    // 1. Prepare QR code data
                    string tovarInfo = $"ID: {selectedTovar.id}\nНазвание: {selectedTovar.название_товара}\nАртикул: {selectedTovar.артикул_товара}";

                    // 2. Generate the QR code
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(tovarInfo, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    // 3. Convert Bitmap to byte array
                    byte[] imageBytes;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        qrCodeImage.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;
                        imageBytes = memoryStream.ToArray();
                    }

                    // 4. Define image properties
                    string filename = $"qr-codes/{selectedTovar.id}.png";
                    string bucketId = "barcode";

                    // 5. Upload the file to Supabase Storage
                    var bucket = App.SupabaseClient.Storage.From(bucketId);
                    var response = await bucket.Upload(imageBytes, filename, new Supabase.Storage.FileOptions { ContentType = "image/png", Upsert = true });

                    if (response != null)
                    {
                        //  string publicUrl = response;
                        // 6. Update the database with the public URL
                        var updateResponse = await App.SupabaseClient
                           .From<Models.Товары>()
                           .Where(x => x.id == selectedTovar.id)
                           .Set(x => x.штрихкод_товара, filename) // or publicUrl
                           .Update();

                        if (updateResponse != null && updateResponse.ResponseMessage != null && updateResponse.ResponseMessage.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"QR-код успешно сгенерирован и сохранен: {filename}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadTovarShtrihData();

                            try
                            {
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.UriSource = new Uri(Task.Run(() => App.SupabaseClient.Storage.From("barcode").GetPublicUrl(filename)).Result, UriKind.Absolute);

                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();

                                QrCodeImage.Source = bitmapImage;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка при загрузке QR-кода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                QrCodeImage.Source = null;
                            }

                            QrCodeImage.Source = null;
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка при обновлении данных о товаре: {updateResponse?.ResponseMessage?.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить QR-код в хранилище.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Произошла ошибка: {ex}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private async void SaveQrCodeToStorageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShtrihTovarComboBox.SelectedItem is Товары selectedTovar && QrCodeImage.Source is BitmapImage bitmapImage)
            {
                try
                {
                    // 1. Prepare the image for upload
                    byte[] imageBytes;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        encoder.Save(memoryStream);
                        memoryStream.Position = 0;
                        imageBytes = memoryStream.ToArray();
                    }

                    // 2. Set filename
                    string filename = $"qr-codes/{selectedTovar.id}.png"; // unique name, with folder
                    string bucketId = "barcode"; // Имя вашего бакета

                    // 3. Initialize Supabase Storage Bucket
                    var bucket = App.SupabaseClient.Storage.From(bucketId);

                    // 4. Upload the file
                    var response = await bucket.Upload(imageBytes, filename, new Supabase.Storage.FileOptions { ContentType = "image/png", Upsert = true });

                    // 5. Handle the response
                    if (response != null)
                    {
                        MessageBox.Show($"QR-код успешно сохранен в хранилище как {filename}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        var updateResponse = await App.SupabaseClient
                           .From<Models.Товары>()
                           .Where(x => x.id == selectedTovar.id)
                           .Set(x => x.штрихкод_товара, filename)
                           .Update();

                        if (updateResponse != null && updateResponse.ResponseMessage != null && !updateResponse.ResponseMessage.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Ошибка при обновлении клиента: {updateResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить QR-код в хранилище.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении QR-кода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Произошла ошибка при сохранении QR-кода: {ex}");
                }
            }
        }

        //Приходная
        private async void AddTovarButton_Click(object sender, RoutedEventArgs e)
        {
            var addTovarWindow = new AddTovarToNakladnayaWindow();
            if (addTovarWindow.ShowDialog() == true)
            {
                if (addTovarWindow.NewTovar != null)
                {
                    _tovaryVnakladnoy.Add(addTovarWindow.NewTovar);
                    UpdateObshayaSumma();
                    UpdateTovaryDataGrid();
                }
            }
        }

        private void UpdateObshayaSumma()
        {
            _obshayaSumma = _tovaryVnakladnoy.Sum(t => t.количество * t.цена);
            ObshayaSummaTextBlock.Text = _obshayaSumma.ToString("0.00");
        }

        private void UpdateTovaryDataGrid()
        {
            TovaryDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn
            {
                Header = "Название товара",
                Binding = new Binding("название_товара")
            };
            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn { Header = "Количество", Binding = new Binding("количество") };
            DataGridTextColumn cenaColumn = new DataGridTextColumn { Header = "Цена", Binding = new Binding("цена") };
            DataGridTextColumn summaColumn = new DataGridTextColumn { Header = "Сумма", Binding = new Binding(".") { ConverterParameter = "Приход", Converter = new SummaConverter() } };

            TovaryDataGrid.Columns.Add(nazvanieColumn);
            TovaryDataGrid.Columns.Add(kolichestvoColumn);
            TovaryDataGrid.Columns.Add(cenaColumn);
            TovaryDataGrid.Columns.Add(summaColumn);

            TovaryDataGrid.ItemsSource = null;
            TovaryDataGrid.ItemsSource = _tovaryVnakladnoy;
        }

        //Расходная
        private async void AddTovarRashButton_Click(object sender, RoutedEventArgs e)
        {
            if (RashSkladComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Guid.TryParse(RashSkladComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addTovarWindow = new AddTovarToRashodnayaNakladnayaWindow(idSklada); // Pass sklad ID
            if (addTovarWindow.ShowDialog() == true)
            {
                if (addTovarWindow.NewTovar != null)
                {
                    _tovaryRashVnakladnoy.Add(addTovarWindow.NewTovar);
                    UpdateRashObshayaSumma();
                    UpdateTovaryRashDataGrid();
                }
            }
        }
        private void UpdateRashObshayaSumma()
        {
            _rashObshayaSumma = _tovaryRashVnakladnoy.Sum(t => t.количество * t.цена);
            RashObshayaSummaTextBlock.Text = _rashObshayaSumma.ToString("0.00");
        }

        private void UpdateTovaryRashDataGrid()
        {
            TovaryRashDataGrid.Columns.Clear();

            DataGridTextColumn nazvanieColumn = new DataGridTextColumn
            {
                Header = "Название товара",
                Binding = new Binding("название_товара")
            };
            DataGridTextColumn kolichestvoColumn = new DataGridTextColumn { Header = "Количество", Binding = new Binding("количество") };
            DataGridTextColumn cenaColumn = new DataGridTextColumn { Header = "Цена", Binding = new Binding("цена") };
            DataGridTextColumn summaColumn = new DataGridTextColumn { Header = "Сумма", Binding = new Binding(".") { ConverterParameter = "Расход", Converter = new SummaConverter() } };

            TovaryRashDataGrid.Columns.Add(nazvanieColumn);
            TovaryRashDataGrid.Columns.Add(kolichestvoColumn);
            TovaryRashDataGrid.Columns.Add(cenaColumn);
            TovaryRashDataGrid.Columns.Add(summaColumn);

            TovaryRashDataGrid.ItemsSource = null;
            TovaryRashDataGrid.ItemsSource = _tovaryRashVnakladnoy;
        }

        //Сохранение приходной накладной
        private async void SaveNakladnayaButton_Click(object sender, RoutedEventArgs e)
        {
            if (PostavshikComboBox.SelectedItem == null || SkladComboBox.SelectedItem == null || DataPostupleniyaDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

            try
            {
                DateTime dataPostupleniya = DataPostupleniyaDatePicker.SelectedDate.Value; // Убираем время

                ПриходныеНакладные newNakladnaya = new ПриходныеНакладные
                {
                    id_поставщика = idPostavshika,
                    id_склада = idSklada,
                    дата_поступления = dataPostupleniya,
                    общая_сумма = _obshayaSumma,
                    примечания = PrimechanieTextBox.Text
                };

                var insertResponse = await App.SupabaseClient
                    .From<Models.ПриходныеНакладные>()
                    .Insert(newNakladnaya);

                if (insertResponse != null && insertResponse.Models != null && insertResponse.Models.Any())
                {
                    Guid nakladnayaId = insertResponse.Models.First().id;

                    foreach (var tovarVnakladnoy in _tovaryVnakladnoy)
                    {
                        ТоварыПриходнойНакладной2 newTovarPrihodDTO = new ТоварыПриходнойНакладной2
                        {
                            id_приходной_накладной = nakladnayaId,
                            id_товара = tovarVnakladnoy.id_товара,
                            количество = tovarVnakladnoy.количество,
                            цена = tovarVnakladnoy.цена,
                        };

                        var insertTovarResponse = await App.SupabaseClient
                            .From<Models.ТоварыПриходнойНакладной2>()
                            .Insert(newTovarPrihodDTO);

                        // Сохраняем остатки товара
                        Console.WriteLine($"Начинаем обновление остатков. idSklada: {idSklada}, idТовара: {tovarVnakladnoy.id_товара}, количество: {tovarVnakladnoy.количество}, dataPostupleniya: {dataPostupleniya}");
                        await AddNewОстаткиТоваров(idSklada, tovarVnakladnoy.id_товара, tovarVnakladnoy.количество, dataPostupleniya);
                    }

                    MessageBox.Show("Приходная накладная успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show($"Ошибка при создании накладной: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Ошибка при создании накладной: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }

        //Сохранение расходной накладной
        private async void SaveRashNakladnayaButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на заполненность обязательных полей
            if (KlientComboBox.SelectedItem == null || RashSkladComboBox.SelectedItem == null || DataOtgruzkiDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение id_клиента (исправлено!)
            if (KlientComboBox.SelectedValue == null || !Guid.TryParse(KlientComboBox.SelectedValue.ToString(), out Guid idKlienta))
            {
                MessageBox.Show("Некорректный формат данных в поле Клиент.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Получение id_склада
            if (RashSkladComboBox.SelectedValue == null || !Guid.TryParse(RashSkladComboBox.SelectedValue.ToString(), out Guid idSklada))
            {
                MessageBox.Show("Некорректный формат данных в поле Склад.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dataOtgruzki = DataOtgruzkiDatePicker.SelectedDate.Value;

            // 4. Создание объекта РасходныеНакладные
            РасходныеНакладные newNakladnaya = new РасходныеНакладные
            {
                id = Guid.NewGuid(), // Generate a new Guid for the invoice
                id_клиента = idKlienta,
                id_склада = idSklada,
                дата_отгрузки = dataOtgruzki,
                общая_сумма = _rashObshayaSumma,
                примечания = RashPrimechanieTextBox.Text
            };

            // 5. Вставка данных в таблицу Расходные_накладные
            try
            {
                var insertResponse = await App.SupabaseClient
                    .From<Models.РасходныеНакладные>()
                    .Insert(newNakladnaya);

                if (insertResponse != null && insertResponse.Models != null && insertResponse.Models.Any())
                {
                    Guid nakladnayaId = insertResponse.Models.First().id;

                    // 6. Сохранение товаров в таблицу Товары_расходной_накладной
                    foreach (var tovarVnakladnoy in _tovaryRashVnakladnoy)
                    {
                        ТоварыРасходнойНакладной2 newTovarRashDTO = new ТоварыРасходнойНакладной2
                        {
                            id = Guid.NewGuid(), // Generate a new Guid for the item
                            id_расходной_накладной = nakladnayaId,
                            id_товара = tovarVnakladnoy.id_товара,
                            количество = tovarVnakladnoy.количество,
                            цена = tovarVnakladnoy.цена,
                        };

                        var insertTovarResponse = await App.SupabaseClient
                            .From<Models.ТоварыРасходнойНакладной2>()
                            .Insert(newTovarRashDTO);

                        if (insertTovarResponse?.ResponseMessage?.IsSuccessStatusCode == false)
                        {
                            MessageBox.Show($"Ошибка при создании товаров накладной: {insertTovarResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine($"Ошибка при создании товаров накладной: {insertTovarResponse.ResponseMessage.ReasonPhrase}");
                        }
                    }

                    MessageBox.Show("Расходная накладная успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearRashForm();
                }
                else
                {
                    MessageBox.Show($"Ошибка при создании накладной: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Ошибка при создании накладной: {insertResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}");
                }
            }
            catch (Exception ex)
            {
                //ВНИМАНИЕ: Ошибка, выброшенная триггером, будет здесь!
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Произошла ошибка: {ex}");
            }
        }

        //Очистка приходной формы
        private void ClearForm()
        {
            PostavshikComboBox.SelectedItem = null;
            SkladComboBox.SelectedItem = null;
            DataPostupleniyaDatePicker.SelectedDate = null;
            PrimechanieTextBox.Text = string.Empty;
            _tovaryVnakladnoy.Clear();
            UpdateObshayaSumma();
            UpdateTovaryDataGrid();
        }

        //Очистка расходной формы
        private void ClearRashForm()
        {
            KlientComboBox.SelectedItem = null;
            RashSkladComboBox.SelectedItem = null;
            DataOtgruzkiDatePicker.SelectedDate = null;
            RashPrimechanieTextBox.Text = string.Empty;
            _tovaryRashVnakladnoy.Clear();
            UpdateRashObshayaSumma();
            UpdateTovaryRashDataGrid();
        }

        private async Task AddNewОстаткиТоваров(Guid idSklada, Guid idТовара, int количество, DateTime dataPostupleniya)
        {
            try
            {
                Console.WriteLine($"AddNewОстаткиТоваров: idSklada = {idSklada}, idТовара = {idТовара}, количество = {количество}, dataPostupleniya = {dataPostupleniya}");

                // Всегда добавляем новую запись
                ОстаткиТоваров newOstatok = new ОстаткиТоваров
                {
                    id_склада = idSklada,
                    id_товара = idТовара,
                    количество = количество,
                    дата_обновления = dataPostupleniya
                };

                Console.WriteLine($"Создаем новую запись в Остатки_товаров: idSklada = {idSklada}, idТовара = {idТовара}, количество = {количество}, data_обновления = {dataPostupleniya}");

                var insertResponse = await App.SupabaseClient
                    .From<Models.ОстаткиТоваров>()
                    .Insert(newOstatok);

                Console.WriteLine($"Insert запрос выполнен. Status Code: {insertResponse?.ResponseMessage?.StatusCode}");

                if (insertResponse?.ResponseMessage?.IsSuccessStatusCode == false)
                {
                    MessageBox.Show($"Ошибка при создании остатка: {insertResponse.ResponseMessage.ReasonPhrase}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"Ошибка при создании остатка: {insertResponse.ResponseMessage.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании остатков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Ошибка при создании остатков: {ex}");
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
                Console.WriteLine($"Ошибка при загрузке данных о складах: {ex}");
            }
        }



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
                        prihodNaladnie.Any(y => y.id == x.id_приходной_накладной && y.id_склада == idSklada && y.дата_поступления >= dataNachala && y.дата_поступления <= dataOkonchaniya)
                        && x.id_товара == idTovara
                    ).Sum(x => x.количество);

                    var rashodZaPeriod = tovaryRashod.Where(x =>
                         rashodNaladnie.Any(y => y.id == x.id_расходной_накладной && y.id_склада == idSklada && y.дата_отгрузки >= dataNachala && y.дата_отгрузки <= dataOkonchaniya)
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
                        примечания = "", // По умолчанию
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

        private async void SaveInventarizatsiyaButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на заполненность обязательных полей
            if (InventSkladComboBox.SelectedItem == null || InventDataNachalaDatePicker.SelectedDate == null || InventDataOkonchaniyaDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Получение id_склада
            if (InventSkladComboBox.SelectedValue == null || !Guid.TryParse(InventSkladComboBox.SelectedValue.ToString(), out Guid idSklada))
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

        // ViewModel для отображения данных в DataGrid
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

    public class SummaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ТоварыПриходнойНакладной tovarPrihod && parameter as string == "Приход")
            {
                return tovarPrihod.количество * tovarPrihod.цена;
            }
            else if (value is ТоварыРасходнойНакладной tovarRashod && parameter as string == "Расход")
            {
                return tovarRashod.количество * tovarRashod.цена;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}