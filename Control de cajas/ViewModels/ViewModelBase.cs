using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;
using Control_de_cajas.Modelo;
using System.Windows;

namespace Control_de_cajas.ViewModels
{
    class ViewModelBase:Notificador
    {
        /// <summary>
        /// Este metodo establece los parametros de la base de datos
        /// </summary>
        protected void EstablecerParametrosDeLaBD()
        {
            BDComun.UsserID = "root";
            BDComun.Server = "localhost";
            BDComun.DataBase = "mi_sistema_de_cajas";
            BDComun.Password = "clave1234";
        }

        

        protected void MostrarMensaje(string message)
        {
            MessageBox.Show(message);
        }

        protected bool PreguntarUsuario(string message, string title)
        {
            MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.OKCancel);
            switch(result)
            {
                case MessageBoxResult.OK:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
                default:
                    return false;
            }
        }

        protected void CategoryIsSelected(Category beforeValue, Category actualValue)
        {
            if(beforeValue != null)
            {
                beforeValue.IsSelected = false;
            }

            if(actualValue != null)
            {
                actualValue.IsSelected = true;
            }
        }

        protected void UserIsSelected(User beforeValue, User actualValue)
        {
            if (beforeValue != null)
            {
                beforeValue.IsSelected = false;
            }

            if (actualValue != null)
            {
                actualValue.IsSelected = true;
            }
        }

        protected void CashboxIsSelected(Cashbox beforeValue, Cashbox actualValue)
        {
            if (beforeValue != null)
            {
                beforeValue.IsSelected = false;
            }

            if (actualValue != null)
            {
                actualValue.IsSelected = true;
            }
        }
    }
}
