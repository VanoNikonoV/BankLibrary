using System.ComponentModel;
using System.Diagnostics;

namespace BankLibrary.Models
{

    


    /// <summary>
    /// Базовый класс счета
    /// </summary>
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
        private static int accountNumberSeed = 123000;

        /// <summary>
        /// Конструтор Account 
        /// </summary>
        /// <param name="initialBalance">Начальный баланс при открытии счета</param>
        public Account(decimal initialBalance):this()
        {
            Balance = initialBalance;
        }

        public Account()
        {
            this.Number = accountNumberSeed.ToString();

            // accountNumberSeed++;

            if (accountNumberSeed > 123200) 
            { 
                throw new AccountException($"Номер счета превысил допустимый диапозон! Значение {accountNumberSeed}"); 
            }

            else accountNumberSeed++;

            Debug.Write("\n" + this.Number);
        }

        /// <summary>
        /// Операция с денежными средствами в конце каждого месяца
        /// </summary>
        public virtual void PerformMonthEndTransactions() { }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}
