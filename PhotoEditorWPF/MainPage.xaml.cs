using Microsoft.Win32;
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


namespace PhotoEditorWPF
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        Image? originalImage;

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
                originalImage = DeepDataCopy(currentImage);                
            }
        }

        private void saveImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage?.Source != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (saveFileDialog.ShowDialog() == true)
                {
                    BitmapSource bitmapSource = (BitmapSource)currentImage.Source;
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                    using(FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
            }
            else
                MessageBox.Show("Не выбрано изображения для сохранения", "Ошибка");
        }

        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(currentImage?.Source != null)
            {
                currentImage.Source = SetImageBrightness((BitmapSource)originalImage.Source, brightnessSlider.Value);
                brightnessTextValue.Text = $"Яркость {(int)(brightnessSlider.Value * 100)}%";
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

        private Image DeepDataCopy(Image image)
        {
            Image copiedImage = new Image();

            if (image != null)
            {
                copiedImage.Width = image.Width;
                copiedImage.Height = image.Height;

                if (image.Source is BitmapSource bitmapSource)
                {
                    BitmapSource copiedBitmapSource = new WriteableBitmap(bitmapSource);
                    copiedImage.Source = copiedBitmapSource;
                }
            }

            return copiedImage;
        }
    }
}
