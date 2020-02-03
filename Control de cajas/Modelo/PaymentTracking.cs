using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Control_de_cajas.Modelo
{
    class PaymentTracking
    {
        private static int paymentPoints;
        private static int prepaymentPoints;
        private static int latePaymentPoints;

        private DateTime _cutoffDate;
        /// <summary>
        /// Es la fecha en la cual se esperaba un pago
        /// </summary>
        public DateTime CutoffDate => _cutoffDate;

        private DateTime _paymentDate;
        /// <summary>
        /// Fecha en la cula se realizó el pago
        /// </summary>
        public DateTime PaymentDate => _paymentDate;

        private decimal _amountDebt;
        /// <summary>
        /// El saldo de la deuda al momento del pago 
        /// </summary>
        public decimal AmountDebt => _amountDebt;

        private decimal _amountPayment;
        /// <summary>
        /// Es el valor del pago realizado
        /// </summary>
        public decimal AmountPayment => _amountPayment;

        private double _paymentPercentage;
        /// <summary>
        /// Es el porcentaje del pago realizado
        /// </summary>
        public double PaymentPercentage => _paymentPercentage;

        private int _daysPrepayment;
        /// <summary>
        /// Son los dias de pronto pago del cliente
        /// </summary>
        public int DaysPrepayment => _daysPrepayment;

        private int _daysLatePayment;
        public int DaysLatePayment => _daysLatePayment;

        private int _points;
        /// <summary>
        /// Son los puntos obtenidos por esta transaccion
        /// </summary>
        public int Points => _points;

        public static void DefineParameters(int pPoint, int ppPoints, int lpPoints)
        {
            paymentPoints = pPoint;
            prepaymentPoints = ppPoints;
            latePaymentPoints = lpPoints;
        }

        public PaymentTracking(DateTime cutoffDate, DateTime paymentDate, decimal amountDebt, decimal amountPayment)
        {
            _cutoffDate = cutoffDate;
            _paymentDate = paymentDate;
            _amountDebt = amountDebt;
            _amountPayment = amountPayment;
            _paymentPercentage = (double) (_amountPayment / _amountDebt);
            
            //Ahora se define los días de pronto pago
            if(cutoffDate>paymentDate)
            {
                _daysPrepayment = (int) cutoffDate.Subtract(paymentDate).TotalDays;
            }
            else
            {
                _daysLatePayment = (int)paymentDate.Subtract(cutoffDate).TotalDays;
            }

            //Ahora se calculan los puntos
            _points = paymentPoints + (_daysPrepayment * prepaymentPoints) + (_daysLatePayment * latePaymentPoints);

            if(Points>0)
            {
                _points = (int) (Points * PaymentPercentage);
            }
            else
            {
                _points = (int)(Points * (1 - PaymentPercentage));
            }
            
        }

    }
}
