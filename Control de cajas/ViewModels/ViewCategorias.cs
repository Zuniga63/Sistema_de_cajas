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
    class ViewCategorias:ViewModelBase
    {
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
        public bool IsEnabled
        {
            get { return _isEnabled; }
            private set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        public ObservableCollection<User> Users { get; private set; }

        private User _userSelected;
        /// <summary>
        /// Al actualizar al usuario seleccionado, esta propiedad manda la orden para 
        /// recargar la clase.
        /// </summary>
        public User UserSelected
        {
            get { return _userSelected; }
            set
            {
                _userSelected = value;
                OnPropertyChanged("UserSelected");
                ReloadClass();
            }
        }

        
        /// <summary>
        /// Es un listado con todas las categorias independientes de su rango, ordenadas por la clase
        /// </summary>
        private List<Category> allCategories;
        private List<Category> allMainCategories;
        private List<Category> allSubcategories;
        private List<Category> allAvailableCategories;

        /// <summary>
        /// Esta es la lista de las categorias seleccionada por el usuarios de manera descendente
        /// </summary>
        private List<Category> categoryRoute;

        private string _categoriesRoute;
        public string CategoryRoute
        {
            get { return _categoriesRoute; }
            private set { _categoriesRoute = value; OnPropertyChanged("CategoryRoute"); }
        }

        private int _currentClass;
        /// <summary>
        /// Notifica la clase actual a la cual se agregan nuevas categorías, por defecto tiene valor de 1
        /// </summary>
        public int CurrentClass
        {
            get { return _currentClass; }
            private set { _currentClass = value; OnPropertyChanged("CurrentClass"); }
        }

        /// <summary>
        /// Contiene a las categorías principales del usuario, son la raiz de toda las rutas posibles
        /// </summary>
        public ObservableCollection<Category> MainCategories { get; set; }

        /// <summary>
        /// Es un listado con las sugcategorías de la ultima categoría de la ruta
        /// </summary>
        public ObservableCollection<Category> Subcategories { get; set; }

        public ObservableCollection<Category> AvailableCategories { get; set; }

        #region BUSQUEDA Y SELECCION DE CATEGORÍAS POR NOMBRE
        private string _mainCategoryName;
        /// <summary>
        /// Es el nombre de la categoría a la cual se procede a realizar la busqueda
        /// </summary>
        public string MainCategoryName
        {
            get { return _mainCategoryName; }
            set
            {
                _mainCategoryName = value;
                OnPropertyChanged("MainCategoryName");
                SearchByMainCategoryName(value);
            }
        }

        private string _subcategoryName;
        public string SubcategoryName
        {
            get { return _subcategoryName; }
            set
            {
                _subcategoryName = value;
                OnPropertyChanged("SubcategoryName");
                SearchBySubcategoryName(value);
            }
        }

        private string _avalilableCategoryName;
        public string AvailableCategoryName
        {
            get { return _avalilableCategoryName; }
            set
            {
                _avalilableCategoryName = value;
                OnPropertyChanged("AvailableCategoryName");
                SearchByAvailableCategoryName(value);
            }
        }

        /// <summary>
        /// Este metodo recarga la lista de categorias principales segun el nombre pasado como parametros 
        /// desde la lista privada llamada allMaincategories
        /// </summary>
        /// <param name="name"></param>
        private void SearchByMainCategoryName(string name)
        {
            MainCategories.Clear();

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
            {
                foreach (Category c in allMainCategories)
                {
                    if (c.Name.ToUpper().Contains(name.ToUpper()))
                    {
                        MainCategories.Add(c);
                    }
                }
            }
            else
            {
                foreach (Category c in allMainCategories)
                {
                    MainCategories.Add(c);
                }
            }
        }

        private void SearchBySubcategoryName(string name)
        {
            Subcategories.Clear();

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
            {
                foreach (Category c in allSubcategories)
                {
                    if (c.Name.ToUpper().Contains(name.ToUpper()))
                    {
                        Subcategories.Add(c);
                    }
                }
            }
            else
            {
                foreach (Category c in allSubcategories)
                {
                    Subcategories.Add(c);
                }
            }

        }

        private void SearchByAvailableCategoryName(string name)
        {
            AvailableCategories.Clear();

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
            {
                foreach (Category c in allAvailableCategories)
                {
                    if (c.Name.ToUpper().Contains(name.ToUpper()))
                    {
                        AvailableCategories.Add(c);
                    }
                }
            }
            else
            {
                foreach (Category c in allAvailableCategories)
                {
                    AvailableCategories.Add(c);
                }
            }
        }

        #endregion

        private Category _mainCategorySelected;
        /// <summary>
        /// Al selecionar una categoría, lanza los metodos encargados de actualizar la view
        /// </summary>
        public Category MainCategorySelected
        {
            get { return _mainCategorySelected; }
            set
            {
                if(value != _mainCategorySelected)
                {
                    CategoryIsSelected(_mainCategorySelected, value);
                    _mainCategorySelected = value;

                    MainCategoryIsChange();
                    OnPropertyChanged("MainCategorySelected");
                    //Metodo que actualiza las rutas de categorias
                }
            }
        }

        private Category _subcategorySelected;
        public Category SubcategorySelected
        {
            get { return _subcategorySelected; }
            set
            {
                CategoryIsSelected(_subcategorySelected, value);
                _subcategorySelected = value;
                OnPropertyChanged("SubcategorySelected");
            }
        }

        private Category _availaibleCategorySelected;
        public Category AvailableCategorySelected
        {
            get { return _availaibleCategorySelected; }
            set
            {
                CategoryIsSelected(_availaibleCategorySelected, value);
                _availaibleCategorySelected = value;
                OnPropertyChanged("AvailableCategorySelected");
            }
        }

        private string _newMainCategoryName;
        public string NewMainCategoryName
        {
            get { return _newMainCategoryName; }
            set { _newMainCategoryName = value; OnPropertyChanged("NewMainCategoryName"); }
        }

        private string _errorInNewMainCategoryName;
        public string ErrorInNewMainCategoryName
        {
            get { return _errorInNewMainCategoryName; }
            private set { _errorInNewMainCategoryName = value; OnPropertyChanged("ErrorInNewMainCategoryName"); }
        }

        private string _newSubcategoryName;
        public string NewSubcategoryName
        {
            get { return _newSubcategoryName; }
            set { _newSubcategoryName = value; OnPropertyChanged("NewSubcategoryName"); }
        }

        private string _errorInNewSubcategoryName;
        public string ErrorInNewSubcategoryName
        {
            get { return _errorInNewSubcategoryName; }
            private set { _errorInNewSubcategoryName = value; OnPropertyChanged("ErrorInNewMainCategoryName"); }
        }

        private bool _creatingMainCategory;
        public bool CreatingMainCategory
        {
            get { return _creatingMainCategory; }
            private set { _creatingMainCategory = value; OnPropertyChanged("CreatingMainCategory"); }
        }

        private bool _creatinSubcategory;
        public bool CreatingSubcategory
        {
            get { return _creatinSubcategory; }
            private set { _creatinSubcategory = value; OnPropertyChanged("CreatingSubcategory"); }
        }

        public Command NewMainCategoryCmd { get; private set; }
        public Command NewSubcatetoryCmd { get; private set; }
        public Command CreateMainCategoryCmd { get; private set; }
        public Command CreateSubcategoryCmd { get; private set; }
        public Command CancelOperationCmd { get; private set; }
        public Command AddToRouteCmd { get; private set; }
        public Command RemoveToRouteCmd { get; private set; }
        public Command MatchCategoryCmd { get; private set; }

        public ViewCategorias()
        {
            InicializarCommands();
            InicializarListas();
            ReloadUsers();
            ReloadCategories();
            RecoveryRelationList();
            IsEnabled = true;
        }

        private void InicializarListas()
        {
            allCategories = new List<Category>();
            allMainCategories = new List<Category>();
            allSubcategories = new List<Category>();
            allAvailableCategories = new List<Category>();
            categoryRoute = new List<Category>();

            Users = new ObservableCollection<User>();
            MainCategories = new ObservableCollection<Category>();
            Subcategories = new ObservableCollection<Category>();
            AvailableCategories = new ObservableCollection<Category>();
        }

        private void InicializarCommands()
        {
            NewMainCategoryCmd = new Command(() => WaitingState(true), () => UserSelected != null);
            NewSubcatetoryCmd = new Command(() => WaitingState(false), () => (UserSelected != null && CurrentClass > 1));
            CreateMainCategoryCmd = new Command(CreateNewMainCategory, () => NewMainCategoryName != null);
            CreateSubcategoryCmd = new Command(CreateNewSubcategory, () => NewSubcategoryName != null);
            CancelOperationCmd = new Command(ActivateState, () => !IsEnabled);
            AddToRouteCmd = new Command(AddCategoryToRoute, ValidateAddCategoryToRoute);
            RemoveToRouteCmd = new Command(RemoveCategoryToRoute, ValidateRemoveCategoryToRoute);
            MatchCategoryCmd = new Command(MatchCategory, ()=> (categoryRoute.Count > 0 && AvailableCategorySelected != null));
        }

        /// <summary>
        /// Este metodo recarga de la base de datos la informacion de los usuarios
        /// </summary>
        private void ReloadUsers()
        {
            Users.Clear();

            foreach (User user in BDComun.ReadAllUser())
            {
                Users.Add(user);
            }
            
        }

        /// <summary>
        /// Este metodo consulta a la base de datos todas las categorías que esta contiene y las
        /// recarga en las listas que la vista utiliza, dependiend del usuario
        /// </summary>
        private void ReloadCategories()
        {
            allCategories.Clear();
            allMainCategories.Clear();
            allSubcategories.Clear();
            allAvailableCategories.Clear();

            if (UserSelected != null)
            {
                CategorySystem.DefineParameter(UserSelected.ID);
                allMainCategories = CategorySystem.RecoveryMainCategory();
            }

            MainCategoryName = null;            //Al ponerlo en null debe actualizar la lista MainCategories
            SubcategoryName = null;             //Al darle valor null debe actualizar la lista de subcategorias
            AvailableCategoryName = null;       //Al darle valor null debe actualizar la lista de AvailableCategories
        }

        /// <summary>
        /// Este metodo se debe lanzar cuando se cambia el usuario seleccionado
        /// </summary>
        private void ReloadClass()
        {
            ReloadCategories();                 //Con esto actualizo el listado interno de categorias
            categoryRoute.Clear();         //Con esto reinicio la ruta interna de categorias 
            WriteCategoryRoute();               //Se reinicia el mensaje al usuario
            CurrentClass = 1;
        }

        /// <summary>
        /// Crea una cadena con la ruta actual de categorias para el usuario
        /// </summary>
        private void WriteCategoryRoute()
        {
            CategoryRoute = null;
            bool primero = true;

            foreach (Category c in categoryRoute)
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

        /// <summary>
        /// Este metodo comprueba que el nombre de la categoría no esté en blanco y luego
        /// verifica que el nombre que se intenta agregar sea unico
        /// </summary>
        /// <returns></returns>
        private bool ValidateCreateNewMainCategory()
        {
            ErrorInNewMainCategoryName = null;
            
            
            if(string.IsNullOrEmpty(NewMainCategoryName) || string.IsNullOrWhiteSpace(NewMainCategoryName))
            {
                ErrorInNewMainCategoryName = "Este campo es obligatorio";
                return false;
            }

            foreach(Category c in allMainCategories)
            {
                if (c.Name.ToUpper() == NewMainCategoryName.ToUpper())
                {
                    ErrorInNewMainCategoryName = "Este nombre ya está en uso";
                    return false;
                }
            }

            return true;
        }//Fin del metodo

        /// <summary>
        /// Crea la categoría en la base de datos
        /// </summary>
        private void CreateNewMainCategory()
        {
            if(ValidateCreateNewMainCategory())
            {
                if(BDComun.CreateCategory(NewMainCategoryName, 1, UserSelected.ID))
                {
                    NewMainCategoryName = null;
                    ReloadRelationList();                 //Como solo se crea una nueva se puede seguir usando la lista de relaciones
                    ActivateState();
                    MostrarMensaje("Categoría creada correctamente");
                }
                else
                {
                    ActivateState();
                    MostrarMensaje(BDComun.Error);
                }//FIn de else
            }
        }

        private bool ValidateCreateNewSubcategory()
        {
            ErrorInNewSubcategoryName = null;
            if (string.IsNullOrEmpty(NewSubcategoryName) || string.IsNullOrWhiteSpace(NewSubcategoryName))
            {
                ErrorInNewSubcategoryName = "Este campo es obligatorio";
                return false;
            }

            bool uniqueName = CategorySystem.ValidateCategoryName(NewSubcategoryName, CurrentClass);

            if(!uniqueName)
            {
                ErrorInNewSubcategoryName = "Este nombre ya está en uso";
                return false;
            }

            return true;
        }

        private void CreateNewSubcategory()
        {
            if (ValidateCreateNewSubcategory())
            {
                if(BDComun.CreateCategory(NewSubcategoryName, CurrentClass, UserSelected.ID))
                {
                    NewSubcategoryName = null;
                    ReloadRelationList();
                    ActivateState();
                    MostrarMensaje("Categoría creada correctamente");
                }
                else
                {
                    ActivateState();
                    MostrarMensaje(BDComun.Error);
                }
            }
        }

        private void WaitingState(bool mainCategory)
        {
            IsEnabled = false;

            if(mainCategory)
            {
                CreatingMainCategory = true;
            }
            else
            {
                CreatingSubcategory = true;
            }
        }

        private void ActivateState()
        {
            if(CreatingMainCategory)
            {
                CreatingMainCategory = false;
            }

            if(CreatingSubcategory)
            {
                CreatingSubcategory = false;
            }

            IsEnabled = true;
        }

        /// <summary>
        /// Este metodo va construyendo las listas de relaciones entre las distintas categorias
        /// </summary>
        private void RecoveryRelationList()
        {
            int fatherClass = 1;
            List<Category> categories = null;
            Category father = null;

            //Se limpian las listas
            allSubcategories.Clear();
            allAvailableCategories.Clear();

            if (categoryRoute.Count > 0)
            {
                fatherClass = categoryRoute.Count;
                father = categoryRoute[fatherClass - 1];
                allSubcategories = CategorySystem.RecoverySubcategories(father);

                categories = CategorySystem.RecoveryCategoryByClass(fatherClass + 1);
                
                for(int indexTemporal = 0; indexTemporal<categories.Count; indexTemporal++)
                {
                    bool relacionado = false;
                    Category c = categories[indexTemporal];

                    for(int indexSubcat = 0; indexSubcat < allSubcategories.Count; indexSubcat++)
                    {
                        if(c.ID == allSubcategories[indexSubcat].ID)
                        {
                            relacionado = true;
                            break;
                        }
                    }

                    if(!relacionado)
                    {
                        allAvailableCategories.Add(categories[indexTemporal]);
                    }
                }
            }


            SubcategoryName = null;
            AvailableCategoryName = null;

        }//Fin del metodo

        /// <summary>
        /// Este metodo se ejecuta cuando se ha elegido una categoría principal de la lista de MainCategories
        /// </summary>
        private void MainCategoryIsChange()
        {
            categoryRoute.Clear();

            if (MainCategorySelected != null)
            {
                categoryRoute.Add(MainCategorySelected);
                CurrentClass = 2;
            }
            else
            {
                CurrentClass = 1;
            }
            
            WriteCategoryRoute();
            RecoveryRelationList();
        }
       
        private bool ValidateAddCategoryToRoute()
        {
            if(MainCategorySelected != null)
            {
                if(SubcategorySelected != null)
                {
                    return true;
                }
            }

            return false;
        }
        
        private void AddCategoryToRoute()
        {
            categoryRoute.Add(SubcategorySelected);
            CurrentClass++;
            WriteCategoryRoute();
            RecoveryRelationList();
        }

        private bool ValidateRemoveCategoryToRoute()
        {
            if (CurrentClass > 2)
                return true;
            return false;
        }

        private void RemoveCategoryToRoute()
        {
            categoryRoute.RemoveAt(categoryRoute.Count - 1);
            CurrentClass--;
            WriteCategoryRoute();
            RecoveryRelationList();
        }



        /// <summary>
        /// Este metodo es utilizado cuando se crea una nueva relacion o cuando se elimina
        /// </summary>
        private void ReloadRelationList()
        {
            List<Category> copy = categoryRoute.ToList();               //Guardo una copia de la ruta actual
            categoryRoute.Clear();                                      //Limpio el listado de la ruta actual
            CurrentClass = 1;                                           //Reinicio el contador de la clase porque no hay ninguna clase seleccionada
            ReloadCategories();                                         //Recupero las categorias de la base de datos
            MainCategoryName = null;
            
                                                                        //Se vuelve a montar las categorias copiadas
            if(copy.Count>0)
            {
                int mainCategoryId = copy[0].ID;
                foreach(Category c in MainCategories)
                {
                    if(c.ID == mainCategoryId)
                    {
                        MainCategorySelected = c;
                        break;
                    }
                }

                for(int index = 1; index<copy.Count; index++)
                {
                    Category c = copy[index];

                    foreach (Category c2 in Subcategories)
                    {
                        if (c2.ID == c.ID)
                        {
                            SubcategorySelected = c2;
                            AddCategoryToRoute();
                            break;
                        }
                    }
                }
            }
        
            
        }

        private void MatchCategory()
        {
            int fatherID = categoryRoute[categoryRoute.Count - 1].ID;

            if(BDComun.MatchCategories(fatherID, AvailableCategorySelected.ID))
            {
                ReloadRelationList();
            }
        }

        private void RemoveRelationship()
        {
            int fatherID = categoryRoute[categoryRoute.Count - 1].ID;

            if(BDComun.RemoveRlationship(fatherID, SubcategorySelected.ID))
            {
                ReloadRelationList();
            }
        }
    }
}
