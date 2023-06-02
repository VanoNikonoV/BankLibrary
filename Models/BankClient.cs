using System;
using System.ComponentModel;
using System.Windows;

namespace Bank.Models
{
    public class BankClient<T> : IEquatable<BankClient<T>>, INotifyPropertyChanged where T : Account
    {
        /// <summary>
        /// Cведения о владельце счета
        /// </summary>
        public Client Owner { get; set; }

        /// <summary>
        /// Депозитный счет
        /// </summary>
        public T Deposit { get; set; }

        /// <summary>
        /// Депозитный счет
        /// </summary>
        public T NoDeposit { get; set; }

        /// <summary>
        /// Конструктор клиента банка, с возможностью завести два счета
        /// </summary>
        /// <param name="bankClient">Базованя информация о клиенте</param>
        /// <param name="deposit">Депозитный счет</param>
        /// <param name="noDeposit">Не депозитный счет</param>
        public BankClient(Client owner)
        {
            this.Owner = owner;

            Deposit = null;
            NoDeposit = null;
  
        }

        /// <summary>
        /// Открытие нового депозитного счета 
        /// </summary>
        /// <param name="selectedAccount">Выбранный тип счета</param>
        /// <param name="initialBalance">Начальный баланс при открытии счета</param>
        public void AddAccount(AccountType selectedAccount, decimal initialBalance)
        {
            switch (selectedAccount)
            {
                case AccountType.Deposit:

                    this.Deposit = new DepositAccount(initialBalance) as T;

                    OnPropertyChanged(nameof(Deposit));

                    break;
                case AccountType.NoDeposit:

                    this.NoDeposit = new NoDepositAccount(initialBalance) as T;

                    OnPropertyChanged(nameof(NoDeposit));

                    break;
                default:
                    MessageBox.Show("Что-то пошло не так!");
                    break;
            }
        }
        /// <summary>
        /// Закрытие депозитного счета
        /// </summary>
        public void CloseAccount(AccountType selectedAccount) 
        {
            switch (selectedAccount)
            {
                case AccountType.Deposit:

                    this.Deposit = null;

                    OnPropertyChanged(nameof(Deposit));

                    break;
                case AccountType.NoDeposit:

                    this.NoDeposit = null;

                    OnPropertyChanged(nameof(NoDeposit));

                    break;
                default:
                    MessageBox.Show("Что-то пошло не так!");
                    break;
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        public bool Equals(BankClient<T> other)
        {
            if (this.Owner.FirstName == other.Owner.FirstName
                && this.Owner.SecondName == other.Owner.SecondName
                && this.Owner.MiddleName == other.Owner.MiddleName
                && this.Owner.SeriesAndPassportNumber == other.Owner.SeriesAndPassportNumber
                && this.Owner.Telefon == other.Owner.Telefon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
