using Avalonia.Controls;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Avalonia;
using System.Diagnostics;
using System;

using SharpImage = SixLabors.ImageSharp.Image;

namespace WallpaperEngine.Views
{
    public partial class ImageWallpaperView : UserControl, IDisposable
    {
        // Osztályszintű változó a bitkép tárolására, hogy tudjuk "dispose"-olni
        private Bitmap? _currentBitmap;

        public ImageWallpaperView()
        {
            InitializeComponent();
        }
        public ImageWallpaperView(string imagePath)
        {
            InitializeComponent();
            LoadWallpaper(imagePath);
        }

        private void LoadWallpaper(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                // ELŐSZÖR TAKARÍTÁS: Felszabadítjuk a régi bitkép memóriáját
                _currentBitmap?.Dispose();

                var stopwatch = Stopwatch.StartNew();

                using (var image = SharpImage.Load(imagePath))
                {
                    Debug.WriteLine($"[WallpaperEngine] Eredeti méret: {image.Width}x{image.Height}");

                    // Felskálázás
                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new SixLabors.ImageSharp.Size(1920, 1080),
                            Mode = ResizeMode.Crop
                        })
                        .GaussianSharpen(0.5f)
                    );

                    stopwatch.Stop();
                    Debug.WriteLine($"[WallpaperEngine] Felskálázva 1920x1080-ra {stopwatch.ElapsedMilliseconds}ms alatt.");

                    // MemoryStream a kép átmeneti tárolására
                    using (var ms = new MemoryStream())
                    {
                        image.SaveAsPng(ms);
                        ms.Position = 0;

                        // Új bitkép létrehozása és elmentése osztályszintű változóba
                        _currentBitmap = new Bitmap(ms);

                        var imageControl = this.FindControl<Avalonia.Controls.Image>("WallpaperImage");
                        if (imageControl != null)
                        {
                            imageControl.Source = _currentBitmap;
                            imageControl.Stretch = Avalonia.Media.Stretch.UniformToFill;
                        }
                    }
                }
            }
        }

        // Metódus a memória explicit felszabadítására, ha a View bezáródik
        public void Dispose()
        {
            _currentBitmap?.Dispose();
            _currentBitmap = null;
        }
    }
}