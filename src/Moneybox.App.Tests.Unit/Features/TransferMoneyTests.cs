using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using Xunit;

namespace Moneybox.App.Tests.Unit.Features
{
    public class TransferMoneyTests
    {
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<INotificationService> notificationServiceMock;
        private readonly TransferMoney transferMoneyFeature;

        public TransferMoneyTests()
        {
            this.accountRepositoryMock = new Mock<IAccountRepository>();
            this.notificationServiceMock = new Mock<INotificationService>();

            transferMoneyFeature = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);
        }

        [Fact]
        public void Execute_BalanceAfterWithdrawLowerThan500_NotifyLowerFunds()
        {
            // Arrange
            var fromAccountGuid = Guid.NewGuid();
            var toAccountGuid = Guid.NewGuid();
            var email = "email@gmail.com";
            
            var fromAccount = new Account
            {
                Balance = 1000m,
                Id = fromAccountGuid,
                User = new User
                {
                    Email = email
                },
                Withdrawn = 50m,
                PaidIn = 50m
            };
            
            var toAccount = new Account
            {
                Balance = 1500m,
                Id = toAccountGuid,
                User = new User(),
                Withdrawn = 50m,
                PaidIn = 50m
            };
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccountGuid))
                .Returns(fromAccount);
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccountGuid))
                .Returns(toAccount);
            
            // Act
            transferMoneyFeature.Execute(fromAccountGuid, toAccountGuid, 600m);
            
            // Assert
            notificationServiceMock.Verify(x=>x.NotifyFundsLow(email),Times.Once);
        }
        
        [Fact]
        public void Execute_PayInAfterDepositCloseToLimit_NotifyApproachingPayInLimit()
        {
            // Arrange
            var fromAccountGuid = Guid.NewGuid();
            var toAccountGuid = Guid.NewGuid();
            var email = "email@gmail.com";
            
            var fromAccount = new Account
            {
                Balance = 10000m,
                Id = fromAccountGuid,
                User = new User(),
                Withdrawn = 50m,
                PaidIn = 50m
            };
            
            var toAccount = new Account
            {
                Balance = 1500m,
                Id = toAccountGuid,
                User = new User
                {
                    Email = email
                },
                Withdrawn = 50m,
                PaidIn = 0m
            };
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccountGuid))
                .Returns(fromAccount);
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccountGuid))
                .Returns(toAccount);
            
            // Act
            transferMoneyFeature.Execute(fromAccountGuid, toAccountGuid, Account.PayInLimit - 100);
            
            // Assert
            notificationServiceMock.Verify(x=>x.NotifyApproachingPayInLimit(email),Times.Once);
        }
        
        [Fact]
        public void Execute_ExecuteWithoutErrors_UpdateAccounts()
        {
            // Arrange
            var fromAccountGuid = Guid.NewGuid();
            var toAccountGuid = Guid.NewGuid();
            
            var fromAccount = new Account
            {
                Balance = 10000m,
                Id = fromAccountGuid,
                User = new User(),
                Withdrawn = 50m,
                PaidIn = 50m
            };
            
            var toAccount = new Account
            {
                Balance = 1500m,
                Id = toAccountGuid,
                User = new User(),
                Withdrawn = 50m,
                PaidIn = 0m
            };
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccountGuid))
                .Returns(fromAccount);
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccountGuid))
                .Returns(toAccount);
            
            // Act
            transferMoneyFeature.Execute(fromAccountGuid, toAccountGuid, 500m);
            
            // Assert
            accountRepositoryMock.Verify(x => 
                x.Update(It.Is<Account>(acc => acc.Id == fromAccountGuid)), Times.Once);
            
            accountRepositoryMock.Verify(x => 
                x.Update(It.Is<Account>(acc => acc.Id == toAccountGuid)), Times.Once);
        }
    }
}