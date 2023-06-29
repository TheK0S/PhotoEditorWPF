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
      


        public static  WriteableBitmap SetImageBrightness(BitmapSource bitmapSource, double brightness)
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

        public static void SaveImage(InkCanvas drawingCanvas, double width, double height)
        {
            try
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                if (saveFileDialog.ShowDialog() == true)
                {
                    var tempWidth = drawingCanvas.Width;
                    var tempHeight = drawingCanvas.Height;

                    drawingCanvas.Height = height;
                    drawingCanvas.Width = width;

                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)drawingCanvas.Width, (int)drawingCanvas.Height, 96, 96, PixelFormats.Default);

                    renderTargetBitmap.Render(drawingCanvas);

                    BitmapEncoder encoder = new PngBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(stream);
                    }

                    drawingCanvas.Height = tempHeight;
                    drawingCanvas.Width = tempWidth;
                }

                MessageBox.Show("Image succesfully saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Issue occurred while saving an image");

            }

        }

        public static bool IsImage(string file)
        {
            string extentions = System.IO.Path.GetExtension(file);

            string[] imageExtentions = { ".jpeg", ".jpg", ".png", ",gif", "bpm" };

            return imageExtentions.Contains(extentions, StringComparer.OrdinalIgnoreCase);
        }




        public static TabItem AddNewImage(int tabIndex,BitmapImage bitmapImage) 
        {
            TabItem tabItem = new TabItem();

               tabItem.Header = "Image" + tabIndex;
            tabItem.TabIndex = tabIndex ;

            InkCanvas drawingCanvas = new InkCanvas();

            drawingCanvas.Background = new ImageBrush(bitmapImage);

            tabItem.Content = drawingCanvas;

            return tabItem;
        }

       
      //public static bool IsImageAdded(List<BitmapImage> bitmapImages)
      //  {
      //      if (bitmapImages.Count == 0) return false;

      //      if (bitmapImages[bitmapImages.Count - 1] != null)
      //      {

      //      }


      //  }



    }
}
