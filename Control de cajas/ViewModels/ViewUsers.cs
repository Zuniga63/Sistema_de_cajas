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
    class ViewUsers:ViewModelBase
    {
        private enum AuxiliarView { NewUser, NewBox, NewTramsaction, ModifyTransaction, CategoryGestion}              //Para identificar las vista auxiliares 
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

        public Command RemoveCategoryCmd { get; private set; }
        public Command AddNewTransactionCmd { get; private set; }

        public Command ActivateViewNewUser { get; private set; }
        public Command ActivateViewNewBox { get; private set; }
        public Command ActivateModifyTransaction { get; set; }
        public Command ActivateCategoryGestion { get; set; }
        public Command CancelOperation { get; private set; }
        public Command ModifyTransactionsCmd { get; private set; }

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
        }

        private void InicializarComandos()
        {
            CreateNewUserCmd = new Command(CreateNewUser, () => true);
            DeleteUserCmd = new Command(DeleteUser, () => UserSelected != null);
            CreateNewCashboxCmd = new Command(CreateNewCasbox, ()=> UserSelected != null);
            RemoveCategoryCmd = new Command(RemoveCategory, () => CategoriesSelected.Count > 0);
            AddNewTransactionCmd = new Command(AddNewTransacction, ValidateCommandNewTransaction);
            ActivateViewNewUser = new Command(()=> WaitingState(AuxiliarView.NewUser), () => true);
            ActivateViewNewBox = new Command(() => WaitingState(AuxiliarView.NewBox), () => UserSelected != null);
            ActivateModifyTransaction = new Command(() => WaitingState(AuxiliarView.ModifyTransaction), ValidateActiveModifyTransactios);
            ActivateCategoryGestion = new Command(() => WaitingState(AuxiliarView.CategoryGestion), ()=> true);
            CancelOperation = new Command(ActivatState, () => !IsEnabled);
            ModifyTransactionsCmd = new Command(ModifyTransaction, () => true);
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
            if(string.IsNullOrEmpty(_nameOfNewUser) || string.IsNullOrWhiteSpace(_nameOfNewUser))
            {
                ErrorOfNewUser = "Espacio en blanco";
                return false;
            }

            //Se valida que el nombre sea unico
            foreach(User user in Users)
            {
                if(user.UserName.ToUpper() == _nameOfNewUser.ToUpper())
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
            if(ValidateNewUser())
            {
                if(BDComun.NewUser(_nameOfNewUser))
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
            if(PreguntarUsuario(message, "Eliminar usuario"))
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
            if(UserSelected != null)
            {
                foreach(Cashbox box in BDComun.ReadAllCashbox(UserSelected.ID))
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

            if(UserSelected != null)
            {
                CategorySystem.DefineParameter(UserSelected.ID);
                foreach(Category c in CategorySystem.GetFatherCategories())
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

            if(string.IsNullOrEmpty(_newCashBoxName) || string.IsNullOrEmpty(_newCashBoxName))
            {
                ErrorOfCashboxName = "Campo obligatorio";
                return false;
            }

            foreach(Cashbox box in Boxs)
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
            if(ValidateNewCashbox())
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
            if(UserSelected == null)
            {
                return false;
            }

            if(BoxSelected == null)
            {
                return false;
            }

            if(Categories.Count>0)
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

            if(BoxSelected == null)
            {
                ErrorOfBox = "Se debe seleccionar una caja";
                result = false;
            }

            if(!TransactionDate.HasValue)
            {
                ErrorOfDate = "Se debe eleguir una fecha valida";
                result = false;
            }

            if(CategoriesSelected.Count ==0)
            {
                ErrorOfCategory = "Se debe elegir una categoría";
                result = false;
            }
            else if(Categories.Count>0)
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
            if(ValidateNewTransaction())
            {
                int user_id = UserSelected.ID;
                int cashbox_id = BoxSelected.ID;
                List<int> categoriesId = new List<int>();
                foreach(Category c in CategoriesSelected)
                {
                    categoriesId.Add(c.ID);
                }
                

                if(BDComun.AddNewTransaction(user_id, cashbox_id, TransactionDate.Value, Description, Amount, categoriesId))
                {
                    Description = null;
                    LoadCategories();
                    Amount = 0;

                    BDComun.ReloadUser(UserSelected);
                    BDComun.ReloadCashbox(BoxSelected);
                    CashboxTransaction lastTransaction = BDComun.RecuperarUltimaTransaccion(user_id, cashbox_id);

                    if(lastTransaction != null)
                    {
                        allTransactions.Add(lastTransaction);
                        RechargeTransactions();
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
                currentDate = allTransactions[allTransactions.Count - 1].TransactionDate;
            }

            foreach (CashboxTransaction t in allTransactions)
            {
                if (t.TransactionDate >= currentDate)
                {
                    Transactions.Add(t);
                }
            }
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
                if(_amount<0)
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

            switch(aux)
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
                        if(UserSelected != null)
                        {
                            foreach(User u in ViewCategorias.Users)
                            {
                                if(UserSelected.UserName == u.UserName)
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

            if(CategoryGestionView)
            {
                string username = null;
                if (UserSelected != null)
                {
                    username = UserSelected.UserName;
                    IsSelected = true;
                    foreach(User u in Users)
                    {
                        if(u.UserName == username)
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
            if(UserSelected != null)
            {
                if(BoxSelected != null)
                {
                    if(allTransactions.Count>0)
                    {
                        if(TransactionSelected != null)
                        {
                            if(!TransactionSelected.IsATransfer)
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
            if(ModifyTransactionView.UpdateTransactionInBD())
            {
                ActivatState();
                MostrarMensaje("Transaccion modificada");
                BDComun.ReloadUser(UserSelected);
                BDComun.ReloadCashbox(BoxSelected);
                ReloadTransaction();
            }
        }
    }
}
