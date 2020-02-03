using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Control_de_cajas.Modelo
{
    class CashboxTransaction
    {
        private int _id;
        public int ID => _id;

        private DateTime _transactionDate;
        public DateTime TransactionDate => _transactionDate;

        private string _description;
        public string Description => _description;

        private decimal _amount;
        public decimal Amount => _amount;

        private bool _isAtransfer;
        public bool IsATransfer => _isAtransfer;

        public CashboxTransaction(int id, DateTime tDate, string description, decimal amount, bool isATransfer)
        {
            _id = id;
            _transactionDate = tDate;
            _description = description;
            _amount = amount;
            _isAtransfer = isATransfer;
        }
    }
}
