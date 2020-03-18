using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Control_de_cajas.Modelo;
using Utilidades;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Control_de_cajas.ViewModels
{
    class ViewUsers : ViewModelBase
    {
        private enum AuxiliarView { NewUser, NewBox, NewTramsaction, ModifyTransaction, CategoryGestion }              //Para identificar las vista auxiliares 
        private List<Category> allCategories;
        private DateTime currentDate;
        private List<CashboxTransaction> allTransactions;

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
                if (value)
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

        private bool _newUserAuxiliarView;
        /// <summary>
        /// Esta propiedad permite hacer visible el formulario para crear un nuevo usuario
        /// </summary>
        public bool NewUserAuxiliarView
        {
            get { return _newUserAuxiliarView; }
            private set
            {
                _newUserAuxiliarView = value;
                OnPropertyChanged("NewUserAuxiliarView");
            }
        }

        private bool _newCahsboxAuxiliarView;

        public bool NewCashboxAuxiliarView
        {
            get { return _newCahsboxAuxiliarView; }
            private set
            {
                _newCahsboxAuxiliarView = value;
                OnPropertyChanged("NewCashboxAuxiliarView");
            }

        }

        private bool _modifyTransactionAuxiliar;
        public bool ModifyTransactionAuxiliar
        {
            get { return _modifyTransactionAuxiliar; }
            private set
            {
                _modifyTransactionAuxiliar = value;
                OnPropertyChanged("ModifyTransactionAuxiliar");
            }
        }

        private bool _categoryGestionView;
        public bool CategoryGestionView
        {
            get { return _categoryGestionView; }
            set
            {
                _categoryGestionView = value;
                OnPropertyChanged("CategoryGestionView");
            }
        }

        /// <summary>
        /// Listado de usuarios en la base de datos
        /// </summary>
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Cashbox> Boxs { get; set; }


        private User _userSelected;
        /// <summary>
        /// Es el usuario seleccionado del listado de usuario, require hacer algunas acciones cuando
        /// cambia a null;
        /// </summary>
        public User UserSelected
        {
            get { return _userSelected; }
            set
            {
                UserIsSelected(_userSelected, value);
                _userSelected = value;
                OnPropertyChanged("UserSelected");
                LoadBoxs();
                LoadCategories();
                TransferView.SetParameters(value, Boxs.ToList());
                ConsultTransView.DefinedUser(value);
            }
        }

        private Cashbox _boxSelected;
        public Cashbox BoxSelected
        {
            get { return _boxSelected; }
            set
            {
                CashboxIsSelected(_boxSelected, value);
                _boxSelected = value;
                OnPropertyChanged("BoxSelected");
                ReloadTransaction();
            }
        }

        public ObservableCollection<CashboxTransaction> Transactions { get; set; }

        private CashboxTransaction _transactionSelected;
        public CashboxTransaction TransactionSelected
        {
            get { return _transactionSelected; }
            set { _transactionSelected = value; OnPropertyChanged("TransactionSelected"); }
        }

        #region NEW USER
        private string _nameOfNewUser;
        public string NameOfNewUser
        {
            get { return _nameOfNewUser; }
            set { _nameOfNewUser = value; OnPropertyChanged("NameOfNewUser"); }
        }

        private string _errorOfNewUser;
        public string ErrorOfNewUser
        {
            get { return _errorOfNewUser; }
            private set { _errorOfNewUser = value; OnPropertyChanged("ErrorOfNewUser"); }
        }

        public Command CreateNewUserCmd { get; private set; }
        public Command DeleteUserCmd { get; private set; }

        #endregion

        #region NEW CASHBOX
        private string _newCashBoxName;
        public string NewCashBoxName
        {
            get { return _newCashBoxName; }
            set { _newCashBoxName = value; OnPropertyChanged("NewCashBoxName"); }
        }

        private string _errorOfCashboxName;
        public string ErrorOfCashboxName
        {
            get { return _errorOfCashboxName; }
            private set { _errorOfCashboxName = value; OnPropertyChanged("ErrorOfCashboxName"); }
        }

        public Command CreateNewCashboxCmd { get; private set; }

        #endregion

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Category> CategoriesSelected { get; set; }

        private string _category_route;
        public string CategoryRoute
        {
            get { return _category_route; }
            private set
            {
                _category_route = value;
                OnPropertyChanged("CategoryRoute");
            }
        }

        private string _errorOfBox;
        public string ErrorOfBox
        {
            get { return _errorOfBox; }
            private set { _errorOfBox = value; OnPropertyChanged("ErrorOfBox"); }
        }

        private Category _categorySelected;
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

        private string _errorOfCategory;
        public string ErrorOfCategory
        {
            get { return _errorOfCategory; }
            private set { _errorOfCategory = value; OnPropertyChanged("ErrorOfCategory"); }
        }

        private DateTime? _transactionDate;
        public DateTime? TransactionDate
        {
            get { return _transactionDate; }
            set
            {
                _transactionDate = value;
                OnPropertyChanged("TransactionDate");
                RechargeTransactions();
            }
        }

        public List<int> Hours { get; set; }
        public List<int> Minutes { get; set; }

        private int _hourSelected;
        public int HourSelected
        {
            get { return _hourSelected; }
            set { _hourSelected = value;  OnPropertyChanged("HourSelected"); }
        }

        private int _minuteSelected;
        public int MinuteSelected
        {
            get { return _minuteSelected; }
            set { _minuteSelected = value; OnPropertyChanged("MinuteSelected"); }
        }

        private string _errorOfDate;
        public string ErrorOfDate
        {
            get { return _errorOfDate; }
            private set { _errorOfDate = value; OnPropertyChanged("ErrorOfDate"); }
        }

        private DateTime? _startTransactionDate;
        public DateTime? StartTransactionDate
        {
            get { return _startTransactionDate; }
            set { _startTransactionDate = value; OnPropertyChanged("StartTransactionDate"); }
        }

        private DateTime? _endTransactionDate;
        public DateTime? EndTransactionDate
        {
            get { return _endTransactionDate; }
            set { _endTransactionDate = value; OnPropertyChanged("EndTransactionDate"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }

        private string _errorOfDescription;
        public string ErrorOfDescription
        {
            get { return _errorOfDescription; }
            private set { _errorOfDescription = value; OnPropertyChanged("ErrorOfDescription"); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                AmountCorrector();
            }
        }

        private bool _depositType;
        public bool DepositType
        {
            get { return _depositType; }
            set
            {
                _depositType = value;
                AmountCorrector();
            }
        }

        private bool _egressType;
        public bool EgressType
        {
            get { return _egressType; }
            set
            {
                _egressType = value;
                AmountCorrector();
            }
        }

        private string _errorOfAmount;
        public string ErrorOfAmount
        {
            get { return _errorOfAmount; }
            private set { _errorOfAmount = value; OnPropertyChanged("ErrorOfAmount"); }
        }

        private bool _lanzarAperturaDeCajon;
        public bool LanzarAperturaDeCajon
        {
            get { return _lanzarAperturaDeCajon; }
            set { _lanzarAperturaDeCajon = value; OnPropertyChanged("LanzarAperturaDeCajon"); }
        }

        public Command RemoveCategoryCmd { get; private set; }
        public Command AddNewTransactionCmd { get; private set; }

        public Command ActivateViewNewUser { get; private set; }
        public Command ActivateViewNewBox { get; private set; }
        public Command ActivateModifyTransaction { get; set; }
        public Command ActivateCategoryGestion { get; set; }
        public Command CancelOperation { get; private set; }
        public Command ModifyTransactionsCmd { get; private set; }
        public Command Imprimir { get; private set; }
        public Command ImprimirXCmd { get; private set; }
        public Command ImprimirZCmd { get; private set; }

        public ViewTransferencia TransferView { get; set; }
        public ConsultTransactionView ConsultTransView { get; set; }
        public ViewModifyTransaction ModifyTransactionView { get; set; }
        public ViewCategorias ViewCategorias { get; set; }

        public ViewUsers()
        {
            TransferView = new ViewTransferencia();
            ConsultTransView = new ConsultTransactionView();
            ModifyTransactionView = new ViewModifyTransaction();
            ViewCategorias = new ViewCategorias();
            InicializarListas();
            InicializarComandos();
            ReloadUsers();
            EndTransactionDate = DateTime.Now;
            IsEnabled = true;
            HourSelected = Hours[EndTransactionDate.Value.Hour];
            MinuteSelected = Minutes[EndTransactionDate.Value.Minute];
        }

        private void InicializarListas()
        {
            allCategories = new List<Category>();
            allTransactions = new List<CashboxTransaction>();
            Users = new ObservableCollection<User>();
            Boxs = new ObservableCollection<Cashbox>();
            Categories = new ObservableCollection<Category>();
            CategoriesSelected = new ObservableCollection<Category>();
            Transactions = new ObservableCollection<CashboxTransaction>();
            ReportsOfBoxClosed = new ObservableCollection<ReportOfBoxClosed>();

            Hours = new List<int>();
            Minutes = new List<int>();

            for(int i = 0; i<24; i++)
            {
                Hours.Add(i);
            }

            for (int i= 0; i<60; i++)
            {
                Minutes.Add(i);
            }
        }

        private void InicializarComandos()
        {
            CreateNewUserCmd = new Command(CreateNewUser, () => true);
            DeleteUserCmd = new Command(DeleteUser, () => UserSelected != null);
            CreateNewCashboxCmd = new Command(CreateNewCasbox, () => UserSelected != null);
            RemoveCategoryCmd = new Command(RemoveCategory, () => CategoriesSelected.Count > 0);
            AddNewTransactionCmd = new Command(AddNewTransacction, ValidateCommandNewTransaction);
            ActivateViewNewUser = new Command(() => WaitingState(AuxiliarView.NewUser), () => true);
            ActivateViewNewBox = new Command(() => WaitingState(AuxiliarView.NewBox), () => UserSelected != null);
            ActivateModifyTransaction = new Command(() => WaitingState(AuxiliarView.ModifyTransaction), ValidateActiveModifyTransactios);
            ActivateCategoryGestion = new Command(() => WaitingState(AuxiliarView.CategoryGestion), () => true);
            CancelOperation = new Command(ActivatState, () => !IsEnabled);
            ModifyTransactionsCmd = new Command(ModifyTransaction, () => true);
            Imprimir = new Command(PrintDocument, () => BoxSelected != null);
            ImprimirXCmd = new Command(ImprimirX, () => ReportsOfBoxClosed.Count > 0);
            ImprimirZCmd = new Command(ImprimirZ, () => ReportsOfBoxClosed.Count > 0);
        }

        /// <summary>
        /// Este metodo recarga los datos de usuario desde la base de datos
        /// </summary>
        private void ReloadUsers()
        {
            Users.Clear();                                          //Se limpia la lista de usuarios

            foreach (User user in BDComun.ReadAllUser())             //Se consulta a la base de datos por los usuarios
            {
                Users.Add(user);
            }
        }

        /// <summary>
        /// Este metodo se encarga de verificar que el nombre que se intenta ingresar a la base de datos
        /// corresponda a un nombre unico, si no está en blanco o es null.
        /// </summary>
        /// <returns>Retorna true si el nombre es unico</returns>
        private bool ValidateNewUser()
        {
            //Primero verifico que el nombre no esté en blanco o vacío
            if (string.IsNullOrEmpty(_nameOfNewUser) || string.IsNullOrWhiteSpace(_nameOfNewUser))
            {
                ErrorOfNewUser = "Espacio en blanco";
                return false;
            }

            //Se valida que el nombre sea unico
            foreach (User user in Users)
            {
                if (user.UserName.ToUpper() == _nameOfNewUser.ToUpper())
                {
                    ErrorOfNewUser = "Este nombre ya está en uso";
                    return false;
                }
            }

            ErrorOfNewUser = null;
            return true;
        }

        /// <summary>
        /// Crea al usuario en la base de datos y recarga la lsita de usuarios
        /// </summary>
        private void CreateNewUser()
        {
            if (ValidateNewUser())
            {
                if (BDComun.NewUser(_nameOfNewUser))
                {
                    ReloadUsers();
                    NameOfNewUser = null;
                    ActivatState();
                    MostrarMensaje("Usuario creado satisfactoriamente");
                }
                else
                {
                    MostrarMensaje(BDComun.Error);
                }
            }
        }

        /// <summary>
        /// Elimina el usuario de la base de datos y recarga el listado de usuarios
        /// </summary>
        private void DeleteUser()
        {
            string message = "Se va a eliminar al usuario ";
            message += UserSelected.ToString();
            message += " junto con todas las categorias asociadas a este";
            if (PreguntarUsuario(message, "Eliminar usuario"))
            {
                if (BDComun.DeleteUser(UserSelected.ID))
                {
                    MostrarMensaje("Usuario eliminado");
                    ReloadUsers();
                }
            }


        }

        /// <summary>
        /// Carga las cajas del usuario que contiene en la base de datos
        /// </summary>
        private void LoadBoxs()
        {
            Boxs.Clear();
            if (UserSelected != null)
            {
                foreach (Cashbox box in BDComun.ReadAllCashbox(UserSelected.ID))
                {
                    Boxs.Add(box);
                }
            }
        }

        /// <summary>
        /// Este metodo recarga las categorias al cambiar el usuario
        /// </summary>
        private void LoadCategories()
        {
            CategoriesSelected.Clear();
            Categories.Clear();

            if (UserSelected != null)
            {
                CategorySystem.DefineParameter(UserSelected.ID);
                foreach (Category c in CategorySystem.GetFatherCategories())
                {
                    Categories.Add(c);
                }
            }
            else
            {
                CategorySystem.DefineParameter(null);
            }

            WriteCategoryRoute();
        }

        private bool ValidateNewCashbox()
        {
            ErrorOfCashboxName = null;

            if (string.IsNullOrEmpty(_newCashBoxName) || string.IsNullOrEmpty(_newCashBoxName))
            {
                ErrorOfCashboxName = "Campo obligatorio";
                return false;
            }

            foreach (Cashbox box in Boxs)
            {
                if (box.BoxName.ToUpper() == _newCashBoxName.ToUpper())
                {
                    ErrorOfCashboxName = "Nombre en uso";
                    return false;
                }
            }

            return true;
        }

        private void CreateNewCasbox()
        {
            if (ValidateNewCashbox())
            {
                if (BDComun.CreateCasbox(UserSelected.ID, NewCashBoxName))
                {
                    BDComun.ReloadUser(UserSelected);
                    NewCashBoxName = null;
                    ActivatState();
                    MostrarMensaje("Caja creada satisfactoriamente");
                    LoadBoxs();
                }
            }
        }

        private void AddCategory()
        {
            Category actualCategory = CategorySelected;

            CategoriesSelected.Add(CategorySelected);
            Categories.Clear();

            foreach (Category c in CategorySystem.RecoverySubcategories(actualCategory))
            {
                Categories.Add(c);
            }

            WriteCategoryRoute();
        }

        private void RemoveCategory()
        {
            int count = CategoriesSelected.Count;
            Categories.Clear();

            CategoriesSelected.RemoveAt(count - 1);
            count--;

            if (count == 0)
            {
                foreach (Category c in CategorySystem.RecoveryMainCategory())
                {
                    Categories.Add(c);
                }
            }
            else
            {
                Category lastCategory = CategoriesSelected[count - 1];
                foreach (Category c in CategorySystem.RecoverySubcategories(lastCategory))
                {
                    Categories.Add(c);
                }
            }

            WriteCategoryRoute();
        }

        /// <summary>
        /// Este metodo se encarga de habilitar o deshabilitar el comando que permite a la view agregar una
        /// nueva tranmsaccion a la base de datos
        /// </summary>
        /// <returns></returns>
        private bool ValidateCommandNewTransaction()
        {
            if (UserSelected == null)
            {
                return false;
            }

            if (BoxSelected == null)
            {
                return false;
            }

            if (Categories.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Este metodo verifica que todos los campos y requisitos para crea una transaccion sean correctos
        /// </summary>
        /// <returns></returns>
        private bool ValidateNewTransaction()
        {
            bool result = true;

            ErrorOfBox = null;
            ErrorOfDate = null;
            ErrorOfDescription = null;
            ErrorOfCategory = null;
            ErrorOfAmount = null;

            if (BoxSelected == null)
            {
                ErrorOfBox = "Se debe seleccionar una caja";
                result = false;
            }

            if (!TransactionDate.HasValue)
            {
                ErrorOfDate = "Se debe eleguir una fecha valida";
                result = false;
            }

            if (CategoriesSelected.Count == 0)
            {
                ErrorOfCategory = "Se debe elegir una categoría";
                result = false;
            }
            else if (Categories.Count > 0)
            {
                ErrorOfCategory = "Se debe completar la ruta de categorías";
                result = false;
            }

            if (string.IsNullOrEmpty(Description) || string.IsNullOrWhiteSpace(Description))
            {
                ErrorOfDescription = "Este campo es obligatorio";
                result = false;
            }

            if (Amount == 0m)
            {
                ErrorOfAmount = "Transaccion con valor cero";
                result = false;
            }

            return result;
        }

        private void AddNewTransacction()
        {
            if (ValidateNewTransaction())
            {
                int user_id = UserSelected.ID;
                int cashbox_id = BoxSelected.ID;
                List<int> categoriesId = new List<int>();
                foreach (Category c in CategoriesSelected)
                {
                    categoriesId.Add(c.ID);
                }

                DateTime fecha = new DateTime(TransactionDate.Value.Year, TransactionDate.Value.Month, TransactionDate.Value.Day, HourSelected, MinuteSelected, 0);

                if (BDComun.AddNewTransaction(user_id, cashbox_id, fecha, Description, Amount, categoriesId))
                {
                    Description = null;
                    LoadCategories();
                    Amount = 0;

                    BDComun.ReloadUser(UserSelected);
                    BDComun.ReloadCashbox(BoxSelected);
                    CashboxTransaction lastTransaction = BDComun.RecuperarUltimaTransaccion(user_id, cashbox_id);

                    if (lastTransaction != null)
                    {
                        allTransactions.Add(lastTransaction);
                        RechargeTransactions();
                    }

                    if(LanzarAperturaDeCajon)
                    {
                        PrintDocument();
                        LanzarAperturaDeCajon = false;
                    }

                    MostrarMensaje("Transaccion creada satisfctoriamente");
                }
                else
                {
                    MostrarMensaje(BDComun.Error);
                }
            }
        }

        private void ReloadTransaction()
        {
            allTransactions.Clear();

            if (BoxSelected != null && UserSelected != null)
            {
                int user_id = UserSelected.ID;
                int cashbox_id = BoxSelected.ID;
                allTransactions = BDComun.ReadAllTransactions(user_id, cashbox_id);

                RechargeTransactions();
            }
            else
            {
                Transactions.Clear();
            }
            //
        }

        /// <summary>
        /// Este metodo recarga las transacciones de la lista allTransactions segun la fecha seleccionada para una neuva transaccion
        /// o segun la ultima fecha de las transacciones
        /// </summary>
        private void RechargeTransactions()
        {
            Transactions.Clear();

            if (TransactionDate.HasValue)
            {
                currentDate = TransactionDate.Value;
            }
            else if (allTransactions.Count > 0)
            {
                if(BoxSelected.FechaDeCierre.HasValue)
                {
                    currentDate = BoxSelected.FechaDeCierre.Value;
                }
                else
                {
                    currentDate = allTransactions[allTransactions.Count - 1].TransactionDate;
                }
            }

            foreach (CashboxTransaction t in allTransactions)
            {
                if (t.TransactionDate >= currentDate)
                {
                    Transactions.Add(t);
                }
            }

            CreateReportOfBox();
        }

        /// <summary>
        /// Este metodo corrige el valor del monto cada vez que se cambia el tipo de la transaccion 
        /// o directamente el valor del monto.
        /// </summary>
        private void AmountCorrector()
        {
            if (_depositType)
            {
                _amount = Math.Abs(_amount);
                OnPropertyChanged("Amount");
            }
            else if (_egressType)
            {
                _amount = Math.Abs(_amount) * -1;
                OnPropertyChanged("Amount");
            }
            else
            {
                if (_amount < 0)
                {
                    _egressType = true;
                    _depositType = false;
                }
                else
                {
                    _depositType = true;
                    _egressType = false;
                }

                OnPropertyChanged("EgressType");
                OnPropertyChanged("DepositType");
            }
        }

        /// <summary>
        /// Crea una cadena con la ruta actual de categorias para el usuario
        /// </summary>
        private void WriteCategoryRoute()
        {
            CategoryRoute = null;
            bool primero = true;

            foreach (Category c in CategoriesSelected)
            {
                if (primero)
                {
                    CategoryRoute = c.ToString();
                    primero = false;
                }
                else
                {
                    CategoryRoute += " --> " + c.ToString();
                }
            }
        }

        private void WaitingState(AuxiliarView aux)
        {
            IsEnabled = false;

            switch (aux)
            {
                case AuxiliarView.NewUser:
                    NewUserAuxiliarView = true;
                    break;
                case AuxiliarView.NewBox:
                    NewCashboxAuxiliarView = true;
                    break;
                case AuxiliarView.ModifyTransaction:
                    {
                        ModifyTransactionAuxiliar = true;
                        ModifyTransactionView.DefinedParameter(allTransactions, TransactionSelected);
                    }
                    break;
                case AuxiliarView.CategoryGestion:
                    {
                        CategoryGestionView = true;
                        ViewCategorias.IsSelected = true;
                        if (UserSelected != null)
                        {
                            foreach (User u in ViewCategorias.Users)
                            {
                                if (UserSelected.UserName == u.UserName)
                                {
                                    ViewCategorias.UserSelected = u;
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void ActivatState()
        {
            NewUserAuxiliarView = false;
            NewCashboxAuxiliarView = false;
            ModifyTransactionAuxiliar = false;

            if (CategoryGestionView)
            {
                string username = null;
                if (UserSelected != null)
                {
                    username = UserSelected.UserName;
                    IsSelected = true;
                    foreach (User u in Users)
                    {
                        if (u.UserName == username)
                        {
                            UserSelected = u;
                            break;
                        }
                    }
                }

                CategoryGestionView = false;
            }
            IsEnabled = true;
        }

        private bool ValidateActiveModifyTransactios()
        {
            if (UserSelected != null)
            {
                if (BoxSelected != null)
                {
                    if (allTransactions.Count > 0)
                    {
                        if (TransactionSelected != null)
                        {
                            if (!TransactionSelected.IsATransfer)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void ModifyTransaction()
        {
            if (ModifyTransactionView.UpdateTransactionInBD())
            {
                ActivatState();
                MostrarMensaje("Transaccion modificada");
                BDComun.ReloadUser(UserSelected);
                BDComun.ReloadCashbox(BoxSelected);
                ReloadTransaction();
            }
        }

        private void PrintDocument()
        {
            PrintDocument p = new PrintDocument();
            p.PrinterSettings.PrinterName = "EPSON TM-T20II Receipt";
            /*
            p.PrintPage += (s, e) =>
            {
                e.Graphics.DrawString("Hola Edith",
                                         new Font("Times New Roman", 12),
                                         new SolidBrush(Color.Black),
                                         new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width,
                                                         p.DefaultPageSettings.PrintableArea.Height));
            };
            */
            //p.Print();

            CrearTicket ticket = new CrearTicket();
            ticket.AbreCajon();
            ticket.ImprimirTicket("EPSON TM-T20II Receipt");

            //RawPrinterHelper.SendStringToPrinter("EPSON TM-T20II Receipt", "\x1B" + "p" + "\x00" + "\x0F" + "\x96");

        }

        private void ImprimirX()
        {
            CrearTicket ticket = new CrearTicket();
            ticket.TextoCentro("TIENDA CARMU");
            ticket.TextoCentro("NIT: 1.098.617.663");
            ticket.TextoCentro("C:C IBIRICO PLAZA LOCAL 15");
            ticket.TextoCentro("www.tiendacarmu.com");
            ticket.LineaGion();
            ticket.TextoExtremos("Fecha:", DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));
            ticket.TextoIzquierda("X");
            ticket.EncabezadoVenta();
            ticket.LineaGion();
            decimal ingresos = 0m;
            decimal egresos = 0m;
            foreach(ReportOfBoxClosed r in ReportsOfBoxClosed)
            {
                ticket.AgregaArticulo(r.Category, r.Cant, r.Amount);

                if(r.Amount>0)
                {
                    ingresos += r.Amount;
                }
                else
                {
                    egresos += r.Amount;
                }
            }

            ticket.LineaIguales();
            ticket.AgregarTotales("          TOTAL INGRESOS : $ ", ingresos);
            ticket.AgregarTotales("          TOTAL EGRESOS  : $ ", egresos);
            ticket.AgregarTotales("                SUBTOTAL : $ ", ingresos);
            ticket.LineaGion();
            ticket.AgregarTotales("                    BASE : $ ", BaseOfBox);
            ticket.AgregarTotales("                EFECTIVO : $ ", Cash);
            ticket.TextoIzquierda(" ");
            ticket.TextoIzquierda(" ");
            ticket.TextoIzquierda(" ");
            ticket.TextoIzquierda(" ");
            ticket.TextoIzquierda(" ");
            ticket.TextoIzquierda(" ");
            ticket.CortaTicket();
            ticket.AbreCajon();
            ticket.ImprimirTicket("EPSON TM-T20II Receipt");

        }

        private void ImprimirZ()
        {
            CrearTicket ticket = new CrearTicket();

            if (BDComun.CloseBox(BoxSelected.ID, DateTime.Now, NewBase))
            {
                ticket.TextoCentro("TIENDA CARMU");
                ticket.TextoCentro("NIT: 1.098.617.663");
                ticket.TextoCentro("C:C IBIRICO PLAZA LOCAL 15");
                ticket.TextoCentro("www.tiendacarmu.com");
                ticket.LineaGion();
                ticket.TextoExtremos("Desde:", BoxSelected.FechaDeCierre.HasValue ? BoxSelected.FechaDeCierre.Value.ToString("dd-MM-yyyy hh:mm tt") : "Origen de los tiempos");
                ticket.TextoExtremos("Fecha actual:", DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));
                ticket.TextoIzquierda("TICKET DE CIERRE (Z)");
                ticket.EncabezadoVenta();
                ticket.LineaGion();
                decimal ingresos = 0m;
                decimal egresos = 0m;
                foreach (ReportOfBoxClosed r in ReportsOfBoxClosed)
                {
                    ticket.AgregaArticulo(r.Category, r.Cant, r.Amount);

                    if (r.Amount > 0)
                    {
                        ingresos += r.Amount;
                    }
                    else
                    {
                        egresos += r.Amount;
                    }
                }

                ticket.LineaIguales();
                ticket.AgregarTotales("          TOTAL INGRESOS : $ ", ingresos);
                ticket.AgregarTotales("          TOTAL EGRESOS  : $ ", egresos);
                ticket.AgregarTotales("                SUBTOTAL : $ ", ingresos+egresos);//Es una suma porque los egresos tinen valores negativos
                ticket.LineaGion();
                ticket.AgregarTotales("                    BASE : $ ", BaseOfBox);
                ticket.AgregarTotales("                EFECTIVO : $ ", Cash);
                ticket.AgregarTotales("              NUEVA BASE : $ ", NewBase);
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.TextoIzquierda(" ");
                ticket.CortaTicket();
                ticket.AbreCajon();
                ticket.ImprimirTicket("EPSON TM-T20II Receipt");
            }
            else
            {
                ticket.AbreCajon();
                ticket.ImprimirTicket("EPSON TM-T20II Receipt");
            }


        }

        //METODOS PARA LOS REPORTES
        public ObservableCollection<ReportOfBoxClosed> ReportsOfBoxClosed { get; set; }
        private decimal _baseOfBox;
        public decimal BaseOfBox
        {
            get { return _baseOfBox; }
            private set { _baseOfBox = value; OnPropertyChanged("BaseOfBox"); }
        }

        private decimal _cash;
        public decimal Cash
        {
            get { return _cash; }
            set { _cash = value; OnPropertyChanged("Cash"); }
        }

        private decimal _newBase;
        public decimal NewBase
        {
            get { return _newBase; }
            set { _newBase = value; OnPropertyChanged("NewBase"); }
        }


        private void CreateReportOfBox()
        {
            BaseOfBox = 0m;
            Cash = 0m;
            ReportsOfBoxClosed.Clear();

            List<ReportOfBoxClosed> reports = new List<ReportOfBoxClosed>();

            foreach(Category c in CategorySystem.GetFatherCategories())
            {
                BDComun.ConsultAmountOfCategory(c.ID, BoxSelected.ID ,BoxSelected.FechaDeCierre, out int count, out decimal amount);
                ReportOfBoxClosed report = new ReportOfBoxClosed(c.Name, count, amount);
                reports.Add(report);
            }

            if(BoxSelected.BaseDeCaja.HasValue)
            {
                BaseOfBox = BoxSelected.BaseDeCaja.Value;
            }

            foreach(ReportOfBoxClosed r in reports)
            {
                if(r.Amount!=0)
                {
                    Cash += r.Amount;
                    ReportsOfBoxClosed.Add(r);
                }
            }

            Cash += BaseOfBox;
        }
    }

    public class CrearTicket
    {
        StringBuilder linea = new StringBuilder();
        int maxChar = 48, cortar;

        public string LineaGion()
        {
            string result = "";
            for(int i = 0; i < maxChar; i++)
            {
                result += "-";
            }

            return linea.AppendLine(result).ToString();
        }


        public string LineaAsteriscos()
        {
            string result = "";
            for (int i = 0; i < maxChar; i++)
            {
                result += "*";
            }

            return linea.AppendLine(result).ToString();
        }

        public string LineaIguales()
        {
            string result = "";
            for (int i = 0; i < maxChar; i++)
            {
                result += "=";
            }

            return linea.AppendLine(result).ToString();
        }

        //Creamos un metodo para poner el texto a la izquierda
        public void TextoIzquierda(string texto)
        {
            //Si la longitud del texto es mayor al numero maximo de caracteres permitidos, realizar el siguiente procedimiento.
            if (texto.Length > maxChar)
            {
                int caracterActual = 0;//Nos indicara en que caracter se quedo al bajar el texto a la siguiente linea
                for (int longitudTexto = texto.Length; longitudTexto > maxChar; longitudTexto -= maxChar)
                {
                    //Agregamos los fragmentos que salgan del texto
                    linea.AppendLine(texto.Substring(caracterActual, maxChar));
                    caracterActual += maxChar;
                }
                //agregamos el fragmento restante
                linea.AppendLine(texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                //Si no es mayor solo agregarlo.
                linea.AppendLine(texto);
            }
        }

        //Creamos un metodo para poner texto a la derecha.
        public void TextoDerecha(string texto)
        {
            //Si la longitud del texto es mayor al numero maximo de caracteres permitidos, realizar el siguiente procedimiento.
            if (texto.Length > maxChar)
            {
                int caracterActual = 0;//Nos indicara en que caracter se quedo al bajar el texto a la siguiente linea
                for (int longitudTexto = texto.Length; longitudTexto > maxChar; longitudTexto -= maxChar)
                {
                    //Agregamos los fragmentos que salgan del texto
                    linea.AppendLine(texto.Substring(caracterActual, maxChar));
                    caracterActual += maxChar;
                }
                //Variable para poner espacios restntes
                string espacios = "";
                //Obtenemos la longitud del texto restante.
                for (int i = 0; i < (maxChar - texto.Substring(caracterActual, texto.Length - caracterActual).Length); i++)
                {
                    espacios += " ";//Agrega espacios para alinear a la derecha
                }

                //agregamos el fragmento restante, agregamos antes del texto los espacios
                linea.AppendLine(espacios + texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                string espacios = "";
                //Obtenemos la longitud del texto restante.
                for (int i = 0; i < (maxChar - texto.Length); i++)
                {
                    espacios += " ";//Agrega espacios para alinear a la derecha
                }
                //Si no es mayor solo agregarlo.
                linea.AppendLine(espacios + texto);
            }
        }

        //Metodo para centrar el texto
        public void TextoCentro(string texto)
        {
            if (texto.Length > maxChar)
            {
                int caracterActual = 0;//Nos indicara en que caracter se quedo al bajar el texto a la siguiente linea
                for (int longitudTexto = texto.Length; longitudTexto > maxChar; longitudTexto -= maxChar)
                {
                    //Agregamos los fragmentos que salgan del texto
                    linea.AppendLine(texto.Substring(caracterActual, maxChar));
                    caracterActual += maxChar;
                }
                //Variable para poner espacios restntes
                string espacios = "";
                //sacamos la cantidad de espacios libres y el resultado lo dividimos entre dos
                int centrar = (maxChar - texto.Substring(caracterActual, texto.Length - caracterActual).Length) / 2;
                //Obtenemos la longitud del texto restante.
                for (int i = 0; i < centrar; i++)
                {
                    espacios += " ";//Agrega espacios para centrar
                }

                //agregamos el fragmento restante, agregamos antes del texto los espacios
                linea.AppendLine(espacios + texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                string espacios = "";
                //sacamos la cantidad de espacios libres y el resultado lo dividimos entre dos
                int centrar = (maxChar - texto.Length) / 2;
                //Obtenemos la longitud del texto restante.
                for (int i = 0; i < centrar; i++)
                {
                    espacios += " ";//Agrega espacios para centrar
                }

                //agregamos el fragmento restante, agregamos antes del texto los espacios
                linea.AppendLine(espacios + texto);

            }
        }

        //Metodo para poner texto a los extremos
        public void TextoExtremos(string textoIzquierdo, string textoDerecho)
        {
            //variables que utilizaremos
            string textoIzq, textoDer, textoCompleto = "", espacios = "";

            //Si el texto que va a la izquierda es mayor a 18, cortamos el texto.
            if (textoIzquierdo.Length > 22)
            {
                cortar = textoIzquierdo.Length - 22;
                textoIzq = textoIzquierdo.Remove(22, cortar);
            }
            else
            { textoIzq = textoIzquierdo; }

            textoCompleto = textoIzq;//Agregamos el primer texto.

            if (textoDerecho.Length > 24)//Si es mayor a 20 lo cortamos
            {
                cortar = textoDerecho.Length - 24;
                textoDer = textoDerecho.Remove(24, cortar);
            }
            else
            { textoDer = textoDerecho; }

            //Obtenemos el numero de espacios restantes para poner textoDerecho al final
            int nroEspacios = maxChar - (textoIzq.Length + textoDer.Length);
            for (int i = 0; i < nroEspacios; i++)
            {
                espacios += " ";//agrega los espacios para poner textoDerecho al final
            }
            textoCompleto += espacios + textoDerecho;//Agregamos el segundo texto con los espacios para alinearlo a la derecha.
            linea.AppendLine(textoCompleto);//agregamos la linea al ticket, al objeto en si.
        }

        //Creamos el encabezado para los articulos
        public void EncabezadoVenta()
        {
            //Escribimos los espacios para mostrar el articulo. En total tienen que ser 40 caracteres
            linea.AppendLine("ITEM                          |CANT  |PRECIO    ");
        }

        //Metodo para agregar los totales d ela venta
        public void AgregarTotales(string texto, decimal total)
        {
            //Variables que usaremos
            string resumen, valor, textoCompleto, espacios = "";

            if (texto.Length > 29)//Si es mayor a 25 lo cortamos
            {
                cortar = texto.Length - 29;
                resumen = texto.Remove(29, cortar);
            }
            else
            { resumen = texto; }

            textoCompleto = resumen;
            valor = total.ToString("c");//Agregamos el total previo formateo.

            //Obtenemos el numero de espacios restantes para alinearlos a la derecha
            int nroEspacios = maxChar - (resumen.Length + valor.Length);
            //agregamos los espacios
            for (int i = 0; i < nroEspacios; i++)
            {
                espacios += " ";
            }
            textoCompleto += espacios + valor;
            linea.AppendLine(textoCompleto);
        }

        //Metodo para agreagar articulos al ticket de venta
        public void AgregaArticulo(string articulo, int cant, decimal precio)
        {
            //Valida que cant precio e importe esten dentro del rango.
            if (cant.ToString().Length <= 5 && precio.ToString("c").Length <= 13)
            {
                string elemento = "", espacios = "";
                bool bandera = false;//Indicara si es la primera linea que se escribe cuando bajemos a la segunda si el nombre del articulo no entra en la primera linea
                int nroEspacios = 0;

                //Si el nombre o descripcion del articulo es mayor a 20, bajar a la siguiente linea
                if (articulo.Length > 24)
                {
                    //Colocar la cantidad a la derecha.
                    nroEspacios = (5 - cant.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " ";//Generamos los espacios necesarios para alinear a la derecha
                    }
                    elemento += espacios + cant.ToString();//agregamos la cantidad con los espacios

                    //Colocar el precio a la derecha.
                    nroEspacios = (13 - precio.ToString("c").Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " ";//Genera los espacios
                    }
                    //el operador += indica que agregar mas cadenas a lo que ya existe.
                    elemento += espacios + precio.ToString("c");//Agregamos el precio a la variable elemento

                    int caracterActual = 0;//Indicara en que caracter se quedo al bajae a la siguiente linea

                    //Por cada 20 caracteres se agregara una linea siguiente
                    for (int longitudTexto = articulo.Length; longitudTexto > 24; longitudTexto -= 24)
                    {
                        if (bandera == false)//si es false o la primera linea en recorrerer, continuar...
                        {
                            //agregamos los primeros 20 caracteres del nombre del articulos, mas lo que ya tiene la variable elemento
                            linea.AppendLine(articulo.Substring(caracterActual, 24) + elemento);
                            bandera = true;//cambiamos su valor a verdadero
                        }
                        else
                            linea.AppendLine(articulo.Substring(caracterActual, 24));//Solo agrega el nombre del articulo

                        caracterActual += 24;//incrementa en 20 el valor de la variable caracterActual
                    }
                    //Agrega el resto del fragmento del  nombre del articulo
                    linea.AppendLine(articulo.Substring(caracterActual, articulo.Length - caracterActual));

                }
                else //Si no es mayor solo agregarlo, sin dar saltos de lineas
                {
                    for (int i = 0; i < (24 - articulo.Length); i++)
                    {
                        espacios += " "; //Agrega espacios para completar los 20 caracteres
                    }
                    elemento = articulo + espacios;

                    //Colocar la cantidad a la derecha.
                    nroEspacios = (5 - cant.ToString().Length);// +(20 - elemento.Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " ";
                    }
                    elemento += espacios + cant.ToString();

                    //Colocar el precio a la derecha.
                    nroEspacios = (13 - precio.ToString("c").Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " ";
                    }
                    elemento += espacios + precio.ToString("c");

                    linea.AppendLine(elemento);//Agregamos todo el elemento: nombre del articulo, cant, precio, importe.
                }
            }
            else
            {
                linea.AppendLine("Los valores ingresados para esta fila");
                linea.AppendLine("superan las columnas soportdas por éste.");
                throw new Exception("Los valores ingresados para algunas filas del ticket\nsuperan las columnas soportdas por éste.");
            }
        }

        //Metodos para enviar secuencias de escape a la impresora
        //Para cortar el ticket
        public void CortaTicket()
        {
            linea.AppendLine("\x1B" + "m"); //Caracteres de corte. Estos comando varian segun el tipo de impresora
            linea.AppendLine("\x1B" + "d" + "\x00"); //Avanza 9 renglones, Tambien varian
        }

        //Para abrir el cajon
        public void AbreCajon()
        {
            //Estos tambien varian, tienen que ever el manual de la impresora para poner los correctos.
            linea.AppendLine("\x1B" + "p" + "\x00" + "\x0F" + "\x96"); //Caracteres de apertura cajon 0
            //linea.AppendLine("\x1B" + "p" + "\x01" + "\x0F" + "\x96"); //Caracteres de apertura cajon 1
        }

        //Para mandara a imprimir el texto a la impresora que le indiquemos.
        public void ImprimirTicket(string impresora)
        {
            //Este metodo recibe el nombre de la impresora a la cual se mandara a imprimir y el texto que se imprimira.
            //Usaremos un código que nos proporciona Microsoft. https://support.microsoft.com/es-es/kb/322091

            RawPrinterHelper.SendStringToPrinter(impresora, linea.ToString()); //Imprime texto.
            //linea.Clear();//Al cabar de imprimir limpia la linea de todo el texto agregado.
        }
    }

    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "Ticket de Venta";//Este es el nombre con el que guarda el archivo en caso de no imprimir a la impresora fisica.
            di.pDataType = "RAW";//de tipo texto plano
            //di.pOutputFile = "D:\\ticket.txt";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }

}
