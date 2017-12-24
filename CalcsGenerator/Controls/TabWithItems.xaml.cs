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
using CalcsGenerator;
using CalcsGenerator.DataModel;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Text.RegularExpressions;

namespace CalcsGenerator.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemList.xaml
    /// </summary>
    public partial class TabWithItems : UserControl
    {

        public int TabId { get; private set; } //Идентификатор текущей вкладки в базе данных
        public int ProjectId { get; private set; } //Идентификатор проекта в базе данных

        public Action<int> RemoveTab { get; set; }

        public Func<bool> SaveChanges { get; set; }

        public bool IsChangesSaved {
            set
            {
                if (value) RootBorder.BorderBrush = Brushes.SkyBlue;
                else RootBorder.BorderBrush = Brushes.Red;
            }
        }

        int count;
        public int Count { get { return count; } private set { count = value; CountTextBox.Text = "Итого: " + count.ToString(); } }  //Счетчик итого

        Project currentproj;
        Tab currenttab;

        ObservableCollection<TabRecord> TabRecords = new ObservableCollection<TabRecord>(); //Временное хранилище элементов вкладки для локальной обработки(к бд нельзя забандить и отловить события)

        int workcharge = 0;
        int partscharge = 0;

        public TabWithItems(int tabid, int projectid)
        {
            InitializeComponent();
            WorkCharge.ValueChanged =  UpdateAnotherCharge;
            WorkCharge.ShowInfo = () =>
            {
                int count = 0;
                foreach (var item in TabRecords)
                {
                    count += item.Price;
                }

                if (workcharge != 0) count = (int)Math.Floor((double)count / workcharge);
                else count = 0;

                Interaction.MsgBox("Стоимость работ составит " + count);
            };
            PartsCharge.ValueChanged =  UpdateAnotherCharge;
            PartsCharge.ShowInfo = () =>
            {
                int count = 0;
                foreach (var item in TabRecords)
                {
                    count += item.Price;
                }

                if (partscharge != 0) count = (int)Math.Floor((double)count / partscharge);
                else count = 0;

                Interaction.MsgBox("Стоимость расходных материалов составит " + count);
            };


            TabId = tabid;
            ProjectId = projectid;

            currentproj = App.PC.Projects.Where(p => p.Id == ProjectId).First();
            currenttab = currentproj.Tabs.Where(t => t.Id == TabId).First();

            TabTitle.Content = currenttab.Name;

            foreach (var item in currenttab.TabRecords)
            {
                TabRecords.Add(item);
                item.PropertyChanged += UpdateTabRecord;
            }

            TabRecords.CollectionChanged += UpdateTabRecords;

            RecordGrid.ItemsSource = TabRecords;

            if (currenttab.WorkCharge != 0)
            {
                WorkCharge.LoadData(currenttab.WorkCharge);
            }

            if (currenttab.PartsCharge != 0)
            {
                PartsCharge.LoadData(currenttab.PartsCharge);
            }

            workcharge = currenttab.WorkCharge;
            partscharge = currenttab.PartsCharge;

            UpdateCount();            

            //По умолчанию изменения не сохранены, так как что-то рандомно начинает их перебивать в GUI
            //Затем вызывается UpdateTabRecord, событие, вызываемое при изменении поля записи
            //Что красит некоторые таблицы в красный
            //Пустые таблицы остаются синими
            IsChangesSaved = false;
        }

        //Этот методобновляет прочие наценки
        private void UpdateAnotherCharge()
        {
            workcharge = WorkCharge.Value;
            partscharge = PartsCharge.Value;

            currenttab.WorkCharge = workcharge;
            currenttab.PartsCharge = partscharge;
            SaveChanges?.Invoke();

            UpdateCount();
        }

        //Этот метод должен добавять новые элементы
        private void UpdateTabRecords(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TabRecord oldItem in e.OldItems)
                {
                    currenttab.TabRecords.Remove(oldItem);
                    oldItem.PropertyChanged -= UpdateTabRecord;
                }
            }
                
            if(e.NewItems != null)
            {
                foreach (TabRecord newItem in e.NewItems)
                {
                    currenttab.TabRecords.Add(newItem);
                    newItem.PropertyChanged += UpdateTabRecord;
                    
                }
            }

            SaveChanges?.Invoke();
        }

        //Этот метод должен внеcти все изменения элементов в базу данных
        private void UpdateTabRecord(object sender, PropertyChangedEventArgs e)
        {
            var current = sender as TabRecord;
            var result = currenttab.TabRecords.SingleOrDefault(b => b.Id == current.Id);

            //Id нет в бд, новичок
            if (result == null)
            {
                currenttab.TabRecords.Add(current);
                return;
            }

            result.ReplaceValues(current);
            IsChangesSaved = false;

            UpdateCount();
        }

        private void UpdateCount() //Процедура обновления итого
        {
            Count = 0;
            foreach(var item in TabRecords)
            {
                Count += item.Price;
            }

            //Наценки
            Count += (int)Math.Floor((double)Count / 100 * currenttab.WorkCharge);
            Count += (int)Math.Floor((double)Count / 100 * currenttab.PartsCharge);
        }

        private void ChangeTabTitle(object sender, MouseButtonEventArgs e)
        {
            var newname = Interaction.InputBox("Введите", "Введите новое название:");
            if (string.IsNullOrWhiteSpace(newname))
            {
                Interaction.MsgBox("Введите название!");
                return;
            }
            TabTitle.Content = newname;
            currenttab.Name = newname;
            SaveChanges?.Invoke();
        }

        private void AddItemClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in TabRecords)
            {
                if (item.Id == 0)
                {
                    RecordGrid.CurrentItem = RecordGrid.Items.GetItemAt(RecordGrid.Items.Count - 1);

                    bool? issaved = SaveChanges?.Invoke();

                    if (issaved==true)
                    {
                        break;
                    }
                    else
                    {
                        Interaction.MsgBox("Заполните до конца предыдущее поле!");
                        return;
                    }   
                }
            }
            TabRecords.Add(new TabRecord());
            IsChangesSaved = false;
        }

        private void RemoveTabClick(object sender, MouseButtonEventArgs e)
        {
            RemoveTab(TabId);
        }

        private void TableComboBoxCheck(object sender, RoutedEventArgs e)
        {
            TabRecord obj = ((FrameworkElement)sender).DataContext as TabRecord;
            ComboBox box = (FrameworkElement)sender as ComboBox;

            if (obj == null) return;

            var overlap = App.PC.Items.Where(i => i.Name == box.Text);

            if (overlap.Count()==1)
            {
                obj.Count = 1;
                obj.Real = Int32.Parse((overlap.First().Value == null) ? "0" : overlap.First().Value);
                obj.Type = (overlap.First().Type == null) ? " " : overlap.First().Type;
            }
           
        }

        private void SetCharge(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("Введите", "Введите наценку");
            int charge = 0;

            if (string.IsNullOrWhiteSpace(input)) return;

            if(Int32.TryParse(input,out charge))
            {
                foreach(var item in TabRecords)
                {
                    item.Charge = charge;
                    IsChangesSaved = true;
                    SaveChanges.Invoke();
                }
            }
            else
            {
                Interaction.MsgBox("Не удается преобразовать введенный текст в число. Наценка - число без запяток, без пробелов и прочего оформления. Только число!");
            }
        }
    }
}
