using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Supabase;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

namespace NagryzkaTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string supabaseUrl = UrlTextBox.Text;
            string supabaseKey = KeyTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;
            int numberOfRuns;

            if (!int.TryParse(RunsTextBox.Text, out numberOfRuns) || numberOfRuns <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное положительное число для количества прогонов.");
                return;
            }

            ResultsTextBox.Clear();

            await RunLoadTest(supabaseUrl, supabaseKey, email, password, numberOfRuns);
        }

        private async Task RunLoadTest(string supabaseUrl, string supabaseKey, string email, string password, int numberOfRuns)
        {
            var results = new ConcurrentBag<string>();
            var stopwatch = Stopwatch.StartNew();

            var tasks = new List<Task>();

            for (int i = 1; i <= numberOfRuns; i++)
            {
                int runNumber = i;

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var client = new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions
                        {
                            AutoRefreshToken = false,
                        });
                        await client.InitializeAsync();

                        var start = Stopwatch.StartNew();
                        var authResponse = await client.Auth.SignIn(email, password);
                        start.Stop();

                        if (authResponse?.User != null)
                        {
                            results.Add($"Прогон {runNumber}: Успешно за {start.ElapsedMilliseconds} мс, ID пользователя: {authResponse.User.Id}");
                        }
                        else
                        {
                            results.Add($"Прогон {runNumber}: Неудачно за {start.ElapsedMilliseconds} мс, Ошибка: Неизвестно (проверьте исключения)");
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"Прогон {runNumber}: Исключение - {ex.Message}");
                    }
                    finally
                    {
                    }
                }));
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var result in results.OrderBy(r => r))
                {
                    ResultsTextBox.AppendText(result + Environment.NewLine);
                }
                ResultsTextBox.AppendText($"\nОбщее время: {stopwatch.ElapsedMilliseconds} мс для {numberOfRuns} прогонов.");
            });
        }
    }
}