using System;

namespace ManagerServer.Model
{
    public interface IForeignCurrencyTransaction
    {
        public DateTime Date { get; }
        public Guid? Currency { get; }
        public decimal ExchangeRate { get; set; }
        public bool ExchangeRateIsInverse { get; set; }
    }
}