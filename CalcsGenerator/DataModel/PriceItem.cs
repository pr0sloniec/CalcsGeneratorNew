using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcsGenerator.DataModel
{
    public class PriceItem : IComparable
    {
        [Key]
        public int Id { get; set; }

        public string Name { set; get; }

        public string Type { set; get; }

        public string Value { set; get; }

        public int CompareTo(object obj)
        {
            return String.Compare((obj as PriceItem).Name, this.Name);
        }
    }
}
