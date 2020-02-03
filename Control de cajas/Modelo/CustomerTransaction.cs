using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_de_cajas.Modelo
{
    class CustomerTransaction
    {
        private int _id;
        public int ID => _id;

        private DateTime _fecha;
        public DateTime Fecha => _fecha;

        private string _descripcion;
        public string Descriptcion => _descripcion;

        private decimal _deuda;
        public decimal Deuda => _deuda;

        private decimal _abono;
        public decimal Abono => _abono;

        public decimal Saldo { get; set; }

        public CustomerTransaction(int id, DateTime fecha, string description, decimal deuda, decimal abono)
        {
            _id = id;
            _fecha = fecha;
            _descripcion = description;
            _deuda = deuda;
            _abono = abono;
        }
    }
}
