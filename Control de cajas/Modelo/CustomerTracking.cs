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

        private DateTime? _cutOfDate;
        public DateTime? CutOfDate => _cutOfDate;

        private double? _anticipateDays;
        public double? AnticipateDays => _anticipateDays;

        private double? _incentivePercentage;
        public double? IncentivePercentage => _incentivePercentage;

        private double? _expiredDays;
        public double? ExpiredDays => _expiredDays;

        private double? _penaltyPercentage;
        public double? PenaltyPercentage => _penaltyPercentage;

        private double _partialPoints;
        public double PartialPoints => _partialPoints;

        private double _points;
        public double Points => _points;

        public CustomerTracking(DateTime transactionDate, decimal? debt, decimal? payment, decimal balance, 
            DateTime? cutOfDate, double? anticipateDays, double? expiredDays, double partialPoints, double points)
        {
            _transactionDate = transactionDate;
            _debt = debt;
            _payment = payment;
            _balance = balance;
            _cutOfDate = cutOfDate;
            _anticipateDays = anticipateDays;
            _expiredDays = expiredDays;
            _partialPoints = partialPoints;
            _points = points;
        }

        public CustomerTracking(DateTime transactionDate, decimal? debt, decimal? payment, decimal balance,
            DateTime? cutOfDate, double? anticipateDays, double? incentivePercentage, double? expiredDays, 
            double? penaltyPercentage, double partialPoints, double points)
        {
            _transactionDate = transactionDate;
            _debt = debt;
            _payment = payment;
            _balance = balance;
            _cutOfDate = cutOfDate;
            _anticipateDays = anticipateDays;
            _incentivePercentage = incentivePercentage;
            _expiredDays = expiredDays;
            _penaltyPercentage = penaltyPercentage;
            _partialPoints = partialPoints;
            _points = points;
        }

    }
}
