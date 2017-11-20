using CalcsGenerator.Controls;
using CalcsGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace CalcsGenerator.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        bool IsMenuOpened = false;

        public int ProjectId { get; private set; }
        public Action<int> CloseProject { get; set; }

        public ProjectWindow(int projectid)
        {
            InitializeComponent();
            ProjectFrame.Content = new TabListControl(projectid);
            ProjectId = projectid;
        }

        public void MenuItemClick()
        {
            if (IsMenuOpened)
            {
                Storyboard sb = Resources["sbHideLeftMenu"] as Storyboard;
                sb.Begin(SlideMenu);
            }
            else
            {
                Storyboard sb = Resources["sbShowLeftMenu"] as Storyboard;
                sb.Begin(SlideMenu);
            }

            IsMenuOpened = !IsMenuOpened;
        }

        public void JsonExport()
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.ShowDialog();

                if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    AsyncExecute ae = new AsyncExecute(() =>
                    {

                        App.PC.Configuration.LazyLoadingEnabled = false;
                        App.PC.Configuration.ProxyCreationEnabled = false;
                        Project tmp = App.PC.Projects.Where(p => p.Id == ProjectId).First();

                        using (FileStream stream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(JsonConvert.SerializeObject(tmp));
                            Console.WriteLine("Произведен экспорт проекта {0}", tmp.Name);
                        }

                        App.PC.Configuration.LazyLoadingEnabled = true;
                        App.PC.Configuration.ProxyCreationEnabled = true;
                    });
                    ae.ShowDialog();
                }
            }
            catch(Exception ex)
            {
                Interaction.MsgBox("Не удается провести экспорт: " + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseProject?.Invoke(ProjectId);
        }
    }
}
