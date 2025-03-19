using System;
using System.Windows;
using System.Windows.Controls;

namespace MegaSklad
{
    public partial class EditZakazStatusWindow : Window
    {
        public string NewStatus { get; set; }

        public EditZakazStatusWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            NewStatus = ((ComboBoxItem)StatusComboBox.SelectedItem).Content.ToString();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}