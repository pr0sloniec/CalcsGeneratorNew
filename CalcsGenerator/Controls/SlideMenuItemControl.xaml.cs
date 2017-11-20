using System;
using System.Collections.Generic;
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

namespace CalcsGenerator.Controls
{
    /// <summary>
    /// Логика взаимодействия для SlideMenuItemControl.xaml
    /// </summary>
    public partial class SlideMenuItemControl : UserControl
    {

        Action itemclick;
        public Action ItemClick
        {
            set
            {
                itemclick = value;
            }
        }

        public string Text
        {
            set
            {
                ItemName.Content = value;
            }
        }

        public int IconHeightWidth {
            set
            {
                ItemImage.Height = value;
                ItemImage.Width = value;
            }
        }

        public string IconSource {
            set
            {
                ItemImage.Source = new BitmapImage(new Uri(value));
            }
        }

        public SlideMenuItemControl()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            itemclick?.Invoke();
        }
    }
}
