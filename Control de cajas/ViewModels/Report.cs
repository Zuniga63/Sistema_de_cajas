using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_de_cajas.ViewModels
{
    class Report
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal PartialBalance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
