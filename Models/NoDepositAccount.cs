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
    /// Счет для начисления процентов
    /// </summary>
    public class NoDepositAccount:Account, ICovAccount<Account>, IContrAccount<Account>
    {
        public NoDepositAccount( decimal initialBalance) : base(initialBalance) { }

        public Account MakeDeposit(decimal amount)
        {
            this.Balance += amount;

            return this;
        }

        public void MakeWithdrawal(Account client, decimal amount)
        {
            if (client.Balance >= amount)
            {
                client.Balance -= amount;
            }
            else MessageBox.Show("Недостаточно средств");
        }
    }
}
