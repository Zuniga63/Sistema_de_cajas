using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Control_de_cajas.Modelo
{
    class Customer : Notificador
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //Datos recuperados directamente de la base de datos
        private int _customerID;
        public int CustomerID => _customerID;

        private int _userID;
        public int UserID => _userID;

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (value != _customerName)
                {
                    _customerName = value;
                    OnPropertyChanged("CustomerName");
                }
            }
        }

        private string _observation;
        public string Observation
        {
            get { return _observation; }
            set
            {
                if(_observation!= value)
                {
                    _observation = value;
                    OnPropertyChanged("Observation");
                }
            }
        }

        private string _nit;
        public string Nit
        {
            get { return _nit; }
            set
            {
                if(_nit!=value)
                {
                    _nit = value;
                    OnPropertyChanged("Nit");
                }
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if(value!=_address)
                {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set
            {
                if(value!=_phone)
                {
                    _phone = value;
                    OnPropertyChanged("Phone");
                }
            }
        }

        private decimal _balance;
        public decimal Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                OnPropertyChanged("Balance");
                if(Balance>0)
                {
                    HasDebt = true;
                }
                else
                {
                    HasDebt = false;
                }
            }
        }

        private decimal _creditLimit;
        /// <summary>
        /// El limite de credito se recupera desde la base de datos pero este es modificado cuando su informacion
        /// es pasada por el sistema puntuacion
        /// </summary>
        public decimal Creditlimit
        {
            get { return _creditLimit; }
            set
            {
                _creditLimit = value;
                OnPropertyChanged("CreditLimit");
            }
        }

        private decimal _creditLimitBasic;
        public decimal CreditlimitBasic => _creditLimitBasic;

        //++++++++++++++++++++++++++++++++++++++++++++
        //Informacion obtenida aplicando logica sobre las transacciones
        private double _points;
        /// <summary>
        /// Es un parametro utilizado para medir la calidad de los clientes del negocio
        /// </summary>
        public double Points
        {
            get { return _points; }
            set { _points = value; OnPropertyChanged("Points"); }
        }

        private DateTime? _lastPaymentDate;
        /// <summary>
        /// Es la fecha del ultimo pago realizado por el clientes, es null cuando solo tiene creditos
        /// </summary>
        public DateTime? LastPaymentDate
        {
            get { return _lastPaymentDate; }
            set { _lastPaymentDate = value; OnPropertyChanged("LastPaymentDate"); }
        }

        private decimal? _lastPayment;
        /// <summary>
        /// Es el monto pagado por el cliente la ultima vez que abonó;
        /// </summary>
        public decimal? LastPayment
        {
            get { return _lastPayment; }
            set
            {
                _lastPayment = value;
                OnPropertyChanged("LastPayment");
                if(value != null)
                {
                    if(value.HasValue && value.Value>0)
                    {
                        HasPayment = true;
                        HasOnlyDebt = false;
                    }
                    else
                    {
                        HasOnlyDebt = true;
                    }
                }
                else
                {
                    HasPayment = false;
                    HasOnlyDebt = true;
                }
            }
        }

        private double? _paymentPercentage;
        //Es el porcentaje de la deuda del clientes que se abonó
        public double? PaymentPercentage
        {
            get { return _paymentPercentage; }
            set { _paymentPercentage = value; OnPropertyChanged("PaymentPercentage"); }
        }

        

        private int _days;
        /// <summary>
        /// Son el numero de dias desde que se efectuo el ultimo abono
        /// </summary>
        public int Days
        {
            get { return _days; }
            set { _days = value; OnPropertyChanged("Days"); }
        }

        private DateTime? _lastDebtDate;
        /// <summary>
        /// Es la fecha de la ultima deuda adquirida
        /// </summary>
        public DateTime? LastDebtDate
        {
            get { return _lastDebtDate; }
            set { _lastDebtDate = value; OnPropertyChanged("LastDebtDate"); }
        }

        private decimal? _lastDebtAmount;
        /// <summary>
        /// El monto de la ultima deuda adquirida
        /// </summary>
        public decimal? LastDebtAmount
        {
            get { return _lastDebtAmount; }
            set { _lastDebtAmount = value; OnPropertyChanged("LastDebtAmount"); }
        }

        private decimal? _averagePayment;
        public decimal? AveragePayment
        {
            get { return _averagePayment; }
            set { _averagePayment = value; OnPropertyChanged("AveragePayment"); }
        }
        
        private bool _hasOnlyDebt;
        public bool HasOnlyDebt
        {
            get { return _hasOnlyDebt; }
            set { _hasOnlyDebt = value; OnPropertyChanged("HasOnlyDebt"); }
        }

        private bool _hasPayment;
        public bool HasPayment
        {
            get { return _hasPayment; }
            set { _hasPayment = value; OnPropertyChanged("HasPayment"); }
        }

        private bool _hasDebt;
        public bool HasDebt
        {
            get { return _hasDebt; }
            private set { _hasDebt = value; OnPropertyChanged("HasDebt"); }
        }

        public List<CustomerTransaction> Transactions { get; set; }
        public List<CustomerTracking> CustomerTracking { get; set; }

        public Customer(int customerID, int userID, string customerName, string observation,
            string nit, string address, string phone, decimal balance, decimal creditLimit)
        {
            _userID = userID;
            _customerID = customerID;
            _customerName = customerName;
            _observation = observation;
            _nit = nit;
            _address = address;
            _phone = phone;
            _balance = balance;
            _creditLimit = creditLimit;
            _creditLimitBasic = creditLimit;

            Transactions = new List<CustomerTransaction>();
            CustomerTracking = new List<CustomerTracking>();
        }

        
    }
}
