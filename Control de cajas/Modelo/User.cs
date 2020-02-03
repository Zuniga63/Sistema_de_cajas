using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Control_de_cajas.Modelo
{
    class User:Notificador
    {
        private int _id;
        public int ID => _id;

        private DateTime _createDate;
        public DateTime CreateDate => _createDate;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private string _userName;
        /// <summary>
        /// Es el nombre del usuario en la base de datos
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set
            {
                if(value != _userName)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }
        
        private int _boxs;
        /// <summary>
        /// Es el numero de cajas del usaurio en la base de datos
        /// </summary>
        public int Boxs
        {
            get { return _boxs; }
            set { _boxs = value; OnPropertyChanged("Boxs"); }
        }

        private decimal _cashBalances;
        /// <summary>
        /// Es la suma de todos los saldos de las cajas del usuario en la base de datos
        /// </summary>
        public decimal CashBalances
        {
            get { return _cashBalances; }
            set { _cashBalances = value; OnPropertyChanged("CashBalances"); }
        }

        private int _customers;
        /// <summary>
        /// Es el numero de clientes del usuario en la base de datos
        /// </summary>
        public int Customers
        {
            get { return _customers; }
            set { _customers = value; OnPropertyChanged("Customers"); }
        }

        private decimal _customerBalances;
        /// <summary>
        /// Es la suma de los saldos de los clientes asociados al usuario en la base de datos
        /// </summary>
        public decimal CustomerBalances
        {
            get { return _customerBalances; }
            set { _customerBalances = value; OnPropertyChanged("CustomerBalances"); }
        }

        public User(int id, string userName, DateTime createDate)
        {
            _id = id;
            _userName = userName;
            _createDate = createDate;
        }

        /// <summary>
        /// Este metodo consulta a la base de datos y actualiza sus campos
        /// </summary>
        public void Update()
        {
            BDComun.ReloadUser(this);
        }

        public override string ToString()
        {
            return UserName;
        }
    }
}
