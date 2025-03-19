using System;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;
using System.Linq;
using MSM.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Essentials;
using ZXing.Mobile;

namespace MSM
{
    public partial class ScanPage : ContentPage
    {
        private double _scannerSize;

        public ScanPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Включаем анализ при появлении страницы
            scannerView.IsAnalyzing = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Выключаем анализ при исчезновении страницы
            scannerView.IsAnalyzing = false;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // Вычисляем минимальный размер (ширина или высота)
            _scannerSize = Math.Min(width, height) * 0.8; // Занимаем 80% от минимального размера

            // Устанавливаем размеры сканера
            scannerView.WidthRequest = _scannerSize;
            scannerView.HeightRequest = _scannerSize;
        }


        private void ScannerView_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    // Останавливаем анализ сразу после получения результата (но не сканирование)
                    scannerView.IsAnalyzing = false;

                    // Show the result in a popup message
                    await DisplayAlert("Результат сканирования", result.Text, "OK");

                    // Включаем анализ снова после отображения результата
                    scannerView.IsAnalyzing = true;
                }
                else
                {
                    scanResultText.Text = "Не удалось отсканировать код.";
                }
            });
        }
    }
}