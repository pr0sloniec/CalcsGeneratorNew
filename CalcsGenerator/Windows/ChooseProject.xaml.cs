using CalcsGenerator.DataModel;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CalcsGenerator.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChooseProject.xaml
    /// </summary>
    public partial class ChooseProject : Window
    {
        class ProjectInfo
        {
            public ProjectInfo(int Id,string Name)
            {
                this.Id = Id;
                this.Name = Name;
            }

            public int Id { get; private set; }
            public string Name { get; private set; }
            public bool IsOpened { get; set; }
        }

        public void OpenProject(int id)
        {
            lock (projectinfolistlock)
            {
                foreach (var item in ProjectsInfoList)
                {
                    if (item.Id == id)
                    {
                        if (!item.IsOpened)
                        {
                            item.IsOpened = true;
                            ProjectWindow pw = new ProjectWindow(id);
                            pw.CloseProject = CloseProject;
                            pw.Owner = null;
                            pw.Show();
                            return;
                        }
                        else
                        {
                            Interaction.MsgBox("Проект уже открыт!");
                        }

                    }
                }
            }    
        }

        public void CloseProject(int id)
        {
            lock (projectinfolistlock)
            {
                foreach (var item in ProjectsInfoList)
                {
                    if (item.Id == id)
                    {
                        item.IsOpened = false;
                        return;
                    }
                }
                throw new Exception("Проект не найден!");
            }
        }

        ObservableCollection<ProjectInfo> ProjectsInfoList = new ObservableCollection<ProjectInfo>();
        readonly Object projectinfolistlock = new Object();

        private void UpdateProjectList()
        {
            Console.WriteLine("Получение списка проектов...");
            ProjectsInfoList.Clear();
            foreach(var item in App.PC.Projects)
            {
                ProjectsInfoList.Add(new ProjectInfo(item.Id, item.Name));
            }
            Console.WriteLine("Найдено {0} проектов!", ProjectsInfoList.Count);
        }

        public ChooseProject()
        {
            InitializeComponent();

            UpdateProjectList();
            ProjectList.ItemsSource = ProjectsInfoList;
        }

        private void RemoveProjectClick(object sender, RoutedEventArgs e)
        {
            var item = ProjectList.SelectedItem;
            if (item == null) return;

            App.PC.Projects.Remove(item as Project);
            App.PC.SaveChanges();
            UpdateProjectList();
        }

        private void CreateProject(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text))
            {
                Interaction.MsgBox("Следует ввести имя!");
            }


            Project p = new Project();
            p.Name = NameInput.Text;
            p.Info = CommentInput.Text;

            App.PC.Projects.Add(p);
            App.TrySaveChanges();

            Console.WriteLine("Был создан проект: {0}. Его основной ключ:{1}", p.Name, p.Id);
            OpenProject(p.Id);

            UpdateProjectList();
        }

        private void RemoveProject(object sender, MouseButtonEventArgs e)
        {
            var item = ProjectList.SelectedItem as ProjectInfo;
            if (item == null) return;
            foreach(var proj in ProjectsInfoList)
            {
                if (proj.Id==item.Id&&proj.IsOpened)
                {
                    Interaction.MsgBox("Нельзя удалить открытый проект! Сначала закройте соответствующее окно!");
                    return;
                }
            }
            Project tmp = App.PC.Projects.Where(p => p.Id == item.Id).First();
            App.PC.Projects.Remove(tmp);
            App.TrySaveChanges();
            UpdateProjectList();
        }

        private void OpenProject(object sender, MouseButtonEventArgs e)
        {
            var item = ProjectList.SelectedItem as ProjectInfo;
            if (item == null) return;
            OpenProject(item.Id);
        }

        private void ImportProject(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.ShowDialog();

                if (!string.IsNullOrEmpty(openFileDialog1.FileName))
                {
                    AsyncExecute ae = new AsyncExecute(() =>
                    {
                        using (FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            Project tmp = JsonConvert.DeserializeObject<Project>(reader.ReadToEnd());
                            Console.WriteLine("Десериализован проект {0}, добавляю в базу данных", tmp.Name);
                            App.PC.Projects.Add(tmp);
                            App.TrySaveChanges();
                        }
                    });
                    ae.ShowDialog();
                    UpdateProjectList();
                }
            }
            catch(Exception ex)
            {
                Interaction.MsgBox("Не удается провести импорт: " + ex.Message);
            }
        }
    }
}
