using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CalcsGenerator.DataModel
{
    //Класс проекта. Хранит название и список вкладок. Сохраняется в бд, но для экспорта сериализуем

    // [DataContract] - класс сериализации
    // [DataMember] - поле для сериализации

    // [Key] - ключ в бд
    // [Required] - проверка на null
    // [NotMapped] - не добавляется в бд(просчитывается отдельно)

    [DataContract]
    public class Project
    {
        public Project()
        {
            if (Tabs == null) Tabs = new List<Tab>();
        }


        [Key]
        public int Id { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Info { get; set; }

        [DataMember]
        public virtual List<Tab> Tabs { get; set; }

        //Самоуничтожение для удаления из бд
        public static void SelfDestruct(Project tmp)
        {
            foreach (var tab in tmp.Tabs)
            {
                while (tab.TabRecords.Count > 0)
                {
                    tab.TabRecords.RemoveAt(tab.TabRecords.Count - 1);
                }
            }

            while (tmp.Tabs.Count > 0)
            {
                tmp.Tabs.RemoveAt(tmp.Tabs.Count - 1);
            }
        }
    }
}
