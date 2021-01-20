using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository accountRepository;
        private readonly INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
            fromAccount.WithdrawFund(amount);
            
            if (fromAccount.Balance < 500m)
            {
                this.notificationService.NotifyFundsLow(fromAccount.User.Email);
            }
            
            this.accountRepository.Update(fromAccount);
        }
    }
}
