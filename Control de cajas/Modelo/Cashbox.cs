﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Control_de_cajas.Modelo
{
    class Cashbox:Notificador
    {
        private int _id;
        public int ID => _id;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private string _boxName;
        public string BoxName
        {
            get { return _boxName; }
            set { _boxName = value; OnPropertyChanged("BoxName"); }
        }

        public decimal _balance;
        public decimal Balance
        {
            get { return _balance; }
            set { _balance = value; OnPropertyChanged("Balance"); }
        }

        public Cashbox(int id, string name, decimal balance)
        {
            _id = id;
            _boxName = name;
            _balance = balance;
        }

        public override string ToString()
        {
            return BoxName;
        }

    }
}
