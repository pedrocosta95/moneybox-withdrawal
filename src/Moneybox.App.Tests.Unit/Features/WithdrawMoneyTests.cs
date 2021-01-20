using System;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using Xunit;

namespace Moneybox.App.Tests.Unit.Features
{
    public class WithdrawMoneyTests
    {
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<INotificationService> notificationServiceMock;
        private readonly WithdrawMoney transferMoneyFeature;

        public WithdrawMoneyTests()
        {
            this.accountRepositoryMock = new Mock<IAccountRepository>();
            this.notificationServiceMock = new Mock<INotificationService>();

            transferMoneyFeature = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);
        }

        [Fact]
        public void Execute_BalanceAfterWithdrawLowerThan500_NotifyLowerFunds()
        {
            // Arrange
            var fromAccountGuid = Guid.NewGuid();
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
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccountGuid))
                .Returns(fromAccount);
            
            // Act
            transferMoneyFeature.Execute(fromAccountGuid, 600m);
            
            // Assert
            notificationServiceMock.Verify(x=>x.NotifyFundsLow(email),Times.Once);
        }
        
        [Fact]
        public void Execute_ExecuteWithoutErrors_UpdateAccount()
        {
            // Arrange
            var fromAccountGuid = Guid.NewGuid();
            
            var fromAccount = new Account
            {
                Balance = 10000m,
                Id = fromAccountGuid,
                User = new User(),
                Withdrawn = 50m,
                PaidIn = 50m
            };
            
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccountGuid))
                .Returns(fromAccount);
            
            // Act
            transferMoneyFeature.Execute(fromAccountGuid, 500m);
            
            // Assert
            accountRepositoryMock.Verify(x => 
                x.Update(It.Is<Account>(acc => acc.Id == fromAccountGuid)), Times.Once);
        }
    }
}