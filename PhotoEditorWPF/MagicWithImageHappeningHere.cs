using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoEditorWPF
{
   public static class MagicWithImageHappeningHere
    {

        public static List<BitmapImage> bitmapImages = new();

        public static List<MainPage> pageList = new List<MainPage>();

        public static int index = 0;

        public static TabControl tabControl;

        public static async Task<WriteableBitmap> SetImageBrightness(BitmapSource bitmapSource, double brightness)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
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
            });

            return writeableBitmap;
        }



        public static async void SaveImage(InkCanvas drawingCanvas, double width, double height)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {

                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (saveFileDialog.ShowDialog() == true)
                    { 
                      
                        double dpiX = 96;
                        double dpiY = 96;


                        Rect bounds = VisualTreeHelper.GetDescendantBounds(drawingCanvas);
                        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                            (int)bounds.Width, (int)bounds.Height, dpiX, dpiY, PixelFormats.Default);
                        renderTargetBitmap.Render(drawingCanvas);

                        BitmapEncoder encoder;

                        encoder = new PngBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                        using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            encoder.Save(stream);
                        }

                        MessageBox.Show("Image succesfully saved");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Issue occurred while saving an image");
                }
            });
        }



        public static bool IsImage(string file)
        {
            string extentions = Path.GetExtension(file);
            string[] imageExtentions = { ".jpeg", ".jpg", ".png", ".gif", ".bpm" };

            return imageExtentions.Contains(extentions, StringComparer.OrdinalIgnoreCase);
        }
  

        public static async void AddTabs(int previusBitamapImagesCount)
        {

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {

                if (previusBitamapImagesCount == 0) tabControl.Items.Clear();

                for (int i = previusBitamapImagesCount; i < bitmapImages.Count; i++)
                {
                    index = i;

                    MainPage mainPage = new MainPage();
                    mainPage.drawingCanvas.Background = new ImageBrush(bitmapImages[i]);
                    mainPage.widthImage.Text = bitmapImages[i].Width.ToString();
                    mainPage.heightImage.Text = bitmapImages[i].Height.ToString();

                    Frame frame = new Frame();
                    frame.Content = mainPage;

                    pageList.Add(mainPage);

                    TabItem tabItem = new TabItem();
                    tabItem.Header = CreateHeaderGrid($"Page {i + 1}");
                    tabItem.TabIndex = i;
                    tabItem.Content = frame;

                    tabControl.Items.Add(tabItem);
                }
            });
        }

        public static  Grid CreateHeaderGrid(string headerName)
        {
            

                Grid headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition());

                // Создание TextBlock для надписи слева
                TextBlock textBlock = new TextBlock();
                textBlock.Text = headerName;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                // Создание кнопки закрытия справа
                Button closeButton = new Button();
                closeButton.Content = "✖";
                closeButton.Background = new SolidColorBrush(Colors.White);
                closeButton.HorizontalAlignment = HorizontalAlignment.Right;
                closeButton.VerticalAlignment = VerticalAlignment.Center;
                closeButton.Margin = new Thickness(5, 0, 0, 0);
                closeButton.BorderThickness = new Thickness(0);
                closeButton.Click += CloseButton_Click;

                // Добавление элементов в Grid хедера
                headerGrid.Children.Add(textBlock);
                headerGrid.Children.Add(closeButton);
                Grid.SetColumn(textBlock, 0);
                Grid.SetColumn(closeButton, 1);

            
                return headerGrid;
        }

        private static void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if(tabControl.Items.Count == 1)
            {
                if (MessageBox.Show("Вы уверены что хотите закрыть приложение?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Application.Current.MainWindow.Close();
            }
            else
            {
                bitmapImages.RemoveAt(index);
                tabControl.Items.RemoveAt(index);
                pageList.RemoveAt(index);
            }                      
        }
    }
}
