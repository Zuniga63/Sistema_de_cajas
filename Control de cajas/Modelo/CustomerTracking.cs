using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_de_cajas.Modelo
{
    class CustomerTracking
    {
        private DateTime _transactionDate;
        public DateTime TransactionDate => _transactionDate;

        private decimal? _debt;
        public decimal? Debt => _debt;

        private decimal? _payment;
        public decimal? Payment => _payment;

        private decimal _balance;
        public decimal Balance => _balance;

        private double _daysOfLastTransaction;
        public double DaysOfLastTransaction => _daysOfLastTransaction;

        private decimal _realDebt;
        public decimal RealDebt => _realDebt;

        private decimal _grossProfit; //Beneficio bruto
        public decimal GrossProfit => _grossProfit;

        private decimal _netProfit; //Beneficio neto
        public decimal NetProfit => _netProfit;

        private decimal _financialCost; //Costo financiero
        public decimal FinancialCost => _financialCost;

        private double _points;
        public double Points => _points;

        private decimal _creditLimit;
        public decimal CreditLimit => _creditLimit;

        public CustomerTracking(DateTime transactionDate, decimal? debt, decimal? payment, decimal balance, double dayOfLastTransaction,
            decimal realDebt, decimal grossProfit, decimal netProfit, decimal financialCost, double points, decimal creditLimit)
        {
            _transactionDate = transactionDate;
            _debt = debt;
            _payment = payment;
            _balance = balance;
            _daysOfLastTransaction = dayOfLastTransaction;
            _realDebt = realDebt;
            _grossProfit = grossProfit;
            _netProfit = netProfit;
            _financialCost = financialCost;
            _points = points;
            _creditLimit = creditLimit;
        }

        

    }
}
