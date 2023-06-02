using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bank.Interfases;

namespace Bank.Models
{
    //https://learn.microsoft.com/ru-ru/dotnet/csharp/fundamentals/tutorials/oop
    /* abstract Could not create an instance of type Bank.Models.Account. 
    * Type is an interface or abstract class and cannot be instantiated.
    * Path '[0].Deposit.Number', line 13, position 15."
    */

    public class Account : INotifyPropertyChanged  
    {
        /// <summary>
        /// Номер счета
        /// </summary>
        public string Number { get; }


        private decimal balance;
        /// <summary>
        /// Баланс
        /// </summary>
        public decimal Balance 
        {
            get => balance;

            set { 
                if (balance == value) return; 
                this.balance = value;
                OnPropertyChanged(nameof(Balance));
            }            
        }

        /// <summary>
        /// Номер счета
        /// </summary>
        private static int accountNumberSeed = 0;

        /// <summary>
        /// Конструтор Account 
        /// </summary>
        /// <param name="initialBalance">Начальный баланс при открытии счета</param>
        public Account(decimal initialBalance)
        {
            this.Number = accountNumberSeed.ToString();
            accountNumberSeed++;

            Balance = initialBalance;

            //Opened?.Invoke(this, new AccountStateHandler(date: DateTime.Now, notes: "open", whoChangedIt: "консультан", amount: initialBalance));
        }

        public Account()
        {
            this.Number = accountNumberSeed.ToString();
            accountNumberSeed++;
        }

        /// <summary>
        /// Операция с денежными средствами в конце каждого месяца
        /// </summary>
        public virtual void PerformMonthEndTransactions() { }

        /// <summary>
        /// Событие возникающее при открытии счета
        /// </summary>
        //protected internal event EventHandler<AccountStateHandler> Opened;

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}
