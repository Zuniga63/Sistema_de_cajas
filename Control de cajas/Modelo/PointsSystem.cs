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
        public static decimal? LastPayment { get; private set; }
        public static double? PaymentPercentage { get; private set; }
        public static decimal? AveragePayment { get; private set; }

        public static DateTime? DateOfLastDebt { get; private set; }
        public static decimal? LastDebt { get; private set; }

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
            LastPayment = null;
            PaymentPercentage = null;
            AveragePayment = null;

            DateOfLastDebt = null;
            LastDebt = null;

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

                    DateOfLastDebt = CurrentDate;
                    LastDebt = Balance;

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
                    Debt = 0m;
                    Payment = 0m;
                    
                    
                    if(t.Deuda>0)
                    {
                        Debt = t.Deuda;

                        RealDebt += t.Deuda / (decimal)(1 + _utilidad);
                        creditLimitBasic -= t.Deuda;
                        LastDebt = Debt;
                        DateOfLastDebt = CurrentDate;
                    }

                    if(t.Abono>0)
                    {
                        Payment = t.Abono;

                        if (RealDebt > t.Abono)
                        {
                            RealDebt -= t.Abono;
                        }
                        else
                        {
                            GrossProfit += t.Abono - RealDebt;
                            RealDebt = 0m;
                        }
                        DateOfLastPayment = CurrentDate;
                        LastPayment = t.Abono;
                        PaymentPercentage = Balance>0 ? (double)(Payment / Balance) : 0d;
                        creditLimitBasic += t.Abono;
                    }

                    Balance += Debt - Payment;

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

            //Se actualiza los días de este periodo que se calculan desde la ultima transaccion o desde el ultimo pago del cliente
            if(DateOfLastPayment.HasValue && Balance>0)
            {
                DaysOfThisPeriod = DateTime.Now.Subtract(DateOfLastPayment.Value).TotalDays;
            }
            else if(CurrentDate.HasValue && Balance>0)
            {
                DaysOfThisPeriod = DateTime.Now.Subtract(CurrentDate.Value).TotalDays;
            }
            else
            {
                DaysOfThisPeriod = 0d;
            }

            //Ahora se ajusta la puntuacion del cliente como si realizará el pago el día de hoy, solo se hace para clientes con
            //balance superior a cero
            if(Balance>0)
            {
                double days = DateTime.Now.Subtract(CurrentDate.Value).TotalDays;
                Points += (double)(Balance - RealDebt * (decimal)(1 + (Math.Pow(1 + _interesPeriodico, days) - 1)))/1000d;
            }

            CalculateAveragePayment(normalizeTransactions);

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

        private static void CalculateAveragePayment(List<CustomerTransaction> transactions)
        {
            DateTime? startDate = null;
            double totalDays = 0;
            decimal totalPayment = 0m;
            bool cuentaSaldada = true;
            

            if(transactions.Count>0)
            {
                startDate = transactions[0].Fecha;
                cuentaSaldada = false;

                foreach(CustomerTransaction t in transactions)
                {
                    if(t.Abono>0)
                    {
                        totalPayment += t.Abono;
                    }

                    if(t.Saldo == 0 && !cuentaSaldada)
                    {
                        totalDays += t.Fecha.Subtract(startDate.Value).TotalDays;
                        cuentaSaldada = true;
                    }

                    if(t.Saldo != 0 && cuentaSaldada)
                    {
                        startDate = t.Fecha;
                        cuentaSaldada = false;
                    }
                }

                //Finalmente si la cuenta no está saldada entonces se calculan los días hasta la fecha actual
                if(!cuentaSaldada)
                {
                    totalDays += DateTime.Now.Subtract(startDate.Value).TotalDays;
                }


                AveragePayment = totalDays > 0 ? totalPayment / (decimal)(totalDays / 30d) : (decimal?)null;

            }
        }

    }
}
