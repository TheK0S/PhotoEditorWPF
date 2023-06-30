
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using static PhotoEditorWPF.MagicWithImageHappeningHere;


namespace PhotoEditorWPF
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    /// 
    public partial class MainPage : Page
    {              

        private bool IsDrawingModeEnabled = false, rigthMovement = false;

        private Point _previousMousePosition;

        private Brush defaultBackground;

        public System.Windows.Ink.DrawingAttributes DrawingAttributes { get; set; }

        public MainPage()
        {
            InitializeComponent();

            DataContext = this;

            DrawingAttributes = new System.Windows.Ink.DrawingAttributes();
            DrawingAttributes.Width = 1;
            DrawingAttributes.Width = 1;

            DrawingAttributes.Color = Colors.Black;

            defaultBackground = drawingCanvas.Background;

            _previousMousePosition = new Point(CanvasHeight.ActualWidth, CanvasHeight.ActualHeight);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void addImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                if(drawingCanvas.Background != defaultBackground)
                    bitmapImages[index] = new BitmapImage(new Uri(openFileDialog.FileName));                
                else
                    bitmapImages.Add(new BitmapImage(new Uri(openFileDialog.FileName)));

                drawingCanvas.Background = new ImageBrush(bitmapImages[index]);

                drawingCanvas.MaxHeight = bitmapImages[index].Height;
                drawingCanvas.MaxWidth = bitmapImages[index].Width;

                widthImage.Text = ((int)bitmapImages[index].Width).ToString();
                heightImage.Text = ((int)bitmapImages[index].Height).ToString();
            }
        }



        private void saveImage_Click(object sender, RoutedEventArgs e)
        {
            if (bitmapImages[index] != null)
                SaveImage(drawingCanvas, bitmapImages[index].PixelWidth, bitmapImages[index].PixelHeight);
            else
                MessageBox.Show("Не выбрано изображения для сохранения", "Ошибка");
        }

        private async void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (drawingCanvas != null && bitmapImages.Count > 0 && bitmapImages[index] != null)
            {
                drawingCanvas.Background = new ImageBrush(await SetImageBrightness(bitmapImages[index], brightnessSlider.Value));

                brightnessTextValue.Text = $"Яркость {(int)(brightnessSlider.Value * 100)}%";
            }
        }


        private void EnterEditingMode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bitmapImages[index] == null) return;


            if (!IsDrawingModeEnabled)
            {
                IsDrawingModeEnabled = true;

                drawingCanvas.IsEnabled = true;

                return;
            }

            if (IsDrawingModeEnabled)
            {
                IsDrawingModeEnabled = false;

                drawingCanvas.IsEnabled = false;

                return;
            }
        }

        private void WidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DrawingAttributes == null) return;

            DrawingAttributes.Width = ((Slider)sender).Value;
        }

        private void HeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DrawingAttributes == null) return;

            DrawingAttributes.Height = ((Slider)sender).Value;
        }


        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var images = files.Where(IsImage).ToList();

                int currentBitmapImagesCount = bitmapImages.Count;

                foreach (var image in images)
                    bitmapImages.Add(new BitmapImage(new Uri(image)));

                AddTabs(currentBitmapImagesCount);
            }


        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }



        private void CanvasHeight_MouseMove(object sender, MouseEventArgs e)
        {
            if (!rigthMovement) return;

            ChangeCanvasValues(e, heightImage);

            e.Handled = true;
        }

        private void CanvasWidth_MouseMove(object sender, MouseEventArgs e)
        {

            if (!rigthMovement) return;

            ChangeCanvasValues(e, widthImage);

            e.Handled = true;
        }


        private void CanvasValues_MouseDown(object sender, MouseButtonEventArgs e)
        {
           rigthMovement = true;
        }
        private void CanvasValues_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rigthMovement = false;
        }




        void ChangeCanvasValues(MouseEventArgs e,TextBox textBox)
        {
            if (textBox.Text == "") return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double delta = e.GetPosition(null).X - _previousMousePosition.X;

                if (delta > 0)
                    textBox.Text = (Convert.ToInt32(textBox.Text) + 1).ToString();
                else if (delta < 0)
                    textBox.Text = (Convert.ToInt32(textBox.Text) - 1).ToString();

                _previousMousePosition = e.GetPosition(null);
            }

        }


        private void heightImage_TextChanged(object sender, TextChangedEventArgs e)
        {
            drawingCanvas.Height = double.TryParse(heightImage.Text, out double height) ? height : double.NaN;
        }

      
        private void widthImage_TextChanged(object sender, TextChangedEventArgs e)
        {
            drawingCanvas.Width  = double.TryParse(widthImage.Text,out double width) ? width : double.NaN;  
        }

        //private Button CreateCloseButton()
        //{
        //    Button closeButton = new Button();
        //    closeButton.Content = "x";
        //    closeButton.Click += CloseButton_Click;

        //    return closeButton;
        //}

        //private void CloseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    bitmapImages.RemoveAt(index);
        //    tabControl.Items.RemoveAt(index);
        //    pageList.RemoveAt(index);
        //}
    }
    
}

