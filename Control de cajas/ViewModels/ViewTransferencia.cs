using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;
using Control_de_cajas.Modelo;
using System.Collections.ObjectModel;


namespace Control_de_cajas.ViewModels
{
    class ViewTransferencia:ViewModelBase
    {
        private User actualUser;
        
        public ObservableCollection<Cashbox> SenderBoxs { get; set; }
        public ObservableCollection<Cashbox> AdressedBoxs { get; set; }

        private Cashbox _senderBoxSelected;
        public Cashbox SenderBoxSelected
        {
            get { return _senderBoxSelected; }
            set
            {
                _senderBoxSelected = value;
                OnPropertyChanged("SenderBoxSelected");
                DefineRecipients();
            }
        }

        private Cashbox _adressedBoxSelected;
        public Cashbox AdressedBoxSelected
        {
            get { return _adressedBoxSelected; }
            set { _adressedBoxSelected = value; OnPropertyChanged("AdressedBoxSelected"); }
        }

        private decimal _amountToTransfer;
        public decimal AmountToTransfer
        {
            get { return _amountToTransfer; }
            set { _amountToTransfer = value; OnPropertyChanged("AmountToTransfer"); }
        }

        private string _errorInAmount;
        public string ErrorInAmount
        {
            get { return _errorInAmount; }
            private set { _errorInAmount = value; OnPropertyChanged("ErrorInAmount"); }
        }

        public Command MakeTransferCmd { get; private set; }

        public ViewTransferencia()
        {
            SenderBoxs = new ObservableCollection<Cashbox>();
            AdressedBoxs = new ObservableCollection<Cashbox>();
            MakeTransferCmd = new Command(MakeTransfer, ValidateTransfer);
        }

        public void SetParameters(User user, List<Cashbox> boxs)
        {
            SenderBoxs.Clear();
            AdressedBoxs.Clear();
            actualUser = user;

            if (user != null)
            {
                foreach(Cashbox box in boxs)
                {
                    SenderBoxs.Add(box);
                }
            }

        }

        private void DefineRecipients()
        {
            AdressedBoxs.Clear();
            if(SenderBoxSelected != null)
            {
                foreach(Cashbox box in SenderBoxs)
                {
                    if(box.ID != SenderBoxSelected.ID)
                    {
                        AdressedBoxs.Add(box);
                    }
                }
            }
        }

        private bool CheckErrorsInTransfer()
        {
            ErrorInAmount = null;

            if(AmountToTransfer == 0m)
            {
                ErrorInAmount = "El valor de la transferencia no puede ser cero";
                return false;
            }
            else if(AmountToTransfer > SenderBoxSelected.Balance)
            {
                ErrorInAmount = "El valor a transferir supera el saldo en caja";
                return false;
            }

            return true;


        }

        private bool ValidateTransfer()
        {
            if(SenderBoxSelected == null)
            {
                return false;
            }
            else if(AdressedBoxSelected == null)
            {
                return false;
            }

            return true;
        }

        private void MakeTransfer()
        {
            int userId = actualUser.ID;
            int senderId = SenderBoxSelected.ID;
            int adressedId = AdressedBoxSelected.ID;

            if(CheckErrorsInTransfer())
            {
                if (BDComun.MakeTransfer(userId, senderId, adressedId, AmountToTransfer))
                {
                    MostrarMensaje("Monto transferido");

                    BDComun.ReloadCashbox(SenderBoxSelected);
                    BDComun.ReloadCashbox(AdressedBoxSelected);
                    BDComun.ReloadUser(actualUser);

                    SenderBoxSelected = null;
                    AmountToTransfer = 0m;

                }
                else
                {
                    MostrarMensaje(BDComun.Error);
                }
            }
        }


    }
}
