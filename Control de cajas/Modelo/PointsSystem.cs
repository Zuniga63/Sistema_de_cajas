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
        private static double maxIncentPercentage1;
        private static double maxIncentPercentage2;
        private static double minIncentPercentage;
        private static double maxPenaltyPercentage1;
        private static double maxPenaltyPercentage2;
        private static double minPenaltyPercengate1;
        private static double minPenaltyPercentage2;

        private static PeriodoDeCobro periodo;
        private static double daysInPeriod;                  //Numero de dias que se hacen efectivos en el periodo
        private static double a1, a2, a4, a6, a7;           //Son las pendientes de las graficas, a1 y a2 incentivos y a3 y a4 penalizaciones
        private static double b1, b2, b4, b6, b7;           //Son los interceptos de las graficas
        private static double c3, c5, c8;               //Son los valores de las lineas

        public static DateTime? CurrentDate { get; private set; }
        public static decimal? Debt { get; private set; }
        public static decimal? Payment { get; private set; }
        public static decimal Balance { get; private set; }
        public static DateTime? CutOfDate { get; private set; }

        public static DateTime? LastDebtDate { get; private set; }
        public static decimal? LastDebt { get; private set; }
        public static double? DebtIncrement { get; private set; }
        public static decimal AmountDebt { get; private set; }

        public static DateTime? LastPaymentDate { get; private set; }
        public static decimal? LastPayment { get; private set; }
        public static double? PaymentPercentage { get; private set; }
        public static decimal AmountPayment { get; private set; }

        public static double LastPartialPoints { get; private set; }
        public static double LastPoints { get; private set; }
        public static double? AnticipateDays { get; private set; }
        public static double? ExpiredDays { get; private set; }
        
        private static void ReloadClass()
        {
            CurrentDate = null;
            Debt = null;
            Payment = null;
            Balance = 0;
            CutOfDate = null;
            LastDebtDate = null;
            LastDebt = null;
            DebtIncrement = null;
            AmountDebt = 0m;
            LastPaymentDate = null;
            LastPayment = null;
            PaymentPercentage = null;
            AmountPayment = 0m;
            LastPartialPoints = 0;
            LastPoints = 0;
            AnticipateDays = null;
            ExpiredDays = null;
        }


        public static void EstablecerParametros(double maxIncent2, double maxIncent1, double minIncent,
            double minPenalty1, double minPenalty2,  double maxPenalty1, double maxPenalty2,  PeriodoDeCobro per)
        {
            maxIncentPercentage1 = maxIncent1/1000;
            maxIncentPercentage2 = maxIncent2 / 1000;
            minIncentPercentage = minIncent/1000;

            minPenaltyPercengate1 = minPenalty1 / 1000;
            minPenaltyPercentage2 = minPenalty2 / 1000;
            maxPenaltyPercentage1 = maxPenalty1/1000;
            maxPenaltyPercentage2 = maxPenalty2/1000;
            periodo = per;
            daysInPeriod = (double)periodo;
            
            EstablecerModelos();
        }

        /// <summary>
        /// Establece los modelos matematicos lineales
        /// </summary>
        private static void EstablecerModelos()
        {
            //Se establecen los parametros del modelo que incentiva los pagos antes de medio periodo;
            a1 = (maxIncentPercentage1 - maxIncentPercentage2) / (daysInPeriod / 2.0);
            b1 = maxIncentPercentage2;

            //Se establecen los parametros del modelo que incentiva los pagos durante la segunda mirad del
            //periodo de pago
            a2 = (minIncentPercentage - maxIncentPercentage1) / (daysInPeriod / 2.0);
            b2 = maxIncentPercentage1 - a2 * (daysInPeriod / 2.0);

            //Se estable el valor constante de la bonificacion por pago por hasta dos peridoso
            c3 = minIncentPercentage;

            //Se establecen los parametros del modelo que penaliza por mora durante la primera mitad
            //del periodo de mora
            a4 = (minPenaltyPercentage2 - minPenaltyPercengate1) / (daysInPeriod / 2.0);
            b4 = minPenaltyPercengate1 - a4 * daysInPeriod;

            //Se establecen los paraemtros del modelo que penaliza por mora durante la segunda mitad
            //del periodo
            c5 = minPenaltyPercentage2;

            //Se establen los parametros del modelo que penaliza por mora cuando supera un periodo de mora
            a6 = (maxPenaltyPercentage1 - minPenaltyPercentage2) / (daysInPeriod / 2.0);
            b6 = minPenaltyPercentage2 - a6 * daysInPeriod * 2.0;

            //Se establecen los parametros del modelo que penzaliza por mora durante la segunda mitad
            //del segundo periodo por mora
            a7 = (maxPenaltyPercentage2 - maxPenaltyPercentage1) / (daysInPeriod / 2.0);
            b7 = maxPenaltyPercentage1 - a7 * (2.5) * daysInPeriod;

            //Se estable el valor de la constante de la maxima penalizacion por mora del cliente
            c8 = maxPenaltyPercentage2;
        }

        /// <summary>
        /// Calcula el factor utilizado pra calcular los incentivos segun los dias anticipados
        /// o los días de mora
        /// </summary>
        /// <param name="anticipateDay"></param>
        /// <returns></returns>
        private static double IncentivePerPayment(double? anticipateDay, double? expiredDays)
        {
            double currentDay = 0d;

            //Primero se define cual es el dia en el que se encuentra dentro de las graficas
            //si es antes o despues de la fecha de corte
            if(anticipateDay.HasValue)
            {
                if ((daysInPeriod - anticipateDay.Value) < 0)
                {
                    currentDay = 0;
                }
                else
                {
                    currentDay = daysInPeriod - anticipateDay.Value;
                }
            }
            else
            {
                currentDay = daysInPeriod + expiredDays.Value;
            }

            //Ahora si se define el factor de bonificacion segun el día de pago
            if (currentDay <= daysInPeriod / 2.0)
            {
                return a1 * currentDay + b1;
            }//Fin de if
            else if (currentDay <= daysInPeriod)
            {
                return a2 * currentDay + b2;
            }
            else if (currentDay <= daysInPeriod * 3)
            {
                return c3;
            }
            else
            {
                return 0d;
            }

        }//Fin del metodo

        /// <summary>
        /// Calcula el factor utilizado para calcular las penalizaciones por nueveas deudas
        /// </summary>
        /// <param name="anticipateDay"></param>
        /// <param name="expiredDay"></param>
        /// <returns></returns>
        private static double PenaltyPerNewDebt(double? anticipateDay)
        {
            if(anticipateDay.HasValue)
            {
                double currentDay = (double)periodo - anticipateDay.Value < 0 ? 0 : (double)periodo - anticipateDay.Value;
                return a2 * currentDay + b2;
            }
            else
            {
                return a2 * daysInPeriod + b2;
            }
        }

        /// <summary>
        /// Calcula el factor utilizado para calcular la penalizacion por mora en el pago
        /// teniendo en cuenta los días de mora
        /// </summary>
        /// <param name="expiredDays"></param>
        /// <returns></returns>
        private static double PenaltyPerDefault(double expiredDays)
        {
            double currentDay = daysInPeriod + expiredDays;

            if (currentDay > daysInPeriod && currentDay <= 1.5 * daysInPeriod)
            {
                return a4 * currentDay + b4;
            }
            else if (currentDay <= 2 * daysInPeriod)
            {
                return c5;
            }
            else if (currentDay <= 2.5 * daysInPeriod)
            {
                return a6 * currentDay + b6;
            }
            else if (currentDay <= 3.0 * daysInPeriod)
            {
                return a7 * currentDay + b7;
            }
            else
            {
                return c8;
            }
        }

        /// <summary>
        /// Retorna la fecha de corte teniendo en cuenta el periodo de cobro
        /// </summary>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        private static DateTime DefineCutOfDate(DateTime currentDate)
        {
            int year = currentDate.Year;
            int month = currentDate.Month;
            int dayInMonth = DateTime.DaysInMonth(year, month);

            switch(periodo)
            {
                case PeriodoDeCobro.Semanal:
                    return currentDate.AddDays(7);
                case PeriodoDeCobro.Quincenal:
                    return currentDate.AddDays(15);
                case PeriodoDeCobro.Mensual:
                    return currentDate.AddDays(dayInMonth);
                default:
                    return DateTime.Now;
            }
        }

        public static List<CustomerTracking> DefineScore(List<CustomerTransaction> transactions)
        {
            List<CustomerTracking> tracking = new List<CustomerTracking>();
            List<CustomerTransaction> normalizeTransactions = NormalizeTransactions(transactions);
            DateTime currentDate;
            decimal? debt;
            decimal? payment;
            decimal balance = 0m;
            DateTime? cutOfDate = null;
            double? anticipateDay;
            double? expiredDay;
            double partialPoints = 0;
            double points = 0;

            foreach(CustomerTransaction t in normalizeTransactions)
            {
                //Para cada transaccion se reinician los valores de deuda y pago
                currentDate = t.Fecha;
                debt = null;
                payment = null;
                anticipateDay = null;
                expiredDay = null;
                partialPoints = 0d;

                //Todas los clientes inician con saldo igual a cero por lo tanto la primera transaccion debe ser
                //una deuda y por lo tanto define la fecha de corte
                if (balance <= 0)
                {
                    if (t.Deuda > 0) 
                    {
                        debt = t.Deuda;
                        payment = t.Abono;
                        balance += t.Deuda - t.Abono;
                        
                        if(balance>0)
                        {
                            cutOfDate = DefineCutOfDate(currentDate);
                        }

                        CustomerTracking cT = new CustomerTracking(currentDate, debt, payment, balance, cutOfDate, 
                            anticipateDay, expiredDay, partialPoints, points);

                        tracking.Add(cT);
                    }//fin de if
                    else
                    {
                        //throw new NotImplementedException("Error en las fechas de las transacciones");
                        break;
                    }
                }//Fin de if
                else
                {
                    //Primero se determina si es un movimiento anticipado o expirado
                    if(currentDate <= cutOfDate.Value)
                    {
                        anticipateDay = cutOfDate.Value.Subtract(currentDate).TotalDays;
                    }//Fin de if
                    else
                    {
                        expiredDay = currentDate.Subtract(cutOfDate.Value).TotalDays;
                    }

                    //Si la transaccion es una deuda entonces ya no se realiza penalizacion
                    //Si es un abono, entonces realiza las bonificaciones correspondientes
                    if(t.Abono>0)
                    {
                        //Defino el valor del pago
                        payment = t.Abono;

                        //La fecha está vencida entonces existe una bonificacion minima y se penaliza segun la deuda
                        if (expiredDay.HasValue)
                        {
                            double penalty = PenaltyPerDefault(expiredDay.Value);
                            double penatyValue = penalty * ((double)balance);

                            double incentive = IncentivePerPayment(anticipateDay, expiredDay);
                            double incentiveValue = incentive * (double)payment.Value;

                            partialPoints = penatyValue + incentiveValue;
                            points += partialPoints;
                        }
                        else//En este caso se realiza solo la bonificacion
                        {
                            double incentive = IncentivePerPayment(anticipateDay.Value, expiredDay);
                            partialPoints = (double)payment.Value * incentive;
                            points += partialPoints;
                        }

                        //Ahora se actualiza la fecha de corte
                        balance -= t.Abono;
                        if (balance > 0)
                        {
                            cutOfDate = DefineCutOfDate(currentDate);
                        }
                        else
                        {
                            cutOfDate = null;
                        }
                    }

                    if(t.Deuda>0)
                    {
                        debt = t.Deuda;
                        balance += t.Deuda;
                    }

                    CustomerTracking cT = new CustomerTracking(currentDate, debt, payment, balance, cutOfDate, anticipateDay, expiredDay, partialPoints, points);
                    tracking.Add(cT);
                    
                }//Fin de else
            }//Fin de foreach

            //Codigo temporal para determinar la valoracion del cliente si pagara en el momento actual
            if(balance>0)
            {
                currentDate = DateTime.Now;
                decimal nuevoPago = balance;

                //Primero se determina si es un movimiento anticipado o expirado
                if (currentDate <= cutOfDate.Value)
                {
                    anticipateDay = cutOfDate.Value.Subtract(currentDate).TotalDays;
                }//Fin de if
                else
                {
                    expiredDay = currentDate.Subtract(cutOfDate.Value).TotalDays;
                }
            }

            LastPoints = points;
            return tracking;
        }

        public static List<CustomerTracking> DefineScore2(List<CustomerTransaction> transactions)
        {
            List<CustomerTracking> tracking = new List<CustomerTracking>();
            List<CustomerTransaction> normalizeTransactions = NormalizeTransactions(transactions);
            ReloadClass();

            foreach(CustomerTransaction t in normalizeTransactions)
            {
                CurrentDate = t.Fecha;
                Debt = null;
                Payment = null;
                AnticipateDays = null;
                ExpiredDays = null;
                LastPartialPoints = 0;
                double? incentivePercentage = null;
                double? penaltyPercentage = null;

                //Si el balance es cero, lo primero que se debe garantizar es que se tenga una fecha de corte valida 
                //para la siguiente transaccion. En caso de que se tenga nuevamente un balance igual a cero se repetirá 
                //la condicion con cada transaccion hasta que esta lo supere
                if (Balance <= 0)
                {
                    if (t.Deuda > 0)
                    {
                        Debt = t.Deuda;
                        LastDebtDate = CurrentDate;
                        LastDebt = Debt;
                        DebtIncrement = 0;              //Porque el saldo anterior es cero
                        AmountDebt += Debt.Value;

                        Balance += Debt.Value;
                        CutOfDate = DefineCutOfDate(CurrentDate.Value);
                    }//Fin de if

                    if(t.Abono>0)
                    {
                        Payment = t.Abono;
                        LastPaymentDate = CurrentDate;
                        LastPayment = Payment;
                        //SI el balance es cero y hay un pago esto es sinonimo de que existe un error
                        //en el ingreso de la informacion en la base de datos
                        PaymentPercentage = (Balance > 0) ? (double)Payment / (double) Balance : 0;
                        AmountPayment += Payment.Value;

                        Balance -= Payment.Value;

                        if(Balance<=0)
                        {
                            CutOfDate = null;
                        }
                    }//Fin de if
                }//Fin de if
                else
                {
                    //Si se ha entrado a este punto, la fecha de corte debe esta correctamente definida por lo que
                    //no debe haber riesgo de que lance un exeption y se puede definri si es un movimiento anticipado o en mora
                    if(CurrentDate<=CutOfDate)
                    {
                        AnticipateDays = CutOfDate.Value.Subtract(CurrentDate.Value).TotalDays;
                    }
                    else
                    {
                        ExpiredDays = CurrentDate.Value.Subtract(CutOfDate.Value).TotalDays;
                    }

                    //Como puede haber penalizacion por mora, y esta se basa en el saldo hasta la fecha lo 
                    //primero que se verifica es si hay un pago y reliza los incentivos o penalizaciones antes de modificar el 
                    //balance
                    if(t.Abono>0)
                    {
                        Payment = t.Abono;
                        LastPaymentDate = CurrentDate;
                        LastPayment = Payment;
                        PaymentPercentage = (Balance > 0) ? (double)Payment / (double)Balance : 0;
                        AmountPayment += Payment.Value;

                        //Ahora se hacen efectivas las penalizaciones o los incentios
                        if(ExpiredDays.HasValue)
                        {
                            double penalty = PenaltyPerDefault(ExpiredDays.Value);
                            penaltyPercentage = penalty * 1000;
                            double penatyValue = penalty * ((double)Balance);

                            double incentive = IncentivePerPayment(AnticipateDays, ExpiredDays);
                            incentivePercentage = incentive * 1000;
                            double incentiveValue = incentive * (double)Payment.Value;

                            LastPartialPoints = penatyValue + incentiveValue;
                            LastPoints += LastPartialPoints;
                        }
                        else
                        {
                            double incentive = IncentivePerPayment(AnticipateDays, ExpiredDays);
                            incentivePercentage = incentive * 1000;
                            double incentiveValue = incentive * (double)Payment.Value;

                            LastPartialPoints = incentiveValue;
                            LastPoints += LastPartialPoints;
                        }

                        Balance -= Payment.Value;

                        if (Balance > 0)
                        {
                            CutOfDate = DefineCutOfDate(CurrentDate.Value);
                        }
                        else
                        {
                            CutOfDate = null;
                        }
                    }//Fin de if

                    if (t.Deuda > 0)
                    {
                        Debt = t.Deuda;
                        LastDebtDate = CurrentDate;
                        LastDebt = Debt;
                        DebtIncrement = (Balance > 0) ? (double)(Debt.Value / Balance) : 0d;
                        AmountDebt += Debt.Value;

                        //Antes de actualizar el valor del balance se debe verificar si ya estaba en cero porque
                        //esto signfica que la fecha de corte tiene valor nulo, lo cual significa que se canceló la deuda
                        // y se adquirio una nueva
                        if (Balance <= 0)
                        {
                            CutOfDate = DefineCutOfDate(CurrentDate.Value);
                        }

                        Balance += Debt.Value;
                    }
                }

                CustomerTracking track = new CustomerTracking(CurrentDate.Value, Debt, Payment, Balance, CutOfDate,
                    AnticipateDays, incentivePercentage, ExpiredDays, penaltyPercentage, LastPartialPoints, LastPoints);
                tracking.Add(track);
            }//Fin de foreach

            //Codigo temporal para determinar la valoracion del cliente si pagara en el momento actual
            //*****************************************************************************************************************
            //*****************************************************************************************************************
            if (Balance > 0)
            {
                DateTime currentDate = DateTime.Now;
                double? anticipateDays = null;
                double? expiredDays = null;
                double partialPoints = 0;

                //Primero se determina si es un movimiento anticipado o expirado
                if (currentDate <= CutOfDate.Value)
                {
                    anticipateDays = CutOfDate.Value.Subtract(currentDate).TotalDays;
                }//Fin de if
                else
                {
                    expiredDays = currentDate.Subtract(CutOfDate.Value).TotalDays;
                }

                if (expiredDays.HasValue)
                {
                    double penalty = PenaltyPerDefault(expiredDays.Value);
                    double penatyValue = penalty * ((double)Balance);

                    double incentive = IncentivePerPayment(anticipateDays, expiredDays);
                    double incentiveValue = incentive * (double)Balance;

                    partialPoints = penatyValue + incentiveValue;
                    LastPoints += partialPoints;
                }
                else
                {
                    double incentive = IncentivePerPayment(anticipateDays, expiredDays);
                    double incentiveValue = incentive * (double)Balance;

                    partialPoints = incentiveValue;
                    LastPoints += partialPoints;
                }
            }
            //*****************************************************************************************************************
            //*****************************************************************************************************************

            return tracking;
        }//Fin del metodo

        /// <summary>
        /// Este metodo se encarga de normalizar los movimientos con el onjetivo que el sistema de puntuacion pueda 
        /// establecerse correctamente
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
