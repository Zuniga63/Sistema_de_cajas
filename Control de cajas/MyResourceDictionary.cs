using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Control_de_cajas
{
    partial class MyResourceDictionary:ResourceDictionary
    {
        static bool conFoco_txtBuscar;

        public MyResourceDictionary()
        {
            InitializeComponent();
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox c = (TextBox)sender;
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                c.SelectAll();
                conFoco_txtBuscar = true;
            }
        }

        private void Txt_GotMouseFocus(object sender, MouseEventArgs e)
        {
            TextBox c = (TextBox)sender;
            if (!conFoco_txtBuscar && c.SelectionLength == 0)
            {
                conFoco_txtBuscar = true;
                c.SelectAll();
            }
        }

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox c = (TextBox)sender;
            conFoco_txtBuscar = false;
            c.SelectionLength = 0;
        }

    }
}
