using CalcsGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

using Excel = Microsoft.Office.Interop.Excel;

namespace CalcsGenerator.Windows
{
    /// <summary>
    /// Логика взаимодействия для ExporterWindow.xaml
    /// </summary>
    /// 

    public enum ExportType { Excel, Pdf };

    public partial class ExporterWindow : Window
    {
        class InfoItem : INotifyPropertyChanged
        {
            public InfoItem()
            {

            }

            public InfoItem(string name,string value)
            {
                this.name = name;
                this.value = value;
            }

            string name;
            public string Name { get { return name; } set { name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } }

            string value;
            public string Value { get { return value; } set { value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        ObservableCollection<InfoItem> InfoList = new ObservableCollection<InfoItem>
        {
            new InfoItem("Название мероприятия:",""),
            new InfoItem("Дата мероприятия:",""),
            new InfoItem("Место проведение:",""),
            new InfoItem("Количество дней:","")
        };

        public static Excel.Range MergeCells(Excel.Worksheet sheet, Excel.Range firstcell, Excel.Range secondcell)
        {
            var excelcells = sheet.get_Range(firstcell, secondcell);
            excelcells.Merge(Type.Missing);
            return excelcells;
        }

        public static void SetCellColor(Excel.Range cell, System.Drawing.Color color)
        {
            cell.Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
        }

        public ExporterWindow(Project proj, ExportType type)
        {
            InitializeComponent();

            InfoGrid.ItemsSource = InfoList;
        }
    }
}
