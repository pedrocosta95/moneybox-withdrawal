using System;
using Xunit;

namespace Moneybox.App.Tests.Unit.Domain
{
    public class AccountTests
    {
        [Fact]
        public void WithdrawFund_AmountBiggerThanFunds_ThrowsInvalidOperationException()
        {
            // Arrange
            var balance = 500m;
            var withdrawn = 50m;
            
            var account = new Account
            {
                Balance = balance,
                Id = Guid.NewGuid(),
                User = new User(),
                Withdrawn = withdrawn,
                PaidIn = 0m
            };

            // Act
            Assert.Throws<InvalidOperationException>(()=> account.WithdrawFund(balance + 50m));
        }
        
        [Fact]
        public void WithdrawFund_AmountLowerThanFunds_FundsWithdrawn()
        {
            // Arrange
            var balance = 500m;
            var amount = 50m;
            var withdrawn = 50m;
            
            var account = new Account
            {
                Balance = balance,
                Id = Guid.NewGuid(),
                User = new User(),
                Withdrawn = withdrawn,
                PaidIn = 0m
            };

            // Act
            account.WithdrawFund(amount);

            // Assert
            Assert.Equal(balance - amount, account.Balance);
            Assert.Equal(withdrawn + amount, account.Withdrawn);
        }
        
        [Fact]
        public void WithdrawFund_AmountEqualsToFunds_FundsWithdrawn()
        {
            // Arrange
            var balance = 500m;
            var withdrawn = 50m;
            
            var account = new Account
            {
                Balance = balance,
                Id = Guid.NewGuid(),
                User = new User(),
                Withdrawn = withdrawn,
                PaidIn = 0m
            };

            // Act
            account.WithdrawFund(balance);

            // Assert
            Assert.Equal(balance - balance, account.Balance);
            Assert.Equal(withdrawn + balance, account.Withdrawn);
        }
        
        [Fact]
        public void DepositFund_AmountSurpassesPayInLimit_ThrowsInvalidOperationException()
        {
            // Arrange
            var account = new Account
            {
                Balance = 500m,
                Id = Guid.NewGuid(),
                User = new User(),
                Withdrawn = 50m,
                PaidIn = 0m
            };

            // Act
            Assert.Throws<InvalidOperationException>(() => account.DepositFund(Account.PayInLimit + 50));
        }
        
        [Fact]
        public void DepositFund_AmountDoesNotSurpassesPayInLimit_FundsDeposited()
        {
            // Arrange
            var balance = 500m;
            var amount = 50m;
            var paidIn = 50m;
            
            var account = new Account
            {
                Balance = balance,
                Id = Guid.NewGuid(),
                User = new User(),
                Withdrawn = 50m,
                PaidIn = paidIn
            };

            // Act
            account.DepositFund(amount);

            // Assert
            Assert.Equal(balance + amount, account.Balance);
            Assert.Equal(paidIn + amount, account.PaidIn);
        }
        
        [Fact]
        public void DepositFund_AmountEqualsToPayInLimit_FundsDeposited()
        {
            // Arrange
            var balance = 500m;
            var paidIn = 0m;
            
            var account = new Account
            {
                Balance = balance,
                Id = Guid.NewGuid(),
                User = new User(),
                Withdrawn = 50m,
                PaidIn = paidIn
            };

            // Act
            account.DepositFund(Account.PayInLimit);

            // Assert
            Assert.Equal(balance + Account.PayInLimit, account.Balance);
            Assert.Equal(paidIn + Account.PayInLimit, account.PaidIn);
        }
    }
}