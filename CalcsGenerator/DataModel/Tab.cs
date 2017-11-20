using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CalcsGenerator.DataModel
{

    [DataContract]
    public class Tab
    {
        //Класс, который организует вкладку. Сериализуем

        // [DataContract] - класс сериализации
        // [DataMember] - поле для сериализации

        // [Key] - ключ в бд
        // [Required] - проверка на null
        // [NotMapped] - не добавляется в бд(просчитывается отдельно)

        public Tab()
        {
            if (TabRecords == null) TabRecords = new List<TabRecord>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }

        public int WorkCharge { get; set; } //Стоимость работ
        public int PartsCharge { get; set; } //Стоимость комплектующих

        [DataMember]
        public virtual List<TabRecord> TabRecords { get; set; }
    }
}
