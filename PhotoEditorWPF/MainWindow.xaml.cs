using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Button> buttons = new List<Button>();
        List<Task> tasks = new List<Task>();
        public MainWindow()
        {
            InitializeComponent();

            //mainFrame.Navigate(new MainPage());

            OpenMainPageAsync();

            buttons.Add(new Button { Name = "b1", Height = 20, Width = 100, Content = "First image"});
            

            foreach (var button in buttons)
                navigationPanel.Children.Add(button);
        }

        private async void OpenMainPageAsync()
        {
            //tasks.Add(new Task(() => mainFrame.Navigate(new MainPage())));
            ////Task.Run(() =>
            ////{                
            ////    mainFrame.Navigate(new MainPage());
            ////});

           await  Application.Current.Dispatcher.InvokeAsync(() =>
            {
                mainFrame.Navigate(new MainPage());
            });

        }
    }
}
