using Bank.Commands;
using Bank.DataAccesses;
using Bank.Models;
using System;
using System.Media;

namespace Bank.ViewModels
{
    /// <summary>
    /// ViewModel для операций над клиентом банка 
    /// </summary>
    public class Manager : Consultant
    {
        /// <summary>
        /// Событие возникающее при редактировании данных клиента
        /// </summary>
        public event Action<InformationAboutChanges> OnEditClientForManager;

        public Manager(BankRepository bankClients) : base(bankClients) { }

        #region Команды
        private RelayCommandT<string> editFirstNameCommand = null;
        /// <summary>
        /// Команда для редактирования имени клиента
        /// </summary>
        public RelayCommandT<string> EditFirstNameCommand =>
            editFirstNameCommand ?? (editFirstNameCommand = new RelayCommandT<string>(EditFirstName, CanEdit));


        private RelayCommandT<string> editMiddleNameCommand = null;
        /// <summary>
        /// Команда для редактирования отчества клиента
        /// </summary>
        public RelayCommandT<string> EditMiddleNameCommand =>
            editMiddleNameCommand ?? (editMiddleNameCommand = new RelayCommandT<string>(EditMiddleName, CanEdit));


        private RelayCommandT<string> editSecondNameCommand = null;
        /// <summary>
        /// Команда для редактирования фамилии клиента
        /// </summary>
        public RelayCommandT<string> EditSecondNameCommand =>
            editSecondNameCommand ?? (editSecondNameCommand = new RelayCommandT<string>(EditSecondName, CanEdit));


        private RelayCommandT<string> editSeriesAndPassportNumberCommand = null;
        /// <summary>
        /// Команда для редактирования паспортных данных клиента
        /// </summary>
        public RelayCommandT<string> EditSeriesAndPassportNumberCommand =>
            editSeriesAndPassportNumberCommand ?? (editSeriesAndPassportNumberCommand
            = new RelayCommandT<string>(EditSeriesAndPassportNumber, CanEdit));
        #endregion

        #region Закрытые методы для команд

        /// <summary>
        /// Опреляет допустимость нового значения 
        /// </summary>
        /// <param name="args">Содержимое текстового поля из View</param>
        /// <returns>true - если содержимое валидно
        /// false - если содержимое невалидно</returns>
        private bool CanEdit(string args)
        {
            if (!String.IsNullOrWhiteSpace(args) && args != null) { return true; }

            else { return false; }
        }

        /// <summary>
        /// Метод редактирования имени клиента
        /// </summary>
        /// <param name="newName">Новое имя клиента</param>
        private void EditFirstName(string newName)
        {
            string current = Client.Owner.FirstName;

            Client.Owner.FirstName = newName;

            bool equal = newName.Equals(current);

            if (ValidateCustomer(Client.Owner, "FirstName") && !equal)
            {
                BankClient<Account> client = BankClients.Find(i => i.Owner.ID == Client.Owner.ID);

                client.Owner.FirstName = newName;

                OnEditClientForManager?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,

                $"Замена {current} на {newName}", Client.Owner.ID));
            }
            else { Client.Owner.FirstName = current; SystemSounds.Beep.Play(); }
        }

        /// <summary>
        /// Метод редактирования отчества клиента
        /// </summary>
        /// <param name="newMiddleName">Новое отчество клиента</param>
        private void EditMiddleName(string newMiddleName)
        {
            string current = Client.Owner.MiddleName;

            Client.Owner.MiddleName = newMiddleName;

            bool equal = newMiddleName.Equals(current);

            if ( ValidateCustomer(Client.Owner, "MiddleName") && !equal)
            {
                BankClient<Account> client = BankClients.Find(i => i.Owner.ID == Client.Owner.ID);

                client.Owner.MiddleName = newMiddleName;

                OnEditClientForManager?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,

                $"Замена {current} на {newMiddleName}", Client.Owner.ID));
            }
            else { Client.Owner.MiddleName = current; SystemSounds.Beep.Play(); }
        }

        /// <summary>
        /// Метод редактирование фамили клиента
        /// </summary>
        /// <param name="secondName">Новая фамилия клиента</param>
        private void EditSecondName(string newSecondName)
        {
            string current = Client.Owner.SecondName;

            Client.Owner.SecondName = newSecondName;

            bool equal = newSecondName.Equals(current);

            if (ValidateCustomer(Client.Owner, "SecondName")&& !equal)
            {
                BankClient<Account> client = BankClients.Find(i => i.Owner.ID == Client.Owner.ID);

                client.Owner.SecondName = newSecondName;

                OnEditClientForManager?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,

                $"Замена {current} на {newSecondName}", Client.Owner.ID));
            }
            else { Client.Owner.SecondName = current; SystemSounds.Beep.Play(); }
        }

        /// <summary>
        /// Метод редактирование паспортных данных клиента
        /// </summary>
        /// <param name="passport">Новые паспортные данные</param>
        private void EditSeriesAndPassportNumber(string passport)
        {
            string current = Client.Owner.SeriesAndPassportNumber;

            Client.Owner.SeriesAndPassportNumber = passport;

            bool equal = passport.Equals(current);

            if (ValidateCustomer(Client.Owner, "SeriesAndPassportNumber") && !equal)
            {
                BankClient<Account> client = BankClients.Find(i => i.Owner.ID == Client.Owner.ID);

                client.Owner.SeriesAndPassportNumber = passport;

                OnEditClientForManager?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,

                $"Замена {current} на {passport}", Client.Owner.ID));
            }
            else { Client.Owner.SeriesAndPassportNumber = current; SystemSounds.Beep.Play(); }
        }
        #endregion
    }
}
