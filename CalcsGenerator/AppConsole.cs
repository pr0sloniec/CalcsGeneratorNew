using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CalcsGenerator.DataModel;

namespace CalcsGenerator
{
    static class AppConsole
    {
        public static void ShowTree()
        {
            foreach (var project in App.PC.Projects)
            {
                Console.WriteLine("\n|Проект " + project.Name + " Идентификатор " + project.Id);
                foreach (var tab in project.Tabs)
                {
                    Console.WriteLine(@"|----Вкладка " + tab.Name);
                    foreach (var item in tab.TabRecords)
                    {
                        Console.WriteLine(@"|--------Элемент " + item.Name + " Цена " + item.Price);
                    }
                }
            }
        }

        public static void Restart()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
                Process.GetCurrentProcess().Kill();
            });
        }

        public static List<PriceItem> GetPriceList()
        {
            List<PriceItem> tmp = new List<PriceItem>();
            foreach(var item in App.PC.Items)
            {
                tmp.Add(item);
            }
            return tmp;
        }
    }
}
