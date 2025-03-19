using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using MSM.Models;
using MSM;

namespace MSM
{
    public partial class WarehousesPage : ContentPage
    {
        private ObservableCollection<Склады> _warehouses;

        public WarehousesPage()
        {
            InitializeComponent();
            _warehouses = new ObservableCollection<Склады>();
            BindingContext = _warehouses; // Устанавливаем BindingContext для привязки к CollectionView
            LoadWarehouses();
        }

        private async Task LoadWarehouses()
        {
            try
            {
                var response = await App.SupabaseClient
                    .From<Склады>()
                    .Select("*")
                    .Get();

                if (response?.Models != null)
                {
                    _warehouses.Clear();
                    foreach (var warehouse in response.Models)
                    {
                        _warehouses.Add(warehouse);
                    }
                    //WarehousesCollectionView.ItemsSource = _warehouses; //Больше не требуется, так как установили BindingContext
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load warehouses.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error loading warehouses: {ex.Message}", "OK");
            }
        }

        // Removed WarehousesCollectionView_SelectionChanged method
    }
}