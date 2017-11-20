using Microsoft.VisualBasic;
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
using System.Windows.Shapes;

namespace CalcsGenerator.Windows
{
    /// <summary>
    /// Логика взаимодействия для AsyncExecute.xaml
    /// </summary>
    public partial class AsyncExecute : Window
    {
        Action lambda;

        bool isClosable = false;

        public AsyncExecute(Action lambda)
        {
            InitializeComponent();

            this.lambda = lambda;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isClosable)
            {
                e.Cancel = true;
                Interaction.MsgBox("Процесс не завершен. Невозможно закрыть окно!");
            }
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Task.Run(() => { lambda(); });
            isClosable = true;
            Close();
        }
    }
}
