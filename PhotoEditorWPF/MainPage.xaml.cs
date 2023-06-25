﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PhotoEditorWPF
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        List<System.Windows.Controls.Image> images = new List<System.Windows.Controls.Image>();
        public MainPage()
        {
            InitializeComponent();
        }

        private void addImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();

                currentImage.Source = bitmap;
                images.Add(currentImage);
            }
        }

        private void saveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if(saveFileDialog.ShowDialog() == true)
            {
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)currentImage.ActualWidth, (int)currentImage.ActualHeight, 96, 96, PixelFormats.Default);
                renderBitmap.Render(currentImage);

                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(currentImage != null)
            {
                int imageIndex = 0;
                if (imageIndex != -1)
                {
                    currentImage.Source = SetImageBrightness((BitmapSource)images[imageIndex].Source, brightnessSlider.Value);
                }
            }            
        }

        private WriteableBitmap SetImageBrightness(BitmapSource bitmapSource, double brightness)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            // Получение пиксельного массива изображения
            int stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
            int size = stride * bitmapSource.PixelHeight;
            byte[] pixels = new byte[size];
            bitmapSource.CopyPixels(pixels, stride, 0);

            // Изменение яркости пикселей
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                // Изменение яркости каждого цветового канала
                double adjustedR = r * brightness;
                double adjustedG = g * brightness;
                double adjustedB = b * brightness;

                // Ограничение значений в пределах 0-255
                adjustedR = Math.Max(0, Math.Min(255, adjustedR));
                adjustedG = Math.Max(0, Math.Min(255, adjustedG));
                adjustedB = Math.Max(0, Math.Min(255, adjustedB));

                // Присвоение измененных значений обратно в пиксели
                pixels[i] = (byte)adjustedB;
                pixels[i + 1] = (byte)adjustedG;
                pixels[i + 2] = (byte)adjustedR;
            }

            // Запись измененных пикселей обратно в WriteableBitmap
            writeableBitmap.WritePixels(new Int32Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight), pixels, stride, 0);

            return writeableBitmap;
        }
    }
}
