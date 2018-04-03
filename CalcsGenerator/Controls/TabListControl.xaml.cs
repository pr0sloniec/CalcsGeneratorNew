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

        private void UpdateAllSumm(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int allsum = 0;
            foreach (var item in Tabs)
            {
                allsum += item.Count;
            }
            AllSumLabel.Content = "Итоговая сумма: " + allsum;
        }

        public async  Task<bool> SaveChanges()
        {
            var saveresult = await App.TrySaveChanges();
            if (saveresult)
            {
                foreach (var item in Tabs)
                {
                    item.IsChangesSaved = true;
                }                
                return true;
            }
            else
            {
                return false;
            }
        }

        async void RemoveTab(int index)
        {
            for(int i = Tabs.Count - 1; i >= 0; i--)
            {
                if (Tabs[i].TabId == index)
                {
                    Tabs.Remove(Tabs[i]);
                    currentproj.Tabs.Remove(currentproj.Tabs.Where(t=>t.Id==index).First());
                    await App.TrySaveChanges();
                }
            }
        }

        public TabListControl(int projid)
        {
            ProjectId = projid;
            currentproj = App.PC.Projects.Where(p => p.Id == ProjectId).First();

            InitializeComponent();
            UpdateTabs();
            UpdateAllSumm(null,null);

            TabList.ItemsSource = Tabs;
            TitleLabel.Content = currentproj.Name;
        }

        private void UpdateTabs()
        {
            foreach (var tab in Tabs)
            {
                tab.PropertyChanged -= UpdateAllSumm;
            }
            Tabs.Clear();
            foreach (var tab in currentproj.Tabs)
            {
                var w = new TabWithItems(tab.Id, ProjectId);
                w.RemoveTab = RemoveTab;
                w.SaveChanges = SaveChanges;
                w.PropertyChanged += UpdateAllSumm;
                Tabs.Add(w);
            }
        }

        private async void AddTab(object sender, MouseButtonEventArgs e)
        {
            string name = Interaction.InputBox("Введите", "Введите название");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            Tab tmp = new Tab();
            tmp.Name = name;
            currentproj.Tabs.Add(tmp);
            await App.TrySaveChanges();
            UpdateTabs();
        }

        private async void ChangeTabTitle(object sender, MouseButtonEventArgs e)
        {
            string title = Interaction.InputBox("Введите", "Введите заголовок");
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }
            currentproj.Name = title;
            await App.TrySaveChanges();
            TitleLabel.Content = title;
        }
    }
}
