using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Control_de_cajas.Modelo;
using Utilidades;
using System.Collections.ObjectModel;

namespace Control_de_cajas.ViewModels
{
    class ViewClientes:ViewModelBase
    {
        //Listado de clientes sin excluir y segun el usuario seleccionado
        private List<Customer> allCustomer;

        private enum AuxiliarView { NewClient, NewDebt, NewPayment}              //Para identificar las vista auxiliares 

        private bool _isSelected;
        /// <summary>
        /// Esta propiedad va a recargar los usuarios cada vez que se visite esta vista
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
                if(value)
                {
                    ReloadUsers();
                }
            }
        }

        private bool _isEnabled;
        /// <summary>
        /// Esta propiedad habilita o deshabilita la seccion principal
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            private set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        private bool _newCustomerAux;
        public bool NewCustomerAux
        {
            get { return _newCustomerAux; }
            set { _newCustomerAux = value; OnPropertyChanged("NewCustomerAux"); }
        }

        private bool _newDebtAux;
        public bool NewDebtAux
        {
            get { return _newDebtAux; }
            set { _newDebtAux = value; OnPropertyChanged("NewDebtAux"); }
        }

        private bool _newPaymentAux;
        public bool NewpaymentAux
        {
            get { return _newPaymentAux; }
            set { _newPaymentAux = value; OnPropertyChanged("NewpaymentAux"); }
        }

        private DateTime? _maxDate;
        public DateTime? MaxDate { get; set; }

        /// <summary>
        /// Listado de usuarios en la base de datos
        /// </summary>
        public ObservableCollection<User> Users { get; set; }
        
        private User _userSelected;
        /// <summary>
        /// Es el usuario seleccionado de la lista de usuarios
        /// </summary>
        public User UserSelected
        {
            get { return _userSelected; }
            set
            {
                _userSelected = value;
                OnPropertyChanged("UserSelected");
                ReloadCustomers();
                DateCustomerUpdateConsult = DateTime.Now;
            }
        }

        private string _searchByName;
        /// <summary>
        /// Cadena de etxto que es utilizada para poder seleccionar los clientes de la lista de clientes
        /// </summary>
        public string SearchByName
        {
            get { return _searchByName; }
            set
            {
                _searchByName = value;
                OnPropertyChanged("SearchByName");
                SearchCustomer(value);
            }
        }

        /// <summary>
        /// Listado de clientes segun el usuario seleccionado y segun la cadena de busqueda por nombre
        /// </summary>
        public ObservableCollection<Customer> Customers { get; set; }

        private Customer _customerSelected;
        /// <summary>
        /// Es el clientes seleccionado de la lista de clientes
        /// </summary>
        public Customer CustomerSelected
        {
            get { return _customerSelected; }
            set
            {
                _customerSelected = value;
                OnPropertyChanged("CustomerSelected");
                UpdateCustomerProperty(value);
            }
        }

        private bool _alphabeticalOrder;
        /// <summary>
        /// Propiedad que permite ordenar los clientes por orden alfabetico
        /// </summary>
        public bool AlfabeticalOrder
        {
            get { return _alphabeticalOrder; }
            set
            {
                _alphabeticalOrder = value;
                OnPropertyChanged("AlfabeticalOrder");
                if(value)
                {
                    _entryOrder = !value;
                    SearchCustomer(_searchByName);
                }
            }
        }

        private bool _entryOrder;
        /// <summary>
        /// Propiedad que permite ordena los clientes por orden de ingreso a la base de datos
        /// </summary>
        public bool EntryOrder
        {
            get { return _entryOrder; }
            set
            {
                _entryOrder = value;
                OnPropertyChanged("EntryOrder");
                if(value)
                {
                    _alphabeticalOrder = !value;
                    SearchCustomer(_searchByName);
                }
            }
        }

        private bool _sortByDaysOfDelay;
        /// <summary>
        /// Permite cribar los clientes segun el dia de mora
        /// </summary>
        public bool SortByDaysOfDelay
        {
            get { return _sortByDaysOfDelay; }
            set
            {
                _sortByDaysOfDelay = value;
                OnPropertyChanged("SortByDaysOfDelay");
                DayOfDelay = 0;
            }
        }

        private int _daysOfDelay;
        /// <summary>
        /// Son los días que se utilizan para cribar los clientes
        /// </summary>
        public int DayOfDelay
        {
            get { return _daysOfDelay; }
            set
            {
                _daysOfDelay = value;
                OnPropertyChanged("DayOfDelay");
                SearchCustomer(_searchByName);
            }
        }

        public bool _sortByPoints;
        public bool SortByPoint
        {
            get { return _sortByPoints; }
            set
            {
                _sortByPoints = value;
                OnPropertyChanged("SortByPoint");
                SearchCustomer(_searchByName);
            }
        }

        public Command ConsultTransactionsCmd { get; private set; }

        public Command DeleteCustomerCmd { get; private set; }



        #region PROPIEDADES PARA CREAR UN NUEVO CLIENTE
        private string _newCustomerName;
        /// <summary>
        /// Es el nombre del nuevo cliente
        /// </summary>
        public string NewCustomerName
        {
            get { return _newCustomerName; }
            set { _newCustomerName = value; OnPropertyChanged("NewCustomerName"); }
        }

        private string _errorNewCustomerName;
        public string ErrorNewCustomerName
        {
            get { return _errorNewCustomerName; }
            private set { _errorNewCustomerName = value; OnPropertyChanged("ErrorNewCustomerName"); }
        }

        private string _newCustomerNit;
        public string NewCustomerNit
        {
            get { return _newCustomerNit; }
            set { _newCustomerNit = value; OnPropertyChanged("NewCustomerNit"); }
        }

        private string _errorNewCustomerNit;
        public string ErrorNewCustomerNit
        {
            get { return _errorNewCustomerNit; }
            private set { _errorNewCustomerNit = value; OnPropertyChanged("ErrorNewCustomerNit"); }
        }

        private string _newCustomerAddress;
        public string NewCustomerAddress
        {
            get { return _newCustomerAddress; }
            set { _newCustomerAddress = value; OnPropertyChanged("NewCustomerAddress"); }
        }

        private string _newCustomerPhone;
        public string NewCustomerPhone
        {
            get { return _newCustomerPhone; }
            set { _newCustomerPhone = value; OnPropertyChanged("NewCustomerPhone"); }
        }

        private string _newCustomerObservation;
        public string NewCustomerObservation
        {
            get { return _newCustomerObservation; }
            set { _newCustomerObservation = value; OnPropertyChanged("NewCustomerObservation"); }
        }

        public Command CreateNewCustomerCmd { get; private set; }

        #endregion

        #region PROPIEDADES PARA CREAR UNA NUEVA DEUDA
        private DateTime? _dateOfNewDebt;
        public DateTime? DateOfNewDebt
        {
            get { return _dateOfNewDebt; }
            set
            {
                _dateOfNewPayment = value;
                _dateOfNewDebt = value;
                OnPropertyChanged("DateOfNewPayment");
                OnPropertyChanged("DateOfNewDebt");
            }
        }

        private string _errorOfDateDebt;
        public string ErrorOfDateDebt
        {
            get { return _errorOfDateDebt; }
            private set { _errorOfDateDebt = value; OnPropertyChanged("ErrorOfDateDebt"); }
        }

        private string _descriptionOfNewDebt;
        public string DescriptionOfNewDebt
        {
            get { return _descriptionOfNewDebt; }
            set
            {
                if(value!=_descriptionOfNewDebt)
                {
                    _descriptionOfNewDebt = value;
                    OnPropertyChanged("DescriptionOfNewDebt");

                    if (string.IsNullOrEmpty(value))
                    {
                        LengtFree = 255;
                    }
                    else
                    {
                        LengtFree = 255 - value.Length;
                    }
                }
            }
        }

        private int _lengtFree;
        //Es la longitud que se muestra en la vita que queda disponible
        public int LengtFree
        {
            get { return _lengtFree; }
            private set { _lengtFree = value; OnPropertyChanged("LengtFree"); }
        }

        private string _errorOfDescritionDebt;
        public string ErrorOfDescriptionDebt
        {
            get { return _errorOfDescritionDebt; }
            private set { _errorOfDescritionDebt = value; OnPropertyChanged("ErrorOfDescriptionDebt"); }
        }

        private decimal _amountOfDebt;
        public decimal AmountOfDebt
        {
            get { return _amountOfDebt; }
            set { _amountOfDebt = value; OnPropertyChanged("AmountOfDebt"); }
        }

        private string _errorOfAmountDebt;
        public string ErrorOfAmountDebt
        {
            get { return _errorOfAmountDebt; }
            private set { _errorOfAmountDebt = value; OnPropertyChanged("ErrorOfAmountDebt"); }
        }

        public Command CreateDebtCmd { get; private set; }


        #endregion

        #region PROPIEDADES PARA CREAR UN NUEVO PAGO
        private DateTime? _dateOfNewPayment;
        public DateTime? DateOfNewPayment
        {
            get { return _dateOfNewPayment; }
            set
            {
                _dateOfNewPayment = value;
                _dateOfNewDebt = value;
                OnPropertyChanged("DateOfNewPayment");
                OnPropertyChanged("DateOfNewDebt");
            }
        }

        private string _errorOfDatePayment;
        public string ErrorOfDatePayment
        {
            get { return _errorOfDatePayment; }
            private set { _errorOfDatePayment = value; OnPropertyChanged("ErrorOfDatePayment"); }
        }

        private bool _cashPayment;
        public bool CashPayment
        {
            get { return _cashPayment; }
            set { _cashPayment = value; OnPropertyChanged("CashPayment"); }
        }

        private bool _cardPayment;
        public bool CardPayment
        {
            get { return _cardPayment; }
            set { _cardPayment = value; OnPropertyChanged("CardPayment"); }
        }

        private string _errorInPayment;
        public string ErrorInPayment
        {
            get { return _errorInPayment; }
            private set { _errorInPayment = value; OnPropertyChanged("ErrorInPayment"); }
        }

        private decimal _amountPayment;
        public decimal AmountPayment
        {
            get { return _amountPayment; }
            set { _amountPayment = value; OnPropertyChanged("AmountPayment"); }
        }

        private string _errorInAmountPayment;
        public string ErrorInAmountPayment
        {
            get { return _errorInAmountPayment; }
            private set { _errorInAmountPayment = value; OnPropertyChanged("ErrorInAmountPayment"); }
        }

        public Command CreatePaymentCmd { get; private set; }

        #endregion

        private DateTime? _updateCustomerSelected;
        /// <summary>
        /// Es la fecha en la que se va  hacer la consulta de 
        /// </summary>
        public DateTime? DateCustomerUpdateConsult
        {
            get { return _updateCustomerSelected; }
            set
            {
                _updateCustomerSelected = value;
                OnPropertyChanged("DateCustomerUpdateConsult");

                CustomersUpdates.Clear();

                if (value.HasValue && UserSelected != null)
                {
                    foreach (string customer in BDComun.ConsultCustomersUpdate(value.Value, UserSelected.ID))
                    {
                        CustomersUpdates.Add(customer);
                    }
                }
            }
        }

        public ObservableCollection<string> CustomersUpdates { get; set; }

        #region PROPIEDADES PARA ACTUALIZAR UN CLIENTE
        private bool _customerIsReadOnly;
        /// <summary>
        /// Esta propiedad permite modificar los campos del customer seleccionado;
        /// Por defecto cuando se selecciona un clientes se debe marcar como false o cuando este pase a ser null
        /// o cuando ya se haya actualizado los datos
        /// </summary>
        public bool CustomerIsReadOnly
        {
            get { return _customerIsReadOnly; }
            set { _customerIsReadOnly = value; OnPropertyChanged("CustomerIsReadOnly"); }
        }

        private bool _canModify;
        public bool CanModify
        {
            get { return _canModify; }
            set
            {
                _canModify = value;
                OnPropertyChanged("CanModify");
                CustomerIsReadOnly = !value;
            }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; OnPropertyChanged("CustomerName"); }
        }

        private string _erroOfUpdateCustomerName;
        public string ErrorOfUpdateCustomerName
        {
            get { return _erroOfUpdateCustomerName; }
            private set { _erroOfUpdateCustomerName = value; OnPropertyChanged("ErrorOfUpdateCustomerName"); }
        }

        private string _customerNit;
        public string CustomerNit
        {
            get { return _customerNit; }
            set { _customerNit = value; OnPropertyChanged("CustomerNit"); }
        }

        private string _errorOfUpdateCustomerNit;
        public string ErrorOfUpdateCustomerNit
        {
            get { return _errorOfUpdateCustomerNit; }
            private set { _errorOfUpdateCustomerNit = value; OnPropertyChanged("ErrorOfUpdateCustomerNit"); }
        }

        private string _customerPhone;
        public string CustomerPhone
        {
            get { return _customerPhone; }
            set { _customerPhone = value; OnPropertyChanged("CustomerPhone"); }
        }

        private string _customerAddress;
        public string CustomerAddres
        {
            get { return _customerAddress; }
            set { _customerAddress = value; OnPropertyChanged("CustomerAddres"); }
        }

        private string _customerObservation;
        public string CustomerObservation
        {
            get { return _customerObservation; }
            set { _customerObservation = value; OnPropertyChanged("CustomerObservation"); }
        }

        public ObservableCollection<CustomerTransaction> CustomerTransactions { get; set; }

        private CustomerTransaction _transactionSelected;
        public CustomerTransaction TransactionSelected
        {
            get { return _transactionSelected; }
            set
            {
                _transactionSelected = value;
                OnPropertyChanged("TransactionSelected");
                MontarTransaccion();
            }
        }

        public ObservableCollection<CustomerTracking> CustomerTrackings { get; set; }

        public Command UpdateCustomerSelectedCmd { get; private set; }

        #endregion

        #region MODIFICAR UNA TRANSACCION
        private bool _IsADebt;
        private int _transactionId;
        private int _customerId;
        private int _userId;

        private DateTime? _transactionDate;
        public DateTime? TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; OnPropertyChanged("TransactionDate"); }
        }

        private string _transactionDescription;
        public string TransactionDescription
        {
            get { return _transactionDescription; }
            set { _transactionDescription = value; OnPropertyChanged("TransactionDescription"); }
        }

        private string _transactionType;
        public string TransactionType
        {
            get { return _transactionType; }
            private set { _transactionType = value; OnPropertyChanged("TransactionType"); }
        }

        private decimal _transactionAmount;
        public decimal TransactionAmount
        {
            get { return _transactionAmount; }
            set { _transactionAmount = value; OnPropertyChanged("TransactionAmount"); }
        }

        public Command ModificarTransaccionCmd { get; private set; }
        
        /// <summary>
        /// Este metodo carga la informacion de la transaccion para ser modificada
        /// </summary>
        private void MontarTransaccion()
        {
            if(_transactionSelected != null)
            {
                _transactionId = TransactionSelected.ID;
                _customerId = CustomerSelected.CustomerID;
                _userId = UserSelected.ID;

                if (TransactionSelected.Deuda > 0)
                {
                    TransactionType = "Deuda";
                    _IsADebt = true;
                    TransactionAmount = TransactionSelected.Deuda;
                }
                else
                {
                    TransactionType = "Abono";
                    _IsADebt = false;
                    TransactionAmount = TransactionSelected.Abono;
                }

                TransactionDate = TransactionSelected.Fecha;
                TransactionDescription = TransactionSelected.Descriptcion;
                
            }
            else
            {
                _transactionId = -1;
                _customerId = -1;
                _userId = -1;
                TransactionType = null;
                TransactionDescription = null;
                TransactionDate = null;
                TransactionAmount = 0;
            }
        }

        private void ModificarTransaccion()
        {
            if(_IsADebt)
            {
                BDComun.UpdateCustomerDebt(_transactionId, _userId, _customerId, TransactionDescription, TransactionDate.Value);
            }
            else
            {
                BDComun.UpdateCustomerPayment(_transactionId, _userId, _customerId, TransactionDescription, TransactionDate.Value);
            }

            BDComun.UpdateCustmerState(CustomerSelected);
            ConsultTransactions();
        }

        #endregion


        public Command CancelOperation { get; private set; }
        public Command ActivateNewCustomerView { get; private set; }
        public Command ActivateNewDebtView { get; private set; }
        public Command ActivateNewPaymentView { get; private set; }

        /// <summary>
        /// Este metodo es utilizado al crear la view para inicializar las listada de la clase
        /// </summary>
        private void InicializarListas()
        {
            allCustomer = new List<Customer>();
            Users = new ObservableCollection<User>();
            Customers = new ObservableCollection<Customer>();
            CustomerTransactions = new ObservableCollection<CustomerTransaction>();
            CustomerTrackings = new ObservableCollection<CustomerTracking>();
            CustomersUpdates = new ObservableCollection<string>();
        }

        //Este metodo inicializa los comando de la clase al momento de ser creada
        private void InicializarCommand()
        {
            CreateNewCustomerCmd = new Command(CreateNewCustomer, () => UserSelected != null);
            CreateDebtCmd = new Command(CreateDebt, ValidateCommandOfNewDebt);
            CreatePaymentCmd = new Command(CreatePayment, ValidateCommandOfNewDebt);
            ConsultTransactionsCmd = new Command(ConsultTransactions, () => CustomerSelected != null);
            UpdateCustomerSelectedCmd = new Command(UpdateCustomerSelected, () => _canModify);
            DeleteCustomerCmd = new Command(DeleteCustomer, () => _customerSelected != null);
            ModificarTransaccionCmd = new Command(ModificarTransaccion, () => TransactionSelected != null);
            CancelOperation = new Command(ActivatState, () => !IsEnabled);
            ActivateNewCustomerView = new Command(() => WaitingState(AuxiliarView.NewClient), () => UserSelected != null);
            ActivateNewDebtView = new Command(() => WaitingState(AuxiliarView.NewDebt), () => CustomerSelected != null);
            ActivateNewPaymentView = new Command(() => WaitingState(AuxiliarView.NewPayment), () => CustomerSelected != null);
        }

        public ViewClientes()
        {
            InicializarListas();
            InicializarCommand();
            MaxDate = DateTime.Now;
            PaymentTracking.DefineParameters(100, 10, -10);
            DateCustomerUpdateConsult = DateTime.Now;
            IsEnabled = true;
        }

        /// <summary>
        /// Recarga la lista de usuarios desde la base de datos, y como este es un listado
        /// principal tambien limpia el listado de clientes
        /// </summary>
        private void ReloadUsers()
        {
            Users.Clear();
            foreach(User user in BDComun.ReadAllUser())
            {
                Users.Add(user);
            }

            ReloadCustomers();
        }

        /// <summary>
        /// Recarga el listado de clientes desde la base de datos cuando este metodo es llanado.
        /// </summary>
        private void ReloadCustomers()
        {
            Customers.Clear();
            allCustomer.Clear();
            SearchByName = null;
            EntryOrder = true;
            SortByDaysOfDelay = false;

            if(UserSelected != null)
            {
                foreach (Customer customer in BDComun.ReadAllCustomers(UserSelected))
                {
                    allCustomer.Add(customer);
                    Customers.Add(customer);
                }
            }
        }

        /// <summary>
        /// Metodo encargado de validar los datos ingresados por el usuario antes de tratar de ingresarlos
        /// en la base de datos
        /// </summary>
        /// <returns></returns>
        private bool ValidateNewCustomer()
        {
            ErrorNewCustomerName = null;
            ErrorNewCustomerNit = null;

            //Primero se valida el nombre del nuevo clientes
            if (!string.IsNullOrEmpty(_newCustomerName) || !string.IsNullOrWhiteSpace(_newCustomerName))
            {
                //Como se sabe que no está en blanco se verifica que el nombre no esté repetido
                foreach(Customer customer in allCustomer)
                {
                    if(customer.CustomerName.ToUpper() == _newCustomerName.ToUpper())
                    {
                        ErrorNewCustomerName = "El nombre utilizado ya se encuentra en uso";
                        return false;
                    }
                }//Fin de foreach
            }
            else
            {
                ErrorNewCustomerName = "Este campo no puede estar en blanco";
                return false;
            }

            //Ahora se valida el numero de nit, que por custiones legales debe ser unico
            if(string.IsNullOrWhiteSpace(_newCustomerNit) || string.IsNullOrEmpty(_newCustomerNit))
            {
                NewCustomerNit = null;
            }
            else
            {
                //Se verifica entre los nit de los clientes
                foreach(Customer customer in allCustomer)
                {
                    if(!string.IsNullOrEmpty(customer.Nit))
                    {
                        if(customer.Nit == _newCustomerNit)
                        {
                            ErrorNewCustomerNit = "Nit establecido a " + customer.CustomerName;
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Este metodo valida primero que los datos a ingresar sean correctos y luego procede a hacer una actualizacion
        /// de la base de datos para cuando el proceso sea exitoso recargar el listado de clientes del usuario actual
        /// </summary>
        private void CreateNewCustomer()
        {
            if(ValidateNewCustomer())
            {
                if (BDComun.AddCustomer(UserSelected.ID, _newCustomerName, _newCustomerObservation,
                    _newCustomerNit, _newCustomerAddress, _newCustomerPhone)) 
                {
                    ActivatState();
                    MostrarMensaje("Cliente creado satisfactoriamente");            //Se muestra el mensaje al usuario de que ha ido todo correctamente
                    ReloadCustomers();                                              //Se recargan los clientes de la base de datos

                    //Se selecciona el clientes ingresado
                    for(int index = Customers.Count-1; index>0; index--)
                    {
                        Customer c = Customers[index];
                        if (c.CustomerName.ToUpper() == NewCustomerName.ToUpper())
                        {
                            CustomerSelected = c;
                            break;
                        }
                    }

                    //Finalmente se limpian los campos para crear clientes
                    NewCustomerName = null;
                    NewCustomerObservation = null;
                    NewCustomerNit = null;
                    NewCustomerAddress = null;
                    NewCustomerPhone = null;
                    
                }
                else
                {
                    MostrarMensaje(BDComun.Error);
                }
            }
        }

        /// <summary>
        /// Este metodo busca los clientes segun el texto suministrado
        /// </summary>
        /// <param name="text">Texto a buscar entre los nombres de clos clientes</param>
        private void SearchCustomer(string text)
        {
            Customers.Clear();
            List<Customer> customerTemporaryList = new List<Customer>();

            //Primero se selecciona los clietes segun el nombre ingresado
            if(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                foreach(Customer customer in allCustomer)
                {
                    customerTemporaryList.Add(customer);
                }
            }
            else
            {
                foreach (Customer customer in allCustomer)
                {
                    string customerName = customer.CustomerName.ToUpper();
                    bool containText = customerName.Contains(text.ToUpper());
                    if (containText)
                    {
                        customerTemporaryList.Add(customer);
                    }

                }
            }

            //Ahora se ordena por orden alfabetico si está seleccionado
            if(AlfabeticalOrder)
            {
                customerTemporaryList.Sort((x, y) => x.CustomerName.CompareTo(y.CustomerName));
            }
            else if (SortByPoint)
            {
                customerTemporaryList.Sort((x, y) => x.Points.CompareTo(y.Points));
            }

            //Ahora se criba por los dias en mora, le da prioridad a los días de mora
            if (SortByDaysOfDelay)
            {
                customerTemporaryList.Sort((x, y) => x.Days.CompareTo(y.Days));
                foreach(Customer customer in customerTemporaryList)
                {
                    if (customer.Days >= DayOfDelay) 
                    {
                        Customers.Add(customer);
                    }
                }
            }
            else
            {
                foreach(Customer customer in customerTemporaryList)
                {
                    Customers.Add(customer);
                }
            }

            
            
        }//Fin del metodo

        /// <summary>
        /// Este metodo valida la opcion de intentar crear una nueva deuda
        /// </summary>
        /// <returns></returns>
        private bool ValidateCommandOfNewDebt()
        {
            if (UserSelected == null || CustomerSelected == null)
                return false;

            return true;
        }

        /// <summary>
        /// Este metodo valida que los datos suministrados por el usuario esten correctemente diligenciados
        /// </summary>
        /// <returns></returns>
        private bool ValidateNewDebt()
        {
            bool dateCorrect = true;
            bool descriptionCorrect = true;
            bool amountCorrect = true;

            ErrorOfDateDebt = null;
            ErrorOfDescriptionDebt = null;
            ErrorOfAmountDebt = null;

            //Primero se valida que la fecha esté seleccionada
            if(!DateOfNewDebt.HasValue)
            {
                dateCorrect = false;
                ErrorOfDateDebt = "Se debe seleccionar una fecha";
            }

            if(string.IsNullOrEmpty(DescriptionOfNewDebt) || string.IsNullOrWhiteSpace(DescriptionOfNewDebt))
            {
                descriptionCorrect = false;
                ErrorOfDescriptionDebt = "Este campo no puede estar en blanco";
            }

            if(AmountOfDebt<0)
            {
                amountCorrect = false;
                ErrorOfAmountDebt = "No puede ser negativo";
            }
            else if(AmountOfDebt==0)
            {
                amountCorrect = false;
                ErrorOfAmountDebt = "Una deuda no puede ser cero";
            }

            return (dateCorrect && descriptionCorrect && amountCorrect);
        }

        /// <summary>
        /// Este metodo actualiza la base de datos y muestra al usuario el resultado del proceso
        /// </summary>
        private void CreateDebt()
        {
            if(ValidateNewDebt())
            {
                string message = string.Format("Nuevo compromiso por valor de {0:c} al cliente {1}", AmountOfDebt, CustomerSelected.CustomerName);
                if(PreguntarUsuario(message, "Nuevo compromiso"))
                {
                    if (BDComun.AddDebt(CustomerSelected.UserID, CustomerSelected.CustomerID, DateOfNewDebt.Value, DescriptionOfNewDebt, AmountOfDebt))
                    {
                        DescriptionOfNewDebt = null;
                        AmountOfDebt = 0;
                        BDComun.UpdateCustmerState(CustomerSelected);
                        ConsultTransactions();
                        DateCustomerUpdateConsult = DateTime.Now;
                        ActivatState();
                    }
                    else
                    {
                        MostrarMensaje(BDComun.Error);
                    }
                }
                

            }
        }//Fin del metodo

        /// <summary>
        /// Este metodo valida que los datos ingresados por el usuario para crear un nuevo abono al 
        /// cliente sean todos correctos
        /// </summary>
        /// <returns></returns>
        private bool ValidateNewPayment()
        {
            bool dateCorrect = true;
            bool paymentMethodSelected = true;
            bool amountCorrect = true ;

            ErrorInAmountPayment = null;
            ErrorOfDatePayment = null;
            ErrorInPayment = null;

            if(!DateOfNewPayment.HasValue)
            {
                dateCorrect = false;
                ErrorOfDatePayment = "Se debe seleccionar una fecha";
            }

            if(!CashPayment && !CardPayment)
            {
                paymentMethodSelected = false;
                ErrorInPayment = "Se debe elegir una forma de pago";
            }

            if(AmountPayment<0)
            {
                amountCorrect = false;
                ErrorInAmountPayment = "No puede ser negativo";
            }
            else if(AmountPayment==0)
            {
                amountCorrect = false;
                ErrorInAmountPayment = "Un abono no puede terner saldo cero";
            }
            else if(AmountPayment > CustomerSelected.Balance)
            {
                amountCorrect = false;
                ErrorInAmountPayment = "El abono supera lo adeudado";
                
            }
            else if(DateOfNewPayment.HasValue)
            {
                if (!ValidateBalance(AmountPayment, DateOfNewPayment, CustomerSelected.Transactions))
                {
                    amountCorrect = false;
                    ErrorInAmountPayment = "Para esta fecha el abono supera la deuda"; 
                }
            }

            return (amountCorrect && paymentMethodSelected && dateCorrect);
        }

        /// <summary>
        /// Este metodo verifica que para la fecha especificada y con el monto definido no se genera
        /// un saldo a favor del cliente
        /// </summary>
        /// <param name="amountPayment"></param>
        /// <param name="paymentDate"></param>
        /// <param name="original"></param>
        /// <returns></returns>
        private bool ValidateBalance(decimal amountPayment, DateTime? paymentDate, List<CustomerTransaction> original)
        {
            DateTime currentDate = original[0].Fecha;           
            bool pagoRealizado = false;                             //Me permite monitorear que el pago se realice una vez

            if(paymentDate<currentDate)
            {
                return false;
            }

            decimal balance = 0;

            foreach(CustomerTransaction t in original)
            {
                currentDate = t.Fecha;

                if(currentDate<=paymentDate)
                {
                    balance += t.Deuda - t.Abono;
                }
                else
                {
                    //En este punto la fecha actual ha superaro la fecha del pago, por lo que se procede a
                    //validar que el balance no sea negativo al finalizar el recorrido por las transacciones
                    if(!pagoRealizado)
                    {
                        balance -= amountPayment;
                        pagoRealizado = true;
                    }
                    
                    if (balance < 0)
                    {
                        return false;
                    }

                    balance += t.Deuda - t.Abono;

                }
            }

            if(balance<0)
            {
                return false;
            }

            return true;
        }

        private void CreatePayment()
        {
            if (ValidateNewPayment())
            {
                string message = string.Format("Nuevo abono por valor de {0:c} al cliente {1}", AmountPayment, CustomerSelected.CustomerName);

                string observation = null;
                if(CashPayment)
                {
                    observation = "Pago en efectivo";
                }
                else
                {
                    observation = "Pago con tarjeta";
                }

                if(PreguntarUsuario(message, "NUEVO ABONO"))
                {
                    if (BDComun.AddPayment(UserSelected.ID, CustomerSelected.CustomerID, DateOfNewPayment.Value, observation, AmountPayment, CashPayment, CardPayment))
                    {
                        CashPayment = false;
                        CardPayment = false;
                        AmountPayment = 0m;

                        BDComun.UpdateCustmerState(CustomerSelected);
                        ConsultTransactions();
                        DateCustomerUpdateConsult = DateTime.Now;
                        ActivatState();
                    }
                    else
                    {
                        MostrarMensaje(BDComun.Error);
                    }
                }
                

            }
        }//Fin del metodo

        /// <summary>
        /// Este metodo consulta las transacciones para el cliente seleccionado
        /// </summary>
        private void ConsultTransactions()
        {
            CustomerTransactions.Clear();
            CustomerTrackings.Clear();
            foreach(CustomerTransaction t in CustomerSelected.Transactions)
            {
                CustomerTransactions.Add(t);
            }

            foreach(CustomerTracking cT in CustomerSelected.CustomerTracking)
            {
                CustomerTrackings.Add(cT);
            }
            
        }

        /// <summary>
        /// Valida que los valores que se intentan actualizar sean correctos y no esten repetidos
        /// </summary>
        /// <returns></returns>
        private bool ValidateCustomerUpdate()
        {
            bool customerNameCorrect = true;
            bool customerNitCorrect = true;
            ErrorOfUpdateCustomerName = null;
            ErrorOfUpdateCustomerNit = null;


            //Primero se valida el posble cambio de nombre
            foreach (Customer customer in allCustomer)
            {
                if(CustomerName.ToUpper() == customer.CustomerName.ToUpper() 
                    && customer.CustomerID != CustomerSelected.CustomerID)
                {
                    customerNameCorrect = false;
                    ErrorOfUpdateCustomerName = "EL nombre que tratas de ingresar ya está en uso";
                    break;
                }
            }

            //Se valida el posible valor del nit
            if(!string.IsNullOrEmpty(CustomerNit) || !string.IsNullOrWhiteSpace(CustomerNit))
            {
                foreach (Customer customer in allCustomer)
                {
                    if (CustomerNit == customer.Nit
                        && customer.CustomerID != CustomerSelected.CustomerID)
                    {
                        customerNitCorrect = false;
                        ErrorOfUpdateCustomerNit = "Este nit pertenece a "+customer.CustomerName;
                        break;
                    }
                }
            }

            return (customerNitCorrect && customerNameCorrect);
        }

        /// <summary>
        /// Este metodo actualiza las propiedades relacionadas al area de actualizacion de informacion
        /// de la vista.
        /// </summary>
        /// <param name="customer"></param>
        private void UpdateCustomerProperty(Customer customer)
        {
            CanModify = false;

            if (customer != null)
            {
                CustomerName = customer.CustomerName;
                CustomerNit = customer.Nit;
                CustomerAddres = customer.Address;
                CustomerPhone = customer.Phone;
                CustomerObservation = customer.Observation;
                ConsultTransactions();
            }
            else
            {
                CustomerName = null;
                CustomerNit = null;
                CustomerAddres = null;
                CustomerPhone = null;
                CustomerObservation = null;
                CustomerTransactions.Clear();
            }

            
        }

        /// <summary>
        /// Actualiza la informacion de la base de datos y si es correcto actualiza los campos
        /// sin tener que consultar a la base de datos
        /// </summary>
        private void UpdateCustomerSelected()
        {
            if(ValidateCustomerUpdate())
            {
                bool result = BDComun.UpdateCustomer(CustomerSelected.UserID, CustomerSelected.CustomerID,
                    CustomerName, CustomerObservation, CustomerPhone, CustomerAddres, CustomerNit);

                if (result)
                {
                    CustomerSelected.CustomerName = CustomerName;
                    CustomerSelected.Observation = CustomerObservation;
                    CustomerSelected.Phone = CustomerPhone;
                    CustomerSelected.Address = CustomerAddres;
                    CustomerSelected.Nit = CustomerNit;

                    UpdateCustomerProperty(CustomerSelected);
                    MostrarMensaje("Datos actualizados correctamente");
                }
                else
                {
                    MostrarMensaje(BDComun.Error);
                }
            }
        }

        private void DeleteCustomer()
        {
            if (CustomerSelected != null)
            {
                string message = "Se va a eliminar al usuario " + CustomerSelected.CustomerName;

                if(PreguntarUsuario(message, "Eliminacion de cliente"))
                {
                    if (BDComun.DeleteCustomer(CustomerSelected.CustomerID))
                    {
                        ReloadCustomers();
                    }
                    else
                    {
                        MostrarMensaje(BDComun.Error);
                    }
                }
                
            }
        }

        private void WaitingState(AuxiliarView aux)
        {
            IsEnabled = false;

            switch (aux)
            {
                case AuxiliarView.NewClient:
                     NewCustomerAux = true;
                    break;
                case AuxiliarView.NewDebt:
                    NewDebtAux = true;
                    break;
                case AuxiliarView.NewPayment:
                    NewpaymentAux = true;
                    break;
            }
        }

        private void ActivatState()
        {
            NewCustomerAux = false;
            NewDebtAux = false;
            NewpaymentAux = false;
            IsEnabled = true;
        }
    }
}
