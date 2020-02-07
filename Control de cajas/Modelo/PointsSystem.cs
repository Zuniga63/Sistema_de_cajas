using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_de_cajas.Modelo
{
    public enum PeriodoDeCobro { Semanal=7, Quincenal=15, Mensual=30}
    class PointsSystem
    {
        private static double _interesAnual;
        private static double _interesPeriodico;
        private static double _utilidad;
        
        public static DateTime? CurrentDate { get; private set; }
        public static decimal Debt { get; private set; }
        public static decimal Payment { get; private set; }
        public static decimal Balance { get; private set; }

        public static decimal DeudaReal;
        public static double DiasUltimoPago;
        public static decimal CostoFinanciero;
        public static decimal GananciaBruta;
        public static decimal GananciaNeta;
        public static double Points;
        

        
        
        private static void ReloadClass()
        {
            CurrentDate = null;
            Debt = 0m;
            Payment = 0m;
            Balance = 0;

            DeudaReal = 0m;
            DiasUltimoPago = 0;
            CostoFinanciero = 0;
            GananciaBruta = 0m;
            GananciaNeta = 0m;
            Points = 0d;
            
        }


        public static void EstablecerParametros(double interesAnual, double utilidad)
        {
            _interesAnual = interesAnual;
            _interesPeriodico = Math.Pow((1+interesAnual), (1.0d/365.0d)) - 1;
            _utilidad = utilidad;
        }

        
        public static List<CustomerTracking> DefineScore2(List<CustomerTransaction> transactions)
        {
            bool firtsTransaction = true;
            List<CustomerTracking> tracking = new List<CustomerTracking>();
            List<CustomerTransaction> normalizeTransactions = NormalizeTransactions(transactions);
            ReloadClass();

            foreach (CustomerTransaction t in normalizeTransactions)
            {
                //Se créa el primer registro se seguimiento (tracking)
                if(firtsTransaction)
                {
                    firtsTransaction = false;
                    CurrentDate = t.Fecha;
                    Payment = t.Abono;
                    Debt = t.Deuda;
                    Balance = Debt - Payment;

                    DeudaReal = Debt * (decimal)(1 + _utilidad);

                    //Se crea el registro de seguimiento y se agrega a la lista
                    
                }
                else
                {
                    //Se calcula el costo financiero teniendo en cunta los días que han pasado desde la ultima transaccion
                    //o conjunto de transacciones hasta la fecha de esta nueva transaccion
                    DiasUltimoPago = t.Fecha.Subtract(CurrentDate.Value).TotalDays;
                    CostoFinanciero += DeudaReal * (decimal)Math.Pow((1 + _interesPeriodico), DiasUltimoPago);

                    //Se actulizan los valores con respecto al seguimiento
                    CurrentDate = t.Fecha;
                    Debt += t.Deuda;
                    Payment = t.Abono;
                    Balance = Debt - Payment;
                    
                    if(t.Deuda>0)
                    {
                        DeudaReal += t.Deuda * (decimal)(1 + _utilidad);
                    }

                    if(t.Abono>0)
                    {
                        if (DeudaReal > t.Abono)
                        {
                            DeudaReal -= t.Abono;
                        }
                        else
                        {
                            GananciaBruta += t.Abono - DeudaReal;
                            DeudaReal = 0m;
                        }
                    }

                    GananciaNeta = GananciaBruta - CostoFinanciero;
                    Points = (double)GananciaNeta / 1000d;
                }//Fin de else
            }//Fin de foreach

            //Finalmente se actualiza la fecha desde el ultimo cobro
            DiasUltimoPago = DateTime.Now.Subtract(CurrentDate.Value).TotalDays;

            return tracking;
        }//Fin del metodo

        /// <summary>
        /// Este metodo se encarga de normalizar los movimientos con el onjetivo que el sistema de puntuacion pueda 
        /// establecerse correctamente, es decir, unificando los abonos y deudas ocurridos en un mismo día
        /// </summary>
        /// <param name="orignalTransactions"></param>
        /// <returns></returns>
        private static List<CustomerTransaction> NormalizeTransactions(List<CustomerTransaction> orignalTransactions)
        {
            List<CustomerTransaction> result = new List<CustomerTransaction>();
            decimal debt = 0m;
            decimal payment = 0m;
            decimal balance = 0m;
            DateTime currentDate = DateTime.Now;
            
            if(orignalTransactions.Count>0)
            {
                currentDate = orignalTransactions[0].Fecha;
                
                foreach(CustomerTransaction t in orignalTransactions)
                {
                    if(t.Fecha== currentDate)
                    {
                        debt += t.Deuda;
                        payment += t.Abono;
                    }
                    else
                    {
                        balance += debt - payment;
                        CustomerTransaction newT = new CustomerTransaction(0, currentDate, null, debt, payment)
                        {
                            Saldo = balance
                        };

                        result.Add(newT);
                        
                        currentDate = t.Fecha;
                        debt = t.Deuda;
                        payment = t.Abono;
                    }
                }

                balance += debt - payment;
                CustomerTransaction transaction = new CustomerTransaction(0, currentDate, null, debt, payment)
                {
                    Saldo = balance
                };

                result.Add(transaction);


            }

            return result;
        }

    }
}
