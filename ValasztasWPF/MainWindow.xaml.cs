using Microsoft.Win32;
using System.Windows;

namespace ValasztasWPF
{
    public partial class MainWindow : Window
    {
        private ValasztasRepository? _repository;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var connectionString = ConnectionStringProvider.GetConnectionString();
                _repository = new ValasztasRepository(connectionString);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Nem sikerült csatlakozni az adatbázishoz:\n{ex.Message}",
                    "Csatlakozási hiba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Adatbázis kapcsolat sikertelen.";
            }
        }

        private async Task LoadDataAsync()
        {
            if (_repository == null) return;

            StatusText.Text = "Adatok betöltése...";
            try
            {
                var data = await _repository.GetAllAsync();
                EredmenyGrid.ItemsSource = data;
                StatusText.Text = $"{data.Count} sor betöltve.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Hiba az adatok betöltésekor:\n{ex.Message}",
                    "Hiba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Hiba az adatok betöltésekor.";
            }
        }

        private async void MenuMegnyitas_Click(object sender, RoutedEventArgs e)
        {
            if (_repository == null)
            {
                MessageBox.Show("Nincs aktív adatbázis kapcsolat.", "Hiba",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Title = "Választási adatfájl megnyitása",
                Filter = "Szövegfájl (*.txt)|*.txt|Minden fájl (*.*)|*.*",
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() != true) return;

            StatusText.Text = "Fájl feldolgozása...";
            try
            {
                var importData = ImportParser.Parse(dialog.FileName);
                await _repository.ImportAsync(importData);
                await LoadDataAsync();
                MessageBox.Show("Az adatok sikeresen importálva.", "Siker",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Hiba az import során:\n{ex.Message}",
                    "Import hiba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Import sikertelen.";
            }
        }

        private void MenuKilepes_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
