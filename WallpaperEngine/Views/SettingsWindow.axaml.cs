using Avalonia.Controls;
using Avalonia.Interactivity;
using WallpaperEngine.ViewModels;

namespace WallpaperEngine.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly MainWindowViewModel? _mainVm;

        public SettingsWindow()
        {
            InitializeComponent();
        }
        public SettingsWindow(MainWindowViewModel? mainVm = null)
        {
            InitializeComponent();
            _mainVm = mainVm;
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            string newPath = PathTextBox.Text ?? "";
            int newInterval = (int)(IntervalNumeric.Value ?? 5m);

            if (string.IsNullOrWhiteSpace(newPath))
            {
                // Ha üres, csak az intervalt frissítjük, a mappát nem változtatjuk
                _mainVm?.RefreshIntervalOnly(newInterval);
                this.Close();
                return;
            }

            if (!System.IO.Directory.Exists(newPath))
            {
                // Hibás mappa — ne csináljunk semmit, adjunk visszajelzést
                PathTextBox.Classes.Add("error"); // opcionális vizuális jelzés
                return;
            }

            _mainVm?.RefreshSettings(newPath, newInterval);
            this.Close();
        }
    }
}