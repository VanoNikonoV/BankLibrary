using Bank.Interfases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bank.Models
{
    /// <summary>
    /// 
    /// Счет для начисления процентов
    /// </summary>
    public class DepositAccount : Account, ICovAccount<Account>, IContrAccount<Account>
    {
        
        public DepositAccount(decimal initialBalance) : base(initialBalance) 
        {
           
        }
        /// <summary>
        /// Пополнение счета
        /// </summary>
        /// <param name="amount">Сумма перевода</param>
        /// <returns>Счет с измененым балансом</returns>
        public Account MakeDeposit(decimal amount)
        {
            this.Balance += amount;

            return this;
        }
        /// <summary>
        /// Перевод средств на другой счет
        /// </summary>
        /// <param name="client">Получатель средств</param>
        /// <param name="amount">Сумма перевода</param>
        public void MakeWithdrawal(Account client, decimal amount)
        {
            if (client.Balance >= amount)
            {
                client.Balance -= amount;
            }
            else MessageBox.Show("Недостаточно средств");
        }

        public override void PerformMonthEndTransactions()
        {
            if (Balance > 500m)
            {
                this.Balance *= 0.05m;
            }
        }
        
       
    }
}
