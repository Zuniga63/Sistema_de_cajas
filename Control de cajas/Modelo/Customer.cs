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
                    PoseeSaldo = true;
                }
                else
                {
                    PoseeSaldo = false;
                }
            }
        }

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
                    PoseeAbono = true;
                }
                else
                {
                    PoseeAbono = false;
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

        private DateTime? _cutoffDate;
        /// <summary>
        /// Es la fecha de corte desde el ultimo pago realizado por el cliente o desde la fecha
        /// de la primera factura
        /// </summary>
        public DateTime? CutoffDate
        {
            get { return _cutoffDate; }
            set
            {
                _cutoffDate = value;
                OnPropertyChanged("CutoffDate");
                DefineDaysPastDue();
            }
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

        private bool _state;
        public bool State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged("State"); }
        }

        private bool _poseeMora;
        public bool PoseeMora
        {
            get { return _poseeMora; }
            set { _poseeMora = value; OnPropertyChanged("PoseeMora"); }
        }

        private bool _poseeAbono;
        public bool PoseeAbono
        {
            get { return _poseeAbono; }
            set { _poseeAbono = value; OnPropertyChanged("PoseeAbono"); }
        }

        private bool _poseeSaldo;
        public bool PoseeSaldo
        {
            get { return _poseeSaldo; }
            private set { _poseeSaldo = value; OnPropertyChanged("PoseeSaldo"); }
        }

        public List<CustomerTransaction> Transactions { get; set; }
        public List<PaymentTracking> PaymentsTracking { get; set; }
        public List<CustomerTracking> CustomerTracking { get; set; }

        public Customer(int customerID, int userID, string customerName, string observation,
            string nit, string address, string phone, decimal balance)
        {
            _userID = userID;
            _customerID = customerID;
            _customerName = customerName;
            _observation = observation;
            _nit = nit;
            _address = address;
            _phone = phone;
            _balance = balance;
            Transactions = new List<CustomerTransaction>();
            PaymentsTracking = new List<PaymentTracking>();
            CustomerTracking = new List<CustomerTracking>();
        }

        public void UpdateTransactions(List<CustomerTransaction> transactions)
        {
            //TODO
        }
        /// <summary>
        /// Este metodo determina los dias que se estan en mora
        /// </summary>
        private void DefineDaysPastDue()
        {
            DateTime now = DateTime.Now;

            if (CutoffDate.HasValue && CutoffDate.Value < now)
            {
                Days = (int)now.Subtract(CutoffDate.Value).TotalDays;
                PoseeMora = true;
            }
            else
            {
                Days = 0;
                PoseeMora = false;
            }
            
        }
    }
}
