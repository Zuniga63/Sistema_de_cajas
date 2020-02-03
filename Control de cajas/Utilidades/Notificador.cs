/*
 * Created by SharpDevelop.
 * User: Admon
 * Date: 08/30/2017
 * Time: 10:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace Utilidades
{
	/// <summary>
	/// Description of Notificador.
	/// </summary>
    [Serializable]
	public class Notificador:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
	}
}
