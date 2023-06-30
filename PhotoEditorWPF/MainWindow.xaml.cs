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
using static PhotoEditorWPF.MagicWithImageHappeningHere;

namespace PhotoEditorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            tabControl = pages;

            //mainFrame.Navigate(new MainPage());

            for (int i = 0; i < 1; i++)
            {
                MainPage mainPage = new MainPage();
                Frame frame = new Frame();
                frame.Content = mainPage;

                pageList.Add(mainPage);

                TabItem tabItem = new TabItem();
                tabItem.Header = $"Page {i+1}";
                tabItem.TabIndex = i;
                tabItem.Content = frame;

                pages.Items.Add(tabItem);             
            }

            pages.SelectedIndex = 0;
            
        }

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = pages.SelectedIndex;
        }
    }
}
