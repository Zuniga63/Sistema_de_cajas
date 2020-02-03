using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Control_de_cajas.Modelo;
using Utilidades;

namespace Control_de_cajas.ViewModels
{
    class ViewModifyTransaction:ViewModelBase
    {
        private CashboxTransaction originalTransaction;                        //Es la transaccion que se desea modificar
        private List<CashboxTransaction> allTransactions;           //Es el litado que se modifica segun los cambios
        private List<CashboxTransaction> temporalTransactions;
        private List<string> observations;
        private int actualIndex;                                    //Es la ubicacion actual de la transaccion a modificar

        private string _observatiosString;
        public string ObservationsString
        {
            get { return _observatiosString; }
            private set { _observatiosString = value; OnPropertyChanged("ObservationsString"); }
        }

        public bool State { get; private set; }                     //Determina si los cambios son correctos o no

        private bool _updateDate;
        public bool UpdateDate
        {
            get { return _updateDate; }
            set
            {
                _updateDate = value;
                OnPropertyChanged("UpdateDate");
            }
        }

        private bool _updateDescription;
        public bool UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; OnPropertyChanged("UpdateDescription"); }
        }

        private bool _updateAmount;
        public bool UpdateAmount
        {
            get { return _updateAmount; }
            set { _updateAmount = value; OnPropertyChanged("UpdateAmount"); }
        }

        private DateTime? _displayDateEnd;
        public DateTime? DisplayDateEnd
        {
            get { return _displayDateEnd; }
            set { _displayDateEnd = value; OnPropertyChanged("DisplayDateEnd"); }
        }

        private DateTime? _transactionDate;
        public DateTime? TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; OnPropertyChanged("TransactionDate"); }
        }

        private string _errorInTransactionDate;
        public string ErrorInTransactionDate
        {
            get { return _errorInTransactionDate; }
            private set { _errorInTransactionDate = value; OnPropertyChanged("ErrorInTransactionDate"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }

        private string _errorInDescription;
        public string ErrorInDescription
        {
            get { return _errorInDescription; }
            private set { _errorInDescription = value; OnPropertyChanged("ErrorInDescription"); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged("Amount"); }
        }

        private string _errorInAmount;
        public string ErrorInAmount
        {
            get { return _errorInAmount; }
            private set { _errorInAmount = value; OnPropertyChanged("ErrorInAmount"); }
        }

        public ViewModifyTransaction()
        {
            DisplayDateEnd = DateTime.Now;
            allTransactions = new List<CashboxTransaction>();
            temporalTransactions = new List<CashboxTransaction>();
            observations = new List<string>();
        }

        public void DefinedParameter(List<CashboxTransaction> transactions, CashboxTransaction transactionToModify)
        {
            originalTransaction = transactionToModify;
            allTransactions.Clear();
            decimal balance = 0m;

            TransactionDate = originalTransaction.TransactionDate;
            Description = originalTransaction.Description;
            Amount = originalTransaction.Amount;

            UpdateAmount = false;
            UpdateDate = false;
            UpdateDescription = false;

            ErrorInAmount = null;
            ErrorInDescription = null;
            ErrorInTransactionDate = null;

            foreach(CashboxTransaction cT in transactions)
            {
                int id = cT.ID;
                DateTime transactionDate = cT.TransactionDate;
                string description = cT.Description;
                decimal amount = cT.Amount;

                CashboxTransaction transaction = new CashboxTransaction(id, transactionDate, description, amount, cT.IsATransfer);
                allTransactions.Add(transaction);
                balance += amount;

                if(balance<0)
                {
                    string observation = string.Format("En la fecha {0:dd-MM-yy} existe flujo de caja negativo", TransactionDate);
                    State = false;
                    observations.Add(observation);
                }

                if(id == originalTransaction.ID)
                {
                    actualIndex = allTransactions.Count - 1;
                }
            }

            WriteObservations();
        }

        private void ValidateChanged()
        {
            decimal balance = 0m;
            State = true;
            bool sumado = false;                //Para definir si el saldo actual ya se sumo al balance
            observations.Clear();

            foreach(CashboxTransaction t in allTransactions)
            {
                if(t.ID != originalTransaction.ID)
                {
                    if(t.TransactionDate == TransactionDate.Value && !sumado && Amount>0)
                    {
                        balance += Amount;
                        balance += t.Amount;
                        sumado = true;
                    }
                    else if(t.TransactionDate == TransactionDate.Value && !sumado && t.Amount<0)
                    {
                        balance += Amount;
                        balance += t.Amount;
                        sumado = true;
                    }
                    else if(t.TransactionDate > TransactionDate.Value && !sumado)
                    {
                        balance += Amount;
                        balance += t.Amount;
                        sumado = true;
                    }
                    else
                    {
                        balance += t.Amount;
                    }

                    if (balance < 0)
                    {
                        string o = string.Format("Se crea saldo negativo en: {0:dd-MM-yy}", t.TransactionDate);
                        observations.Add(o);
                        State = false;
                    }
                    
                }
            }

            if(!sumado)
            {
                balance += Amount;
                if (balance < 0)
                {
                    string o = string.Format("Se crea saldo negativo en: {0:dd-MM-yy}", TransactionDate);
                    observations.Add(o);
                    State = false;
                }
            }

            WriteObservations();

        }

        private bool ValidateParameters()
        {
            ErrorInTransactionDate = null;
            ErrorInDescription = null;
            ErrorInAmount = null;

            bool result = true;

            if(UpdateAmount || UpdateDate || UpdateDescription)
            {
                if (TransactionDate.HasValue)
                {
                    if (UpdateDate && TransactionDate.Value == originalTransaction.TransactionDate)
                    {
                        ErrorInTransactionDate = "La fecha no ha cambiado";
                        result = false;
                    }
                    else if (TransactionDate.Value > DateTime.Now)
                    {
                        ErrorInTransactionDate = "Se debe elegir una fecha valida";
                        result = false;
                    }
                }
                else if (UpdateDate)
                {
                    ErrorInTransactionDate = "Se debe elegir una fecha valida";
                    result = false;
                }

                if (UpdateDescription)
                {
                    if (Description.ToUpper() == originalTransaction.Description.ToUpper())
                    {
                        ErrorInDescription = "La descripción no ha cambiado";
                        result = false;
                    }
                    else if (string.IsNullOrEmpty(Description) || string.IsNullOrWhiteSpace(Description))
                    {
                        ErrorInDescription = "Este campo no puede estar en blanco";
                        result = false;
                    }
                }

                if (UpdateAmount)
                {
                    if (Amount == 0)
                    {
                        ErrorInAmount = "El valor de la transaccion no puede ser cero";
                        result = false;
                    }
                    else if (Amount == originalTransaction.Amount)
                    {
                        ErrorInAmount = "El valor de la transaccion no ha cambiado";
                        result = false;
                    }
                }
            }
            else
            {
                observations.Clear();
                observations.Add("No se realizado ningun cambio");
                WriteObservations();
                return false;
            }

            

            return result;
        }

        private void WriteObservations()
        {
            ObservationsString = null;

            foreach(string linea in observations)
            {
                ObservationsString += linea + "\n";
            }
        }

        public bool UpdateTransactionInBD()
        {
            if(ValidateParameters())
            {
                ValidateChanged();
                if(State)
                {
                    return BDComun.UpdateTransaction(originalTransaction, TransactionDate.Value, Description, Amount);
                }
            }

            return false;
        }
    }
}
