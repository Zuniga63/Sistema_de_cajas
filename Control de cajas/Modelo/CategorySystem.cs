using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_de_cajas.Modelo
{
    class CategorySystem
    {
        private static List<Category> allCategories;
        private static List<Category> fatherCategories;
        public static List<Category> subcategories;
        public static List<Category> availableCategories;

        public static void DefineParameter(int? userId)
        {
            if(userId.HasValue)
            {
                allCategories = BDComun.ReadAllCategories(userId.Value);
                fatherCategories = RecoveryMainCategory();
            }
            else
            {
                allCategories = new List<Category>();
                fatherCategories = new List<Category>();
            }
            
            subcategories = new List<Category>();
            availableCategories = new List<Category>();
        }

        public static List<Category> GetFatherCategories()
        {
            return fatherCategories.ToList();
        }

        /// <summary>
        /// Este nmetodo recupera todas la categorías de rango 1 y las agrega a la lista de categorias padre
        /// </summary>
        public static List<Category> RecoveryMainCategory()
        {
            List<Category> result = new List<Category>();

            foreach (Category c in allCategories)
            {
                if (c.CategoryClass == 1)
                {
                    result.Add(c);
                }
                else if (c.CategoryClass > 1)
                {
                    break;
                }
            }//Fin de foreach

            return result;
        }//Fin del metodo

        /// <summary>
        /// Este metodo estatico retona un listado con las subcategorias de la categoría pasada como parametro
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static List<Category> RecoverySubcategories(Category category)
        {
            List<Category> result = new List<Category>();
            List<int> sons = category.SubcategoriesIds;

            foreach(int id in sons)
            {
                foreach(Category c in allCategories)
                {
                    if(c.ID == id)
                    {
                        result.Add(c);
                        break;
                    }
                }
            }

            return result;
        }

        public static List<Category> RecoveryCategoryByClass(int categoryClass)
        {
            List<Category> result = new List<Category>();
            foreach(Category c in allCategories)
            {
                if(c.CategoryClass == categoryClass)
                {
                    result.Add(c);
                }
                else if(c.CategoryClass>categoryClass)
                {
                    break;
                }
            }

            return result;
        }

        public static bool ValidateCategoryName(string name, int categoryClass)
        {
            foreach(Category c in allCategories)
            {
                if(c.Name.ToUpper()==name.ToUpper() && c.CategoryClass == categoryClass)
                {
                    return false;
                }

                if(c.CategoryClass>categoryClass)
                {
                    break;
                }
            }

            return true;
        }
    }
}
