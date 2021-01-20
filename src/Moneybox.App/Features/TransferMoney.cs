using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository accountRepository;
        private readonly INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
            var toAccount = this.accountRepository.GetAccountById(toAccountId);

            fromAccount.WithdrawFund(amount);
            toAccount.DepositFund(amount);

            if (fromAccount.Balance < 500m)
            {
                this.notificationService.NotifyFundsLow(fromAccount.User.Email);
            }
            
            if (Account.PayInLimit - toAccount.PaidIn < 500m)
            {
                this.notificationService.NotifyApproachingPayInLimit(toAccount.User.Email);
            }
            
            this.accountRepository.Update(fromAccount);
            this.accountRepository.Update(toAccount);
        }
    }
}
