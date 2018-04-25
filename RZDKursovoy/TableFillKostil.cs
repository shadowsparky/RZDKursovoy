using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZDKursovoy
{
    class TableFillKostil
    {
        public TableFillKostil(string Par1, string Par2, string Par3, string Par4, string Par5, string Par6)
        {
            this.Par1 = Par1;
            this.Par2 = Par2;
            this.Par3 = Par3;
            this.Par4 = Par4;
            this.Par5 = Par5;
            this.Par6 = Par6;
        }

        [DisplayName("Номер поезда")]
        public string Par1 { get; set; }
        [DisplayName("Время отправления")]
        public string Par2 { get; set; }
        [DisplayName("Время прибытия")]
        public string Par3 { get; set; }
        [DisplayName("Дата прибытия")]
        public string Par4 { get; set; }
        [DisplayName("Начальная остановка")]
        public string Par5 { get; set; }
        [DisplayName("Конечная остановка")]
        public string Par6 { get; set; }
    }
}
