using Bank.Commands;
using Bank.DataAccesses;
using Bank.Interfases;
using Bank.Models;
using Bank.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;

namespace Bank.ViewModels
{
    /// <summary>
    ///  ViewModel с ограничеными операциями над клиентом банка 
    /// </summary>
    public class Consultant : WorkspaceViewModel
    {
        public Consultant(BankRepository bankClients)
        {
            this.BankClients = bankClients;

            this.conditionsForListOfAccounts = new string[3] { "Депозитные счета", "Недепозитные счета", "Себе на другой счет" };

            this.ListRecipients = Enumerable.Empty<BankClient<Account>>();

            this.validator = new ClientValidator();

            this.errorDataClient = new List<string>();
            
        }

        /// <summary>
        /// Событие возникающее при редактировании данных клиента
        /// </summary>
        public event Action<InformationAboutChanges> OnEditClient;

        #region Поля
        /// <summary>
        /// Получатель платежа
        /// </summary>
        private BankClient<Account> recipient;

        /// <summary>
        /// Получатель платежа(Account)
        /// </summary>
        private Account recipientAccount;

        /// <summary>
        /// Текущий выбранный счет списания, присваивается в методе 
        /// CanMakeTransfer на основаниия активного Grida(Uid) из View.
        /// Используется в методе ChangedAccount() при операции перевод самому себе.
        /// </summary>
        private string _selectedAccount;

        private BankClient<Account> _client;

        private readonly string[] conditionsForListOfAccounts;

        private string changingSelectionListOfAccounts;

        private IEnumerable<BankClient<Account>> listRecipients;

        string sumAddDeposit = string.Empty;

        string sumAddNoDeposit = string.Empty;

        private ClientValidator validator;

        private bool isValid = false;

        private List<string> errorDataClient;

        #endregion

        #region Свойства
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get => _client?.Owner.FirstName; }
        /// <summary>
        /// Отчество клиента
        /// </summary>
        public string MiddleName { get => _client?.Owner.MiddleName; }

        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string SecondName { get => _client?.Owner.SecondName; }

        /// <summary>
        /// Серия и номер паспотра клиента
        /// </summary>
        public string SeriesAndPassportNumber { get => _client?.Owner.SeriesAndPassportNumber; }

        /// <summary>
        /// Телефон клиента
        /// </summary>
        public string Telefon { get => _client?.Owner.Telefon; }

        /// <summary>
        /// Репозиторий клиентов банка
        /// </summary>
        public BankRepository BankClients { get; }

        /// <summary>
        /// Текущий клиент с которым производятся операции
        /// </summary>
        public BankClient<Account> Client
        {
            get => _client;
            set
            {
                Set(ref _client, value, "");
                /*
                 * Сброс списка получателя платежа при смене отправителя
                 * Может потребоваться при активации условий сортировки
                 * списка получателей платежа, когда Client еще не выбран!
                 * Следовательно он поподает в список потенциальных получателей
                 */
                this.ListRecipients = Enumerable.Empty<BankClient<Account>>();
            }
        }

        /// <summary>
        /// Список условий для Combobox "Отсортируйте получателей"
        /// </summary>
        public string[] ConditionsForListOfAccounts { get => conditionsForListOfAccounts; }
        /// <summary>
        /// Список получателей платежа наполняется каждый раз при смене условий определенных ConditionsForListOfAccounts
        /// см. свойство ChangingSelectionListOfAccounts привязаное к SelectedItem Combobox "Отсортируйте получателей"
        /// </summary>
        public IEnumerable<BankClient<Account>> ListRecipients
        {
            get => listRecipients;

            set => Set(ref listRecipients, value, "ListRecipients");
        }
        /// <summary>
        /// Отслеживает смену критеря соритировки спика получателей и соответственно наполняет это список
        /// Привязано к SelectedItem Combobox "Отсортируйте получателей"
        /// </summary>
        public string ChangingSelectionListOfAccounts
        {
            get => changingSelectionListOfAccounts;

            set
            {
                if (value == changingSelectionListOfAccounts) //|| String.IsNullOrEmpty(value)
                    return;

                changingSelectionListOfAccounts = value;
                OnPropertyChanged(nameof(ChangingSelectionListOfAccounts));


                if (changingSelectionListOfAccounts == ConditionsForListOfAccounts[0]) //Депозитные счета
                {
                    this.ListRecipients = from client in BankClients
                                          where client.Deposit != null
                                          where client.Owner.ID != Client?.Owner.ID
                                          select client;
                }
                if (changingSelectionListOfAccounts == ConditionsForListOfAccounts[1]) //Недепозитные счета
                {
                    this.ListRecipients = from client in BankClients
                                          where client.NoDeposit != null
                                          where client.Owner.ID != Client?.Owner.ID
                                          select client;

                }
                if (changingSelectionListOfAccounts == ConditionsForListOfAccounts[2]) // Самому себе
                {
                    this.ListRecipients = from client in BankClients
                                          where client.Owner.ID == Client?.Owner.ID
                                          select client;
                }
                if (changingSelectionListOfAccounts == null) // Сброс списка при выполнении операции перевода
                {
                    this.ListRecipients = Enumerable.Empty<BankClient<Account>>();
                }
            }
        }

        /// <summary>
        /// Получатель платежа
        /// </summary>
        public BankClient<Account> Recipient
        {
            get => recipient;
            set
            {
                Set(ref recipient, value);
                ChangedAccount();
            }
        }

        /// <summary>
        /// Денежная сумма для перевода на другой счет, пополнения своего депозитного счета
        /// </summary>
        public string SumAddDeposit
        {
            get => sumAddDeposit;

            set { Set(ref sumAddDeposit, value, "SumAddDeposit"); }
        }

        /// <summary>
        /// Денежная сумма для перевода на другой счет, пополнения своего недепозитного счета
        /// </summary>
        public string SumAddNoDeposit
        {
            get => sumAddNoDeposit;

            set { Set(ref sumAddNoDeposit, value, "SumAddNoDeposit"); }
        }

        /// <summary>
        /// Указаывает на наличие ощибки в данных о клиенте
        /// </summary>
        public bool IsValid
        {
            get => isValid;

            set { Set(ref isValid, value, "IsValid"); }
        }
        /// <summary>
        /// Список ощибок выявленных при проверке
        /// </summary>
        public List<string> ErrorDataClient
        {
            get => errorDataClient;

            set { Set(ref errorDataClient, value, "ErrorDataClient"); }
        }

        #endregion

        #region Команды
        private RelayCommandT<string> _editTelefonCommand = null;
        /// <summary>
        /// Команда для редактирования телефона клиента
        /// </summary>
        public RelayCommandT<string> EditTelefonCommand
            => _editTelefonCommand ?? (_editTelefonCommand = new RelayCommandT<string>(EditTelefon, CanEditTelefon));

        private RelayCommandT<string> addAccountCommand = null;
        /// <summary>
        /// Команда добавление счета для выбранного клиента 
        /// </summary>
        public RelayCommandT<string> AddAccountCommand =>
            addAccountCommand ?? (addAccountCommand = new RelayCommandT<string>(AddAccount, CanAddAccount));


        private RelayCommandT<string> closeAccountCommand = null;
        /// <summary>
        /// Команда закрытия счета для выбранного клиента
        /// </summary>
        public RelayCommandT<string> CloseAccountCommand =>
            closeAccountCommand ?? (closeAccountCommand = new RelayCommandT<string>(CloseAccount, CanCloseAccount));

        private RelayCommandT<string> makeTransfer = null;

        /// <summary>
        /// Команда для перевод между счетами
        /// </summary>
        public RelayCommandT<string> MakeTransfer => makeTransfer ?? (makeTransfer =
            new RelayCommandT<string>(TransferExecuted, CanMakeTransfer));

        private RelayCommandT<string> makeDepositCommand = null;
        /// <summary>
        /// Команда для пополнения счета
        /// </summary>
        public RelayCommandT<string> MakeDepositCommand => makeDepositCommand ?? (makeDepositCommand =
            new RelayCommandT<string>(MakeDepositExecuted, CanMakeDeposit));

        #endregion

        #region Редактирование личных данных клиента
        /// <summary>
        /// Опреляет допустимость нового значения номером телефона 
        /// </summary>
        /// <param name="telefon">Содержимое текстового поля из View</param>
        /// <returns>true - если содержимое валидно
        /// false - если содержимое невалидно</returns>
        private bool CanEditTelefon(string telefon)
        {
            if (telefon != null && !String.IsNullOrWhiteSpace(telefon) && !IsValid)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Метод редактирование телефона клиента
        /// </summary>
        /// <param name="telefon"></param>
        private void EditTelefon(string telefon)
        {
            string currentTelefon = Client.Owner.Telefon;

            Client.Owner.Telefon = telefon;

            bool equal = telefon.Equals(currentTelefon);

            if (ValidateCustomer(Client.Owner, "Telefon") && !equal)
            {
                BankClient<Account> client = BankClients.Find(i => i.Owner.ID == Client.Owner.ID); //try client == null

                client.Owner.Telefon = telefon;

                OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                $"Замена {currentTelefon} на {telefon}", Client.Owner.ID));
            }
            else { Client.Owner.Telefon = currentTelefon; SystemSounds.Beep.Play(); }
        }
        #endregion

        #region Методы для работы со счетами
        /// <summary>
        /// Проверка наличия открытого счета у клиента
        /// </summary>
        /// <param name="selectedAccount">Тип счета - Uid = Deposit/NoDeposit активного Grida из View</param>
        /// <returns>true - если у выбранного клиента открыт счет
        /// false - если счет не открыт</returns>
        private bool CanCloseAccount(string selectedAccount)
        {
            switch (selectedAccount)
            {
                case "Deposit":
                    return Client?.Deposit != null ? true : false;
                case "NoDeposit":
                    return Client?.NoDeposit != null ? true : false;

                default: return false;
            }
        }

        /// <summary>
        /// Удаляет счет для выбранного клиента
        /// </summary>
        private void CloseAccount(string selectedAccount)
        {
            if (Client != null)
            {
                switch (selectedAccount)
                {
                    case "Deposit":

                        Client.CloseAccount(AccountType.Deposit); //удаление в списке для консультанты

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Закрытие депозитного счета {Client.Owner.FirstName} {Client.Owner.SecondName}", Client.Owner.ID));

                        break;

                    case "NoDeposit":

                        Client.CloseAccount(AccountType.NoDeposit);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Закрытие недепозитного счета {Client.Owner.FirstName} {Client.Owner.SecondName}", Client.Owner.ID));

                        break;
                }
            }
            else MessageBox.Show(messageBoxText: "Выберите клиента",
                             caption: "Ощибка в данных",
                             MessageBoxButton.OK,
                             icon: MessageBoxImage.Error);

            //MWindow.ViewModel.ShowStatusBarText("Выберите клиента");
        }

        /// <summary>
        /// Проверка наличия открытого счета у клиента
        /// </summary>
        /// <param name="selectedAccount">Тип счета - Uid = Deposit/NoDeposit активного Grida из View</param>
        /// <returns>false - если у выбранного клиента отрыт счет
        /// true - если счет не открыт</returns>
        private bool CanAddAccount(string selectedAccount)
        {
            switch (selectedAccount)
            {
                case "Deposit":
                    return Client?.Deposit != null ? false : true;
                case "NoDeposit":
                    return Client?.NoDeposit != null ? false : true;

                default: return false;
            }
        }

        /// <summary>
        /// Добавление счета для выбранного клиента
        /// </summary>
        private void AddAccount(string selectedAccount)
        {
            if (Client != null)
            {
                switch (selectedAccount)
                {
                    case "Deposit":

                        Client.AddAccount(AccountType.Deposit, 100);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Открытие депозитного счета {Client.Owner.FirstName} {Client.Owner.SecondName}", Client.Owner.ID));

                        break;

                    case "NoDeposit":

                        Client.AddAccount(AccountType.NoDeposit, 100);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Открытие недепозитного счета {Client.Owner.FirstName} {Client.Owner.SecondName}", Client.Owner.ID));

                        break;

                }
            }
            else MessageBox.Show(messageBoxText: "Выберите клиента",
                             caption: "Ощибка в данных",
                             MessageBoxButton.OK,
                             icon: MessageBoxImage.Error);
        }

        /// <summary>
        /// Выбран ли получатель из списка ListRecipients для выполнения перевода
        /// и задает правильный счет для перевода самому себе
        /// </summary>
        /// <param name="selectedAccount">Тип счета - Uid = Deposit/NoDeposit активного Grida из View</param>
        /// <returns>false - если получать выбран
        ///          true - если получать не выбран</returns>
        private bool CanMakeTransfer(string selectedAccount)
        {
            if (Recipient != null)
            {
                _selectedAccount = selectedAccount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Перевод денежных средств между счетами
        /// </summary>
        /// <<param name="selectedAccount">Тип счета - Uid = Deposit/NoDeposit активного Grida из View</param>
        private void TransferExecuted(string selectedAccount)
        {
            decimal amount;

            switch (selectedAccount)
            {
                case "Deposit":

                    if (Recipient.NoDeposit == null)
                    {
                        MessageBox.Show("У вас один счет");

                        SumAddDeposit = string.Empty;

                        ChangingSelectionListOfAccounts = null;

                        break;
                    }

                    if (Decimal.TryParse(SumAddDeposit, out amount))
                    {
                        recipientAccount = (recipientAccount as ICovAccount<Account>).MakeDeposit(amount);

                        (recipientAccount as IContrAccount<Account>).MakeWithdrawal(Client.Deposit, amount);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Перевод денежных средств {amount} руб. между депозитным счетом {Client.Owner.FirstName} {Client.Owner.SecondName} " +
                        $"на счет {recipientAccount.GetType()} {Recipient.Owner.FirstName} {Recipient.Owner.SecondName}", Client.Owner.ID));

                        SumAddDeposit = string.Empty;

                        ChangingSelectionListOfAccounts = null; 
                    }
                    else { MessageBox.Show("Нужно ввести число"); }

                    break;

                case "NoDeposit":

                    if (Recipient.Deposit == null)
                    {
                        MessageBox.Show("У вас один счет");

                        SumAddNoDeposit = string.Empty;

                        ChangingSelectionListOfAccounts = null;

                        break;
                    }

                    if (Decimal.TryParse(SumAddNoDeposit, out amount))
                    {
                        recipientAccount = (recipientAccount as ICovAccount<Account>).MakeDeposit(amount);

                        (recipientAccount as IContrAccount<Account>).MakeWithdrawal(Client.NoDeposit, amount);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Перевод денежных средств {amount} руб. между недепозитным счетом {Client.Owner.FirstName} {Client.Owner.SecondName} " +
                        $"на счет {recipientAccount.GetType()} {Recipient.Owner.FirstName} {Recipient.Owner.SecondName}", Client.Owner.ID));

                        SumAddNoDeposit = string.Empty;

                        ChangingSelectionListOfAccounts = null;

                    }
                    else { MessageBox.Show("Нужно ввести число"); }

                    break;
            }
        }

        /// <summary>
        /// Возможность выполнения операции пополнение счета
        /// </summary>
        /// <param name="selectedAccount">Тип счета - Uid = Deposit/NoDeposit активного Grida из View</param>
        /// <returns>Всегда true</returns>
        private bool CanMakeDeposit(string selectedAccount) 
        {
            if (Client != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Пополнение счета по соответствующему типу
        /// </summary>
        /// <param name="selectedAccount">Тип счета - Uid = Deposit/NoDeposit активного Grida из View</param>
        private void MakeDepositExecuted(string selectedAccount)
        {
            decimal amount;

            switch (selectedAccount)
            {
                case "Deposit":

                    if (Decimal.TryParse(SumAddDeposit, out amount))
                    {
                        Client.Deposit = (Client.Deposit as ICovAccount<Account>).MakeDeposit(amount);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Пополнение депозитного счета {Client.Owner.FirstName} {Client.Owner.SecondName} на {amount} руб.", Client.Owner.ID));

                        SumAddDeposit = string.Empty;
                    }
                    else { MessageBox.Show("Нужно ввести число"); }

                    break;

                case "NoDeposit":

                    if (Decimal.TryParse(SumAddNoDeposit, out amount))
                    {
                        Client.NoDeposit = (Client.NoDeposit as ICovAccount<Account>).MakeDeposit(amount);

                        OnEditClient?.Invoke(new InformationAboutChanges(DateTime.Now, this.GetType().Name,
                        $"Пополнение недепозитного счета {Client.Owner.FirstName} {Client.Owner.SecondName} на {amount} руб.", Client.Owner.ID));

                        SumAddNoDeposit = string.Empty;
                    }
                    else { MessageBox.Show("Нужно ввести число"); }

                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Скрытые метод для корректной работы класса
        /// <summary>
        /// Производит смену аккауна получателя при смене значения в Combobox "Отсортируйте получателей:" 
        /// </summary>
        private void ChangedAccount()
        {
            if (changingSelectionListOfAccounts == ConditionsForListOfAccounts[0]) //Депозитные счета
            {
                recipientAccount = Recipient?.Deposit;
            }
            if (changingSelectionListOfAccounts == ConditionsForListOfAccounts[1]) //Недепозитные счета
            {
                recipientAccount = Recipient?.NoDeposit;
            }
            if (changingSelectionListOfAccounts == ConditionsForListOfAccounts[2]) // Самому себе
            {
                if(_selectedAccount == "Deposit")
                recipientAccount = Recipient?.NoDeposit;

                else recipientAccount = Recipient?.NoDeposit;
            }
        }

        /// <summary>
        /// Метод валидации данных клиента, выводящий всплывающее сообщение о ощибке в данных
        /// </summary>
        /// <param name="customer">Клиен для проверки</param>
        /// <returns>true-в случае если данные валидны
        /// false  - в случае не корректных данных</returns>
        public bool ValidateCustomer(Client customer, string propertyName)
        {
            var result = validator.Validate(customer);

            if (result.IsValid) { return true; } //this.IsValid = false; 

            else
            {
                List<string> tempError = new List<string>();

                foreach (var error in result.Errors)
                {
                    if (error.PropertyName == propertyName){ tempError.Add(error.ErrorMessage);}    
                }
                this.ErrorDataClient = tempError;

                if (tempError.Count > 0) { this.IsValid = true; } //показывать Popup

                return tempError.Count > 0 ?  false : true;
            } 
        }

       
        #endregion
    }
}
