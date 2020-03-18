using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;
using Control_de_cajas.Modelo;

namespace Control_de_cajas.ViewModels
{
    class ViewModel:ViewModelBase
    {
        public ViewUsers ViewUser { get; set; }
        public ViewClientes ViewClientes { get; set; }

        public ViewModel()
        {
            EstablecerParametrosDeLaBD();
            ViewUser = new ViewUsers();
            ViewClientes = new ViewClientes();
            ViewUser.IsSelected = true;
            ViewClientes.IsSelected = false;
        }
    }
}
