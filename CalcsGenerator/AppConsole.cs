using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
