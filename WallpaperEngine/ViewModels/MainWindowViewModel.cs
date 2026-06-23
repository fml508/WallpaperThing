using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using WallpaperEngine.Views;
using System.IO;
using System.Linq;
using Avalonia.Threading;
using System;

namespace WallpaperEngine.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Control? _currentWallpaperView;

        private string _folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshots");
        private string[] _files = Array.Empty<string>();
        private int _currentIndex = 0;
        private DispatcherTimer? _timer;

        public MainWindowViewModel()
        {
            LoadFiles(_folderPath);
            StartTimer(5);
        }

        public void RefreshSettings(string folderPath, int intervalSeconds)
        {
            _timer?.Stop();
            _folderPath = folderPath;
            _currentIndex = 0;
            LoadFiles(folderPath);

            if (_files.Length > 0)
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Tick += (s, e) => ShowNextWallpaper();
                }
                _timer.Interval = TimeSpan.FromSeconds(intervalSeconds);
                _timer.Start();
                ShowNextWallpaper();
            }
        }

        private void LoadFiles(string folderPath)
        {
            _files = Directory.GetFiles(folderPath, "*.*")
                .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))
                .ToArray();
        }

        private void StartTimer(int intervalSeconds)
        {
            if (_files.Length == 0) return;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(intervalSeconds);
            _timer.Tick += (s, e) => ShowNextWallpaper();
            _timer.Start();

            ShowNextWallpaper();
        }

        private void ShowNextWallpaper()
        {
            if (_files.Length == 0) return;

            string currentFile = _files[_currentIndex];
            CurrentWallpaperView = new ImageWallpaperView(currentFile);
            _currentIndex = (_currentIndex + 1) % _files.Length;
        }
        public void RefreshIntervalOnly(int intervalSeconds)
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Interval = TimeSpan.FromSeconds(intervalSeconds);
            _timer.Start();
        }
    }
}
