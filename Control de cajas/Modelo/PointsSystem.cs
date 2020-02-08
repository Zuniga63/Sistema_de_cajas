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

        public static DateTime? DateOfLastPayment { get; private set; }
        public static double? DaysOfThisPeriod { get; private set; }

        public static decimal RealDebt { get; private set; }
        public static double DaysOfLastTransaction { get; private set; }
        public static decimal FinancialCost { get; private set; }
        public static decimal GrossProfit { get; private set; }
        public static decimal NetProfit { get; private set; }
        public static double Points { get; private set; }
        public static decimal CreditLimit { get; private set; }




        private static void ReloadClass()
        {
            CurrentDate = null;
            Debt = 0m;
            Payment = 0m;
            Balance = 0;

            DateOfLastPayment = null;
            DaysOfThisPeriod = null;

            RealDebt = 0m;
            DaysOfLastTransaction = 0;
            FinancialCost = 0;
            GrossProfit = 0m;
            NetProfit = 0m;
            Points = 0d;
            CreditLimit = 0m;
        }


        public static void EstablecerParametros(double interesAnual, double utilidad)
        {
            _interesAnual = interesAnual;
            _interesPeriodico = Math.Pow((1+interesAnual), (1.0d/365.0d)) - 1;
            _utilidad = utilidad;
        }

        
        public static List<CustomerTracking> DefineScore(List<CustomerTransaction> transactions, Customer customer)
        {
            bool firtsTransaction = true;
            List<CustomerTracking> tracking = new List<CustomerTracking>();
            List<CustomerTransaction> normalizeTransactions = NormalizeTransactions(transactions);
            ReloadClass();
            decimal creditLimitBasic = customer.Creditlimit;

            foreach (CustomerTransaction t in normalizeTransactions)
            {
                //Se créa el primer registro se seguimiento (tracking)
                if (firtsTransaction)
                {
                    firtsTransaction = false;
                    CurrentDate = t.Fecha;
                    Payment = t.Abono;
                    Debt = t.Deuda;
                    Balance = Debt - Payment;

                    RealDebt = (Debt-Payment) / (decimal)((1 + _utilidad));
                    creditLimitBasic -= Balance;

                    //Se crea el registro de seguimiento y se agrega a la lista
                    CustomerTracking track = new CustomerTracking(CurrentDate.Value, Debt, Payment, Balance, DaysOfLastTransaction, RealDebt,
                        GrossProfit, NetProfit, FinancialCost, Points, CreditLimit);
                    tracking.Add(track);
                }
                else
                {
                    //Se calcula el costo financiero teniendo en cunta los días que han pasado desde la ultima transaccion
                    //o conjunto de transacciones hasta la fecha de esta nueva transaccion
                    DaysOfLastTransaction = t.Fecha.Subtract(CurrentDate.Value).TotalDays;
                    FinancialCost += RealDebt * (decimal)(Math.Pow((1 + _interesPeriodico), DaysOfLastTransaction) - 1);

                    //Se actulizan los valores con respecto al seguimiento
                    CurrentDate = t.Fecha;
                    Debt = t.Deuda;
                    Payment = t.Abono;
                    Balance += Debt - Payment;
                    
                    if(t.Deuda>0)
                    {
                        RealDebt += t.Deuda / (decimal)(1 + _utilidad);
                        creditLimitBasic -= t.Deuda;
                    }

                    if(t.Abono>0)
                    {
                        if (RealDebt > t.Abono)
                        {
                            RealDebt -= t.Abono;
                        }
                        else
                        {
                            GrossProfit += t.Abono - RealDebt;
                            RealDebt = 0m;
                        }

                        creditLimitBasic += t.Abono;
                    }

                    NetProfit = GrossProfit - FinancialCost;
                    Points = (double)NetProfit / 1000d;
                    CreditLimit = creditLimitBasic + NetProfit;

                    CustomerTracking track = new CustomerTracking(CurrentDate.Value, Debt, Payment, Balance, DaysOfLastTransaction, RealDebt,
                        GrossProfit, NetProfit, FinancialCost, Points, CreditLimit);
                    tracking.Add(track);
                }//Fin de else
            }//Fin de foreach

            //Ahora se calcula el cupo del cliente
            customer.Creditlimit = CreditLimit;

            //Finalmente se actualiza los días de este periodo
            if(DateOfLastPayment.HasValue)
            {
                DaysOfThisPeriod = DateTime.Now.Subtract(DateOfLastPayment.Value).TotalDays;
            }
            else if(CurrentDate.HasValue)
            {
                DaysOfThisPeriod = DateTime.Now.Subtract(CurrentDate.Value).TotalDays;
            }
            else
            {
                DaysOfThisPeriod = null;
            }

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
