using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }
        
        public void WithdrawFund(decimal amount)
        {
            if (this.Balance - amount < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }
            
            this.Balance -= amount;
            
            // I inverted this operation assuming the property "Withdrawn" keeps a record of all the withdrawn money
            this.Withdrawn += amount;
        }
        
        public void DepositFund(decimal amount)
        {
            if (this.PaidIn + amount > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
            
            this.Balance += amount;
            this.PaidIn += amount;
        }
    }
}
