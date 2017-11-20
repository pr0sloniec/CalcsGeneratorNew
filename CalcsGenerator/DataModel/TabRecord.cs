using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CalcsGenerator.DataModel
{

    [DataContract]
    public class TabRecord : IComparable,INotifyPropertyChanged
    {
        //Класс, который организует элемент вкладки

        // [DataContract] - класс сериализации
        // [DataMember] - поле для сериализации

        // [Key] - ключ в бд
        // [Required] - проверка на null
        // [NotMapped] - не добавляется в бд(просчитывается отдельно)

        public TabRecord()
        {
            Number = 0;
        }

        [Key]
        public int Id { get; set; }

        [NotMapped]
        public int Number { get; set; } //Номер для сметы, задается в процессе вывода

        
        string name=string.Empty;
        [MaxLength(50)]
        [Required]
        [DataMember]
        public string Name { get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value)) name = " ";
                else name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        } //Имя, сохраняется в бд

        
        string type=string.Empty;
        [MaxLength(50)]
        [Required]
        [DataMember]
        public string Type { get { return type; }
            set
            {
                if (string.IsNullOrEmpty(value)) type = " ";
                else type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
            }
        } //Единицы измерения

        
        int count=0;
        [Required]
        [DataMember]
        public int Count { get { return count; } set { count = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } } //Единицы измерения

 
        int real=0;
        [Required]
        [DataMember]
        public int Real { get { return real; } set { real = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } } //Реальная цена

        
        bool usn=false;
        [Required]
        [DataMember]
        public bool Usn { get { return usn; } set { usn = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } } //Наценивать ли усн. Влияет на подсчет цены

        
        bool nds = false;
        [Required]
        [DataMember]
        public bool Nds { get { return nds; } set { nds = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } } //Наценивать ли ндс. Влияет на подсчет цены

        
        int charge = 0;
        [Required]
        [DataMember]
        public int Charge { get { return charge; } set { charge = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); } } //Своя наценка


        [NotMapped]
        public int Cost { get { return (int)Math.Floor((double)(real + real * charge / 100 + ((nds) ? real / 100 * 16 : 0) + ((usn) ? real / 100 * 6 : 0))); } }

        [NotMapped]
        public int Price { get { return Cost * count; } }

        public event PropertyChangedEventHandler PropertyChanged;

        //При сортировке не меняем местами элементы с нулевым индексом
        public int CompareTo(object obj)
        {
            var another = obj as TabRecord;
            if (another.Number == 0) return 0;
            if (Number == 0) return 0;

            if (Number > another.Number) return 1;
            if (Number < another.Number) return -1;
            return 0;
        }


        //Если нужно сменить значения с другого метода
        public void ReplaceValues(TabRecord obj)
        {
            name = obj.Name;
            type = obj.Type;
            count = obj.Count;
            usn = obj.Usn;
            nds = obj.Nds;
            charge = obj.Charge;
        }
    }
}
