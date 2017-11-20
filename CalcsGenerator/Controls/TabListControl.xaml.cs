using CalcsGenerator.DataModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для TabListControl.xaml
    /// </summary>
    public partial class TabListControl : UserControl
    {
        public int ProjectId { get; private set; }

        Project currentproj;
        ObservableCollection<TabWithItems> Tabs = new ObservableCollection<TabWithItems>();

        public void SaveChanges()
        {
            foreach(var item in Tabs)
            {
                item.SaveChanges();
            }
        }

        void RemoveTab(int index)
        {
            Interaction.MsgBox(index);
            for(int i = Tabs.Count - 1; i >= 0; i--)
            {
                if (Tabs[i].TabId == index)
                {
                    Tabs.Remove(Tabs[i]);
                    currentproj.Tabs.Remove(currentproj.Tabs.Where(t=>t.Id==index).First());
                    App.TrySaveChanges();
                }
            }
        }

        public TabListControl(int projid)
        {
            ProjectId = projid;
            currentproj = App.PC.Projects.Where(p => p.Id == ProjectId).First();

            InitializeComponent();
            UpdateTabs();
            
            TabList.ItemsSource = Tabs;
            TitleLabel.Content = currentproj.Name;
        }

        private void UpdateTabs()
        {
            Tabs.Clear();
            foreach (var tab in currentproj.Tabs)
            {
                var w = new TabWithItems(tab.Id, ProjectId);
                w.RemoveTab = RemoveTab;
                Tabs.Add(w);
            }
        }

        private void AddTab(object sender, MouseButtonEventArgs e)
        {
            string name = Interaction.InputBox("Введите", "Введите название");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            Tab tmp = new Tab();
            tmp.Name = name;
            currentproj.Tabs.Add(tmp);
            App.TrySaveChanges();
            UpdateTabs();
        }

        private void ChangeTabTitle(object sender, MouseButtonEventArgs e)
        {
            string title = Interaction.InputBox("Введите", "Введите заголовок");
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }
            currentproj.Name = title;
            App.TrySaveChanges();
            TitleLabel.Content = title;
        }
    }
}
