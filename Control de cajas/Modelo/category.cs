using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;

namespace Control_de_cajas.Modelo
{
    class Category:Notificador
    {
        private int _id;
        public int ID => _id;

        private string _name;
        public string Name => _name;

        private int _categoryClass;
        public int CategoryClass => _categoryClass;

        private List<int> _subcategoriesIds;
        public List<int> SubcategoriesIds => _subcategoriesIds;

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value;  OnPropertyChanged("IsSelected"); }
        }

        public Category(int id, string name, int categoryClass)
        {
            _id = id;
            _name = name;
            _categoryClass = categoryClass;

            
        }

        public void AddSubcategories(List<int> subcategories)
        {
            _subcategoriesIds = subcategories;
        }

        public override string ToString()
        {
            return Name + "(" + CategoryClass + ")";
        }
    }
}
