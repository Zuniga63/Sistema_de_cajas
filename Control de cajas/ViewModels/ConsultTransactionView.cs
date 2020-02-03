using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Control_de_cajas.Modelo;
using System.Collections.ObjectModel;
using Utilidades;

namespace Control_de_cajas.ViewModels
{
    class ConsultTransactionView : ViewModelBase
    {
        private enum TypeOfReport { Daily, Biweekly, Mensual}
        /// <summary>
        /// Este es el usuario al que se le van a hacer las consultas preliminares
        /// </summary>
        private User actualUser;

        private decimal generalDeposit;             //Es la suma de los movimientos positivos
        private decimal generalEgress;              //Es la suma de los movimientos negativos
        private decimal generalBalance;
        private DateTime? firtsTransactionDate;     //La fecha de la primera transaccion
        private DateTime? lastTransactionDate;      //La fecha de la ultima transaccion del usuario

        #region PARAMETROS PARA LA SELECCION DE LA RUTA A CONULTAR

        private List<Category> allCategories;       //Utilizada para guardar en memoria las categorías a utilizar
        private List<Category> categoriesSelected;  //Son las categorias que marcan la ruta a seguir

        public ObservableCollection<Category> Categories { get; private set; }

        private Category _categorySelected;
        /// <summary>
        /// Para seleccionar la categoria de la vista, una vez se ha seleccionado se actualiza el listado de 
        /// ruta y re vuelve aponer en valor nulo esta propiedad
        /// </summary>
        public Category CategorySelected
        {
            get { return _categorySelected; }
            set
            {
                _categorySelected = value;
                OnPropertyChanged("CategorySelected");
                if (value != null)
                {
                    AddCategory();
                }
            }
        }

        private string _categoryRoute;
        public string CategoryRoute
        {
            get { return _categoryRoute; }
            private set
            {
                _categoryRoute = value;
                OnPropertyChanged("CategoryRoute");
            }
        }

        #endregion

        #region SELECCION DEL PERIODO A CONSULTAR
        private bool _allTime;
        public bool AllTime
        {
            get { return _allTime; }
            set
            {
                if (_allTime != value)
                {
                    _allTime = value;

                    if (value)
                    {
                        _definedPeriod = !value;
                        ReloadTransactions();
                    }
                    OnPropertyChanged("AllTime");
                    OnPropertyChanged("DefinedPeriod");
                }
            }
        }

        private bool _definedPeriod;
        public bool DefinedPeriod
        {
            get { return _definedPeriod; }
            set
            {
                if (value != _definedPeriod)
                {
                    _definedPeriod = value;

                    if (value)
                    {
                        _allTime = !value;
                        ReloadTransactions();
                    }

                    OnPropertyChanged("AllTime");
                    OnPropertyChanged("DefinedPeriod");
                }

            }
        }

        private DateTime? _sinceDateStart;
        public DateTime? SinceDateStart
        {
            get { return _sinceDateStart; }
            set
            {
                _sinceDateStart = value;
            }
        }

        private DateTime? _sinceDateEnd;
        public DateTime? SinceDateEnd
        {
            get { return _sinceDateEnd; }
            set
            {
                _sinceDateEnd = value;
            }
        }

        private DateTime? _sinceDateSelected;
        public DateTime? SinceDateSelected
        {
            get { return _sinceDateSelected; }
            set
            {
                _sinceDateSelected = value;
                ValidateDate();

                if (DefinedPeriod && value.HasValue)
                {
                    ReloadTransactions();
                }
            }
        }

        private DateTime? _untilDateStart;
        public DateTime? UntilDateStart
        {
            get { return _untilDateStart; }
            set
            {
                _untilDateStart = value;
            }
        }

        private DateTime? _untilDateEnd;
        public DateTime? UntilDateEnd
        {
            get { return _untilDateEnd; }
            set
            {
                _untilDateEnd = value;
            }
        }

        private DateTime? _untilDateSelected;
        public DateTime? UntilDateSelected
        {
            get { return _untilDateSelected; }
            set
            {
                _untilDateSelected = value;
                ValidateDate();

                if (DefinedPeriod && value.HasValue)
                {
                    ReloadTransactions();
                }
            }
        }

        #endregion

        private decimal _deposits;
        public decimal Deposits
        {
            get { return _deposits; }
            private set { _deposits = value; OnPropertyChanged("Deposits"); }
        }

        private decimal _egress;
        public decimal Egress
        {
            get { return _egress; }
            private set { _egress = value; OnPropertyChanged("Egress"); }
        }

        private decimal _balance;
        public decimal Balance
        {
            get { return _balance; }
            private set { _balance = value; OnPropertyChanged("Balance"); }
        }

        public ObservableCollection<CashboxTransaction> Transactions { get; set; }
        public ObservableCollection<Report> Reports { get; set; }

        private bool _copyId;
        public bool CopyId
        {
            get { return _copyId; }
            set { _copyId = value; OnPropertyChanged("CopyId"); }
        }

        private bool _copyDate;
        public bool CopyDate
        {
            get { return _copyDate; }
            set { _copyDate = value; OnPropertyChanged("CopyDate"); }
        }

        private bool _copyDescription;
        public bool CopyDescription
        {
            get { return _copyDescription; }
            set { _copyDescription = value; OnPropertyChanged("CopyDescription"); }
        }

        private bool _copyAmount;
        public bool CopyAmount
        {
            get { return _copyAmount; }
            set { _copyAmount = value; OnPropertyChanged("CopyAmount"); }
        }

        private bool _dailyReport;
        public bool DailyReport
        {
            get { return _dailyReport; }
            set
            {
                _dailyReport = value;
                OnPropertyChanged("DailyReport");
                UpdateReports();
            }
        }

        private bool _biweeklyReport;
        public bool BiweeklyReport
        {
            get { return _biweeklyReport; }
            set
            {
                _biweeklyReport = value;
                OnPropertyChanged("BiweeklyReport");
                UpdateReports();
            }
        }

        private bool _mensualReport;
        public bool MensualReport
        {
            get { return _mensualReport; }
            set
            {
                _mensualReport = value;
                OnPropertyChanged("MensualReport");
                UpdateReports();
            }
        }


        public Command RemoveCategoryCmd { get; private set; }

        /// <summary>
        /// Este metodo inicializa las listas de la clase al ser construida
        /// </summary>
        private void InitializeList()
        {
            allCategories = new List<Category>();
            categoriesSelected = new List<Category>();
            Categories = new ObservableCollection<Category>();
            Transactions = new ObservableCollection<CashboxTransaction>();
            Reports = new ObservableCollection<Report>();
        }

        private void InitializeCommand()
        {
            RemoveCategoryCmd = new Command(RemoveCategory, () => categoriesSelected.Count > 0);
        }

        public ConsultTransactionView()
        {
            InitializeList();
            InitializeCommand();
            DailyReport = true;
        }

        public void DefinedUser(User user)
        {
            actualUser = user;
            LoadCategories();

            UpdateParameters();
            ReloadTransactions();

            
            
        }

        private void UpdateParameters()
        {
            if (actualUser != null)
            {
                BDComun.MetaDataConsult(actualUser.ID, out firtsTransactionDate, out lastTransactionDate,
                    out generalBalance, out generalDeposit, out generalEgress);
            }
            else
            {
                firtsTransactionDate = null;
                lastTransactionDate = null;
                generalBalance = 0m;
                generalDeposit = 0m;
                generalEgress = 0m;
            }

            ValidateDate();
        }

        #region METODOS QUE MANIPULAN TRANSACCIONES Y CATEGORÍAS

        /// <summary>
        /// Recarga las categorias y limpia las listas
        /// </summary>
        private void LoadCategories()
        {
            categoriesSelected.Clear();
            Categories.Clear();

            if(actualUser != null)
            {
                CategorySystem.DefineParameter(actualUser.ID);
                foreach (Category c in CategorySystem.GetFatherCategories())
                {
                    Categories.Add(c);
                }
            }
            else
            {
                CategorySystem.DefineParameter(null);
            }

            WriteRoute();
        }

        /// <summary>
        /// Agrega la categoría seleccionada al listado con la ruta de transacciones a seguir y
        /// luego actualiza el listado de categorías para que sean las subcategorias de la ultima seleccion
        /// </summary>
        private void AddCategory()
        {
            Category actualCategory = CategorySelected;

            categoriesSelected.Add(CategorySelected);
            Categories.Clear();
            WriteRoute();
            ReloadTransactions();

            foreach (Category c in CategorySystem.RecoverySubcategories(actualCategory))
            {
                Categories.Add(c);
            }
        }

        /// <summary>
        /// Elimina la ultima categoría del arbol de ruta y reescibe la ruta que es consumida por la vista
        /// </summary>
        private void RemoveCategory()
        {
            int count = categoriesSelected.Count;
            Categories.Clear();

            categoriesSelected.RemoveAt(count - 1);
            count--;

            WriteRoute();

            if (count == 0)
            {
                foreach (Category c in CategorySystem.RecoveryMainCategory())
                {
                    Categories.Add(c);
                }
            }
            else
            {
                Category lastCategory = categoriesSelected[count - 1];
                foreach (Category c in CategorySystem.RecoverySubcategories(lastCategory))
                {
                    Categories.Add(c);
                }
            }

            ReloadTransactions();
        }

        /// <summary>
        /// Escribe la ruta en una cadena de texto, que luego es consumida por la view
        /// </summary>
        private void WriteRoute()
        {
            string result = "";
            bool firtsCategory = true;

            foreach(Category c in categoriesSelected)
            {
                if(firtsCategory)
                {
                    result += string.Format("{0} ({1})", c.Name, c.CategoryClass);
                    firtsCategory = false;
                }
                else
                {
                    result += string.Format(" --> {0} ({1})", c.Name, c.CategoryClass);
                }
            }

            CategoryRoute = result;
        }

        /// <summary>
        /// Este metodo consulta a la base de datos las transacciones segun el arbol de ruta especificado
        /// </summary>
        private void ReloadTransactions()
        {
            Transactions.Clear();
            Deposits = 0m;
            Egress = 0m;
            Balance = 0m;
            UpdateParameters();            //Con el objetivo de utilizar datos actualizados

            //Este es el primer filtro, con esto evito estar repitiendo codigo por todas partes
            if (categoriesSelected.Count == 0)
            {
                Deposits = generalDeposit;
                Egress = generalEgress;
                Balance = generalBalance;
            }
            else if(firtsTransactionDate.HasValue)
            {
                List<CashboxTransaction> temporalList = new List<CashboxTransaction>();
                DateTime since = firtsTransactionDate.Value;
                DateTime until = lastTransactionDate.Value;

                if(DefinedPeriod)
                {
                    if (SinceDateSelected.HasValue && UntilDateSelected.HasValue)
                    {
                        since = SinceDateSelected.Value;
                        until = UntilDateSelected.Value;
                    }
                    else if( SinceDateSelected.HasValue && !UntilDateSelected.HasValue)
                    {
                        since = SinceDateSelected.Value;
                    }
                    else if(!SinceDateSelected.HasValue && UntilDateSelected.HasValue)
                    {
                        until = UntilDateSelected.Value;
                    }
                }

                temporalList = BDComun.RecoverTransaction(categoriesSelected, since, until);

                foreach (CashboxTransaction t in temporalList)
                {
                    if (t.Amount > 0)
                    {
                        Deposits += t.Amount;
                    }
                    else
                    {
                        Egress += Math.Abs(t.Amount);
                    }

                    Balance += t.Amount;

                    Transactions.Add(t);
                }
            }

            UpdateReports();
        }

        #endregion

        private void ValidateDate()
        {
            //Primero se establece los limites de las fechas
            if (firtsTransactionDate.HasValue)
            {
                _sinceDateStart = firtsTransactionDate;
                _sinceDateEnd = lastTransactionDate;
                _untilDateStart = firtsTransactionDate;
                _untilDateEnd = lastTransactionDate;

                //Ahora se valída la primera fecha
                if(_sinceDateSelected.HasValue)
                {
                    if(_sinceDateSelected<_sinceDateStart)
                    {
                        _sinceDateSelected = _sinceDateStart;
                    }
                    else if(_sinceDateSelected > _sinceDateEnd)
                    {
                        _sinceDateSelected = _sinceDateEnd;
                    }

                    _untilDateStart = _sinceDateSelected;

                    if (_untilDateSelected.HasValue && _untilDateSelected < _sinceDateSelected)
                    {
                        _untilDateSelected = _sinceDateSelected;
                        _sinceDateEnd = _sinceDateSelected;
                    }
                }

                if(_untilDateSelected.HasValue)
                {
                    if(_untilDateSelected < _untilDateStart)
                    {
                        _untilDateSelected = _untilDateStart;
                    }
                    else if(_untilDateSelected > _untilDateEnd)
                    {
                        _untilDateSelected = _untilDateEnd;
                    }

                    _sinceDateEnd = _untilDateSelected;

                    if(_sinceDateSelected.HasValue && _sinceDateSelected>_untilDateSelected)
                    {
                        _sinceDateSelected = _untilDateSelected;
                    }
                }


            }
            else
            {
                _sinceDateStart = null;
                _sinceDateEnd = null;
                _untilDateStart = null;
                _untilDateEnd = null;
            }

            OnPropertyChanged("SinceDateStart");
            OnPropertyChanged("SinceDateEnd");
            OnPropertyChanged("SinceDateSelected");
            OnPropertyChanged("UntilDateStart");
            OnPropertyChanged("UntilDateEnd");
            OnPropertyChanged("UntilDateSelected");
        }

        private void UpdateReports()
        {
            Reports.Clear();
            DateTime? startDate = null;
            DateTime? endDate = null;
            DateTime? currentDate = null;

            decimal revenue = 0m;
            decimal expenses = 0m;
            decimal partialBalance = 0m;
            decimal totalBalance = 0m;
            int id = 0;
            Report report = null;

            if (Transactions.Count > 0)
            {
                startDate = Transactions[0].TransactionDate;
                endDate = Transactions[Transactions.Count - 1].TransactionDate;
                currentDate = startDate;

                if (DailyReport)
                {
                    foreach(CashboxTransaction t in Transactions)
                    {
                        if(currentDate.Value == t.TransactionDate)
                        {
                            if (t.Amount > 0)
                            {
                                revenue += t.Amount;
                            }
                            else
                            {
                                expenses += t.Amount;
                            }

                            partialBalance += t.Amount;
                            totalBalance += t.Amount;
                        }
                        else
                        {
                            id++;
                            report = new Report
                            {
                                Id = id,
                                StartDate = currentDate.Value,
                                EndDate = currentDate.Value,
                                Revenue = revenue,
                                Expenses = expenses,
                                PartialBalance = partialBalance,
                                TotalBalance = totalBalance
                            };

                            Reports.Add(report);

                            currentDate = t.TransactionDate;
                            revenue = t.Amount>0 ? t.Amount : 0m;
                            expenses = t.Amount < 0 ? t.Amount : 0m;
                            partialBalance = t.Amount;
                            totalBalance += t.Amount;
                        }
                    }

                    //Finalmente se crea el ultimo reporte
                    id++;
                    report = new Report
                    {
                        Id = id,
                        StartDate = currentDate.Value,
                        EndDate = currentDate.Value,
                        Revenue = revenue,
                        Expenses = expenses,
                        PartialBalance = partialBalance,
                        TotalBalance = totalBalance
                    };

                    Reports.Add(report);
                }
            }
        }
    }
}
