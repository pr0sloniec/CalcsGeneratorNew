using CalcsGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CalcsGenerator.Windows;
using CalcsGenerator.Controls;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace CalcsGenerator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        static readonly Object lockobj = new Object();
        static readonly ProjectsContext projectsContext = new ProjectsContext();
        public static ProjectsContext PC
        {
            get
            {
                lock (lockobj)
                {
                    return projectsContext;
                }
            }
        }

        //Если сохранение на каждом шаге будут медленными, их можно будет выключить, если вызывать через метод
        public static bool TrySaveChanges()
        {
            lock (lockobj)
            {
                try
                {
                    projectsContext.SaveChanges();
                    Console.WriteLine("Сохранение в {0}", DateTime.Now);
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("\n|Сохранение сущности в базу пропущено: {0}", e.Message);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("|----Свойство: \"{0}\", Предупреждение: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    return false;
                }
                
            }
        }

        public static void AddItems()
        {
            {
                Project p = new Project();
                p.Name = "Тестовый проект";

                Tab tab = new Tab();
                tab.Name = "Вкладка";

                TabRecord tmp = new TabRecord();
                tmp.Name = "Лолка";
                tmp.Type = "Шт";
                tmp.Count = 1;
                tmp.Real = 1488;
                tmp.Charge = 10;
                tmp.Usn = true;

                tab.TabRecords.Add(tmp);

                p.Tabs.Add(tab);

                projectsContext.Projects.Add(p);
                projectsContext.SaveChanges();
            }
        }

        //Этот метод должен читать консоль и выполнять команды
        private static void RunConsole(Action rungui)
        {
            Console.WriteLine("CalcsGenerator");
            string thisname = Process.GetCurrentProcess().ProcessName;
            Console.WriteLine("Название текущего процесса: {0}",thisname);

            if (Process.GetProcessesByName(thisname).Count() > 1)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.WriteLine("Найден другой процесс с названием {0}", thisname);
                Console.WriteLine("Не допускается запуск более одной копии процесса, обрабатываюшего базу данных");
                Console.WriteLine("Нажмите \"S\" чтобы автоматически закрыть процесс и запуститься");
                Console.WriteLine("Нажмите любую другую кнопку для выхода");

                ConsoleKeyInfo key = Console.ReadKey();
                var procarr=Process.GetProcessesByName(thisname);
                if (key.Key==ConsoleKey.S)
                {
                    int thisid= Process.GetCurrentProcess().Id;
                    foreach(var proc in procarr)
                    {
                        if (proc.Id != thisid) proc.Kill();
                    }
                }
                else
                {
                    App.Current.Shutdown();
                    return;
                }
            }

            Task.Run(()=>App.Current.Dispatcher.Invoke(()=>
            {
                try
                {
                    rungui();
                }
                catch(Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Произошла ошибка исполнения!");
                    Console.WriteLine(ex.Message);
                }
            }));

            Task.Run(() =>
            {
                while (true)
                {
                    Console.Write("\n>");
                    string command = Console.ReadLine();

                    //Показать дерево проектов
                    if (command == "showtree")
                    {
                        AppConsole.ShowTree();
                    }
                    else if (command == "dropdb")
                    {
                        PC.Database.ExecuteSqlCommand("drop table dbo.PriceItems");
                        PC.Database.ExecuteSqlCommand("drop table dbo.TabRecords");
                        PC.Database.ExecuteSqlCommand("drop table dbo.Tabs"); 
                        PC.Database.ExecuteSqlCommand("drop table dbo.Projects");
                        PC.Database.ExecuteSqlCommand("drop table dbo.__MigrationHistory");
                        AppConsole.Restart();
                    }
                    else
                    {
                        Console.WriteLine("Комманда не найдена!");
                    }
                }


            });
        }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /*Project p = new Project();
            p.Name = "lolka";
            projectsContext.Projects.Add(p);
            projectsContext.SaveChanges();

            ChooseProject cp = new ChooseProject();
            cp.ShowDialog();*/

            //AddItems();


            RunConsole(() =>
            {
                ChooseProject cp = new ChooseProject();
                cp.ShowDialog();
            });

            
        }
    }
}
