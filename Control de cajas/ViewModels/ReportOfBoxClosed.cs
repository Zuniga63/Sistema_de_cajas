using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_de_cajas.ViewModels
{
    class ReportOfBoxClosed
    {
        private string _category;
        public string Category => _category;

        private int _cant;
        public int Cant => _cant;

        private decimal _amout;
        public decimal Amount => _amout;

        public ReportOfBoxClosed (string name, int cant, decimal amount)
        {
            _category = name;
            _cant = cant;
            _amout = amount;
        }
    }
}
