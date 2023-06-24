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
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new MainPage());

            buttons.Add(new Button { Name = "b1", Height = 20, Width = 100, Content = "hjhj"});
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });
            buttons.Add(new Button { Name = "b2", Height = 20, Width = 100, Content = "qwqwwq" });

            foreach (var button in buttons)
                navigationPanel.Children.Add(button);


            
        }
    }
}
