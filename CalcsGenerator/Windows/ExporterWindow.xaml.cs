using CalcsGenerator.DataModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Resources;
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
                this.v = value;
            }

            string name;
            public string Name { get { return name; } set { name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } }

            string v;
            public string Value { get { return v; } set { v = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } }

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

        private void ToExcel(ExportType type) {
            int width = 6;

            int count = 1; //Использовать для отсчета текущей позиции

            System.Drawing.Color maincolor = System.Drawing.Color.FromArgb(255, 255, 255);  //Цвет фона(белый)
            System.Drawing.Color linecolor = System.Drawing.Color.FromArgb(217, 217, 217);  //Цвет отчерка
            System.Drawing.Color scorecolor = System.Drawing.Color.FromArgb(242, 242, 242);  //Цвет линии итого

            Excel.Application xlApp = new Excel.Application();

            Excel.Workbook wb = xlApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Excel.Worksheet xlWorksheet = wb.Sheets[1];

            xlApp.DisplayAlerts = false;
            xlApp.Visible = false;


            int topspacer = 6;

            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[topspacer, width]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                SetCellColor(splitcells, maincolor);
                count += topspacer;
            }


            //Логотип
            string path = System.IO.Path.GetTempPath() + "logo.png";
            string imagePath = "pack://application:,,,/CalcsGenerator;component/Images/logo.png";
            using (Stream stream = System.Windows.Application.GetResourceStream(new Uri(imagePath)).Stream)
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.CopyTo(fstream);
            }
            int iconheight = (int)Math.Floor(xlWorksheet.Rows.RowHeight * topspacer);
            xlWorksheet.Shapes.AddPicture(path, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 10, 10, iconheight * 3 - 20, iconheight - 20);


            //Отчерк
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                SetCellColor(splitcells, linecolor);
                count += 1;
            }



            //Вывод информации
            foreach (var item in InfoList)
            {
                {
                    var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width - 2]);
                    splitcells.HorizontalAlignment = Excel.Constants.xlLeft;
                    splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                    splitcells.Value2 = item.Name;
                    SetCellColor(splitcells, maincolor);
                }

                {
                    var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 1], xlWorksheet.Cells[count, width]);
                    splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                    splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                    splitcells.Value2 = item.Value;
                    SetCellColor(splitcells, maincolor);
                }
                count += 1;
            }

            //Отчерк
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                SetCellColor(splitcells, linecolor);
                count += 1;
            }

            //Заголовок таблицы
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count + 1, 1]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.Value2 = "№ п.п.";
                SetCellColor(splitcells, maincolor);
            }
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 2], xlWorksheet.Cells[count + 1, width - 4]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "Наименование и техническая характеристика материалов или видов работ";
                SetCellColor(splitcells, maincolor);
            }
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 3], xlWorksheet.Cells[count + 1, width - 3]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "Ед. измер.";
                SetCellColor(splitcells, maincolor);
            }
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 2], xlWorksheet.Cells[count + 1, width - 2]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "Кол. По  проекту";
                SetCellColor(splitcells, maincolor);
            }
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 1], xlWorksheet.Cells[count, width]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "Стоимость";
                SetCellColor(splitcells, maincolor);
                count++;
            }
            {
                var splitcells = xlWorksheet.Cells[count, width - 1];
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "цена  1ед.";
                SetCellColor(splitcells, maincolor);
            }
            {
                var splitcells = xlWorksheet.Cells[count, width];
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "Сумма";
                SetCellColor(splitcells, maincolor);
                count++;
            }

            int calclistcount = 1; //номер позиции
            double calclistsumm = 0;

            //Вывод таблиц
            foreach (var item in CurrentProject.Tabs)
            {
                double sum = 0;

                //Заголовок
                {
                    var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width]);
                    splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                    splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                    splitcells.Value2 = calclistcount.ToString() + "." + item.Name;
                    splitcells.RowHeight = 15;
                    SetCellColor(splitcells, maincolor);
                    count += 1;
                }
                foreach (var line in item.TabRecords)
                {
                    //Номер
                    {
                        var splitcells = xlWorksheet.Cells[count, 1];
                        splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                        splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                        splitcells.WrapText = true;
                        StringBuilder numberbuilder = new StringBuilder();
                        numberbuilder.Append(calclistcount.ToString());
                        numberbuilder.Append('.');
                        numberbuilder.Append(line.Number.ToString());
                        splitcells.NumberFormat = "@";
                        splitcells.Value2 = numberbuilder.ToString();
                        SetCellColor(splitcells, maincolor);
                    }

                    //Название
                    {
                        var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 2], xlWorksheet.Cells[count, width - 4]);
                        splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                        splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                        splitcells.WrapText = true;
                        splitcells.Value2 = line.Name;
                        splitcells.RowHeight = 15;
                        SetCellColor(splitcells, maincolor);
                    }

                    //Тип
                    {
                        var splitcells = xlWorksheet.Cells[count, width - 3];
                        splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                        splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                        splitcells.WrapText = true;
                        splitcells.Value2 = line.Type;
                        SetCellColor(splitcells, maincolor);
                    }

                    //Кол-во
                    {
                        var splitcells = xlWorksheet.Cells[count, width - 2];
                        splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                        splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                        splitcells.WrapText = true;
                        splitcells.Value2 = line.Count;
                        SetCellColor(splitcells, maincolor);
                    }

                    //Цена
                    {
                        var splitcells = xlWorksheet.Cells[count, width - 1];
                        splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                        splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                        splitcells.WrapText = true;
                        splitcells.NumberFormat = "#₽";
                        splitcells.Value2 = line.Cost;
                        SetCellColor(splitcells, maincolor);
                    }

                    //Сумма
                    {
                        var splitcells = xlWorksheet.Cells[count, width];
                        splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                        splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                        splitcells.WrapText = true;
                        splitcells.NumberFormat = "#₽";
                        splitcells.Value2 = line.Price;
                        SetCellColor(splitcells, maincolor);
                    }
                    count++;
                    sum += line.Price;
                }

                calclistsumm += sum;

                //Итог текст
                {
                    var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width - 2]);
                    splitcells.HorizontalAlignment = Excel.Constants.xlRight;
                    splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                    splitcells.WrapText = true;
                    splitcells.Value2 = "Итого:";
                    SetCellColor(splitcells, scorecolor);
                }

                //Итог число
                {
                    var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 1], xlWorksheet.Cells[count, width]);
                    splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                    splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                    splitcells.WrapText = true;
                    splitcells.NumberFormat = "#₽";
                    splitcells.Value2 = sum.ToString();
                    SetCellColor(splitcells, linecolor);
                }
                count++;

                calclistcount++;
            }


            //Отчерк
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                SetCellColor(splitcells, maincolor);
                count++;
            }


            //Общая стоимость текст
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width - 2]);
                splitcells.HorizontalAlignment = Excel.Constants.xlRight;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.Value2 = "Общая стоимость проекта:";
                splitcells.Cells.Font.Bold = true;
                SetCellColor(splitcells, scorecolor);
            }

            //Общая стоимость число
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 1], xlWorksheet.Cells[count, width]);
                splitcells.HorizontalAlignment = Excel.Constants.xlCenter;
                splitcells.VerticalAlignment = Excel.Constants.xlCenter;
                splitcells.WrapText = true;
                splitcells.NumberFormat = "#₽";
                splitcells.Cells.Font.Bold = true;
                splitcells.Value2 = calclistsumm.ToString();
                SetCellColor(splitcells, linecolor);
            }
            count++;

            //Две пустые строчки в конце
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, 1], xlWorksheet.Cells[count, width - 2]);
                SetCellColor(splitcells, maincolor);
            }
            {
                var splitcells = MergeCells(xlWorksheet, xlWorksheet.Cells[count, width - 1], xlWorksheet.Cells[count, width]);
                SetCellColor(splitcells, maincolor);
            }

            //Обводка
            {
                //var splitcells = xlWorksheet.get_Range(xlWorksheet.Cells[1, 1], xlWorksheet.Cells[count, width]);
                var splitcells = xlWorksheet.Range[xlWorksheet.Cells[1, 1], xlWorksheet.Cells[count, width]];
                splitcells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            }

            //Увеличить ширину второй ячейки
            {
                var splitcells = xlWorksheet.Cells[1, 2];
                splitcells.ColumnWidth = 39.71;
            }


            if (type == ExportType.Excel)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                App.Current.Dispatcher.Invoke(()=>saveFileDialog1.ShowDialog());

                if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    xlApp.DisplayAlerts = false;
                    wb.Saved = true;
                    wb.SaveAs(saveFileDialog1.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    wb.Close();
                    xlApp.DisplayAlerts = true;
                }
            }
            else if (type == ExportType.Pdf)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                App.Current.Dispatcher.Invoke(()=>saveFileDialog1.ShowDialog());

                if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    xlApp.DisplayAlerts = false;
                    wb.Saved = true;
                    wb.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, saveFileDialog1.FileName);
                    wb.Close();
                    xlApp.DisplayAlerts = true;
                }
            }
        
            xlApp.Quit();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlApp);
        }

        public static void SetCellColor(Excel.Range cell, System.Drawing.Color color)
        {
            cell.Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
        }

        public Project CurrentProject { get; private set; }

        public ExporterWindow(Project proj, ExportType type)
        {
            InitializeComponent();

            CurrentProject = proj;
            
            foreach(var tab in CurrentProject.Tabs)
            {
                int i = 1;
                double sum = 0;

                foreach(var item in tab.TabRecords)
                {
                    item.Number = i;
                    sum += item.Price;
                    i++;
                }
                
                //Добавить расходные материалы
                if (tab.PartsCharge != 0)
                {
                    TabRecord parts = new TabRecord();
                    parts.Number = i;
                    parts.Name = "Расходные материалы";
                    parts.Type = "%";
                    parts.Count = tab.PartsCharge;
                    parts.Real = (int)sum/100;

                    tab.TabRecords.Add(parts);
                }

                //Добавить накладные расходы
                if (tab.WorkCharge != 0)
                {
                    TabRecord works = new TabRecord();
                    works.Number = i;
                    works.Name = "Накладные расходы";
                    works.Type = "%";
                    works.Count = tab.WorkCharge;
                    works.Real = (int)sum / 100;

                    tab.TabRecords.Add(works);
                }
            }

            if (type == ExportType.Pdf)
            {
                PdfExportCheck.IsChecked = true;
            }
            else
            {
                ExcelExportCheck.IsChecked = true;
            }

            InfoGrid.ItemsSource = InfoList;
        }

        private void StartExport(object sender, RoutedEventArgs e)
        {
            ExportType temp;

            if (PdfExportCheck.IsChecked==true)
            {
                temp=ExportType.Pdf;
            }
            else
            {
                temp=ExportType.Excel;
            }

            AsyncExecute ae = new AsyncExecute(() =>
            {
                try
                {
                    ToExcel(temp);
                }
                catch(Exception ex)
                {
                    Interaction.MsgBox("В процессе экспорта произошла ошибка: " + ex.Message);
                }
            });
            ae.ShowDialog();
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var tab in CurrentProject.Tabs)
            {
                for(int i=tab.TabRecords.Count-1;i>0;i--)
                {
                    if (tab.TabRecords.ElementAt(i).Name == "Накладные расходы")
                    {
                        tab.TabRecords.RemoveAt(i);
                    }
                    else if (tab.TabRecords.ElementAt(i).Name == "Расходные материалы")
                    {
                        tab.TabRecords.RemoveAt(i);
                    }
                }
            }
        }
    }
}
