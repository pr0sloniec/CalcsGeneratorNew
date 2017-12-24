using CalcsGenerator.DataModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace CalcsGenerator.Windows
{
    /// <summary>
    /// Логика взаимодействия для DatabaseSyncWindow.xaml
    /// </summary>
    public partial class DatabaseSyncWindow : Window
    {

        List<PriceItem> items;
        readonly Object itemslock = new Object();
        List<PriceItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                BindingOperations.EnableCollectionSynchronization(items, itemslock);
            }
        }

        public DatabaseSyncWindow()
        {
            InitializeComponent();
            Items = new List<PriceItem>();
        }

        private void LoadFromExcel(string path)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;


            int i = 1;
            int j = 1;
            while (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
            {
                PriceItem tmp = new PriceItem();
                while (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                {
                    if (j == 1)
                    {
                        string name = xlRange.Cells[i, j].Value2.ToString();
                        tmp.Name = char.ToUpper(name[0]) + name.Substring(1);
                    }
                    else if (j == 2) tmp.Type = xlRange.Cells[i, j].Value2.ToString();
                    else if (j == 3) tmp.Value = ((int)Math.Round(xlRange.Cells[i, j].Value2)).ToString();
                    j++;
                }
                i++;
                j = 1;
                Items.Add(tmp);
            }


            PriceItem works = new PriceItem();
            works.Name = "Расходные материалы ";
            works.Type = "%";
            works.Value = "20";

            PriceItem works1 = new PriceItem();
            works1.Name = "Накладные расходы ";
            works1.Type = "%";
            works1.Value = "20";


            items.Add(works);
            items.Add(works1);
            items.Sort();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }

        private void StartSync(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog1.FileName))
            {
                AsyncExecute ae = new AsyncExecute(() =>
                {
                    try
                    {
                        LoadFromExcel(openFileDialog1.FileName);
                    }
                    catch(Exception ex)
                    {
                        Interaction.MsgBox("Не удается считать данные из файла: " + ex.Message);
                        App.Current.Dispatcher.Invoke(() => Close());
                    }
                });
                ae.ShowDialog();
                if (RewriteCheck.IsChecked == true)
                {
                    foreach (var entity in App.PC.Items)
                    {
                        App.PC.Items.Remove(entity);
                    }
                        
                    App.TrySaveChanges();

                    foreach(var item in Items)
                    {
                        App.PC.Items.Add(item);
                    }
                    App.TrySaveChanges();

                    Console.WriteLine("База данных синхронизирована! Добавлено {0} элементов", Items.Count);
                    Interaction.MsgBox("База данных успешно синхронизирована!");
                    AppConsole.Restart();
                }
                else if(ConcatCheck.IsChecked==true)
                {
                    foreach(var item in Items)
                    {
                        App.PC.Items.Add(item);
                    }
                    App.TrySaveChanges();
                    Console.WriteLine("База данных синхронизирована! Добавлено {0} элементов", Items.Count);
                    Interaction.MsgBox("База данных успешно синхронизирована!");
                    AppConsole.Restart();
                }
                else if (UpdateCheck.IsChecked == true)
                {
                    foreach (var item in Items)
                    {
                        foreach(var olditem in App.PC.Items)
                        {
                            if (item.Name == olditem.Name)
                            {
                                olditem.Type = item.Type;
                                olditem.Value = item.Value;
                            }
                        }
                    }
                    App.TrySaveChanges();
                    Console.WriteLine("База данных синхронизирована! Добавлено {0} элементов", Items.Count);
                    Interaction.MsgBox("База данных успешно синхронизирована!");
                    AppConsole.Restart();
                }
            }

        }
    }
}
