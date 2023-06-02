using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Bank.View;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Toolkit.Uwp.Notifications;
using BankLibrary.DataAccesses;
using BankLibrary.Models;
using BankLibrary.Commands;

namespace Bank.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel(string clientsDataFile)
        {
            bankRepository = new BankRepository(clientsDataFile);

            AllClients = System.Windows.Data.CollectionViewSource.GetDefaultView(ViewClientsData(BankRepository));

            employeesTypeOptions = new string[2] { "Консультант", "Менеджер"};

            employeeType = employeesTypeOptions[0]; // по умолчанию

            Consultant = new Consultant(BankRepository);
            //Consultant
            Consultant.OnEditClient += OnEditClient;

            Manager = new Manager(BankRepository);
            Manager.OnEditClientForManager += OnEditClient;
            Manager.OnEditClient += OnEditClient;

            СhangeCurrentClient(); // значение по умолчанию
        }

        #region Поля
        private readonly string[] employeesTypeOptions;

        readonly string PATH = Directory.GetCurrentDirectory() + @"\Data\Log.json";

        private string employeeType;

        private BankRepository bankRepository;

        private Consultant consultant;

        private Manager manager;

        public WorkspaceViewModel workspaces;

        private System.ComponentModel.ICollectionView allClients;

        private BankClient<Account> currentClient;

        private bool isSorted = false;
        #endregion

        #region Свойства
        public Consultant Consultant 
        { 
            get => consultant;

            private set => Set(ref consultant, value);
        }

        public Manager Manager
        {
            get => manager;

            private set => Set(ref manager, value);
        }
        
        public BankRepository BankRepository { get { return bankRepository; } }

        /// <summary>
        /// Коллекция всех клиентов для отображения во View
        /// </summary>
        public System.ComponentModel.ICollectionView AllClients
        {
            get => allClients;

            set => Set(ref allClients, value, "AllClients"); 
        }

        /// <summary>
        /// Текущий выбранный клиент (см. SelectedItem => ListView => bankRepository.First)
        /// </summary>
        public BankClient<Account> CurrentClient 
        {
            get => currentClient;
           
            set 
            {
                Set(ref currentClient, value);

                СhangeCurrentClient();
            }
        }

        /// <summary>
        /// Массив сотрудников банка
        /// </summary>
        public string[] EmployeesTypeOptions { get => employeesTypeOptions; }

        /// <summary>
        /// Выбранный режим доступа (сотрудник банка)
        /// </summary>
        public string EmployeeType
        {
            get { return employeeType; }
            set
            {
                if (value == employeeType || String.IsNullOrEmpty(value))
                    return;

                employeeType = value;

                if (employeeType == employeesTypeOptions[0])
                {
                    this.AllClients = System.Windows.Data.CollectionViewSource.GetDefaultView(ViewClientsData(BankRepository));
                }

                if (employeeType == employeesTypeOptions[1])
                {
                    this.AllClients = System.Windows.Data.CollectionViewSource.GetDefaultView(BankRepository);
                }
                base.OnPropertyChanged(nameof(EmployeeType));
            }
        }

        /// <summary>
        /// Возвращает рабочее пространство для отображения во View в соответсвии с выбраным режимом
        /// </summary>
        public WorkspaceViewModel Workspaces
        {
            get { return workspaces; }

            set { Set(ref workspaces, value, "Workspaces"); }
        }
        #endregion

        #region Команды

        private RelayCommand newClientAddCommand = null;
        public RelayCommand NewClientAddCommand =>
            newClientAddCommand ?? (newClientAddCommand = new RelayCommand(AddNewClient, CanDeleteAndAddClient));

        private RelayCommand deleteClientCommand = null;
        public RelayCommand DeleteClientCommand =>
            deleteClientCommand ?? (deleteClientCommand = new RelayCommand(DeleteClient, CanDeleteAndAddClient));

        private RelayCommand saveRepoCommand = null;

        public RelayCommand SaveRepoCommand =>
            saveRepoCommand ?? (saveRepoCommand = new RelayCommand(SaveRepo, CanSaveRepo));

        private RelayCommand sortCommand = null;

        public RelayCommand SortCommand =>
            sortCommand ?? (sortCommand = new RelayCommand(SortBankRepository, null));

        #endregion

        /// <summary>
        /// Сортировка данных о клиентах по алфавиту
        /// </summary>
        private void SortBankRepository()
        {
            if (isSorted == false) 
            {           
                AllClients.SortDescriptions.Add(new SortDescription("Owner.FirstName", ListSortDirection.Ascending));

                isSorted = true;
            }
            else
            {
                AllClients.SortDescriptions.Clear();

                isSorted = false;
            }   
        }

        #region Методы для опраций с клиентом

        /// <summary>
        /// Возможность удаления клиента
        /// </summary>
        /// <returns> false - если выбран режим консультатн
        /// true - если выбран режим менеджер</returns>
        private bool CanDeleteAndAddClient()
        {
            return EmployeeType == employeesTypeOptions[0] ? false : true;
        }

        /// <summary>
        /// Метод удаления клиента
        /// </summary>
        private void DeleteClient()
        {
            if (CurrentClient != null)
            {
                BankRepository.Remove(CurrentClient);

                OnEditClient(new InformationAboutChanges(DateTime.Now, this.EmployeeType,
                $"Удален клиетна с ID: {CurrentClient.Owner.ID}", CurrentClient.Owner.ID));

                AllClients.Refresh();
            }
        }

        /// <summary>
        /// Метод добавления нового клиенита
        /// </summary>
        private void AddNewClient()
        {
            NewClientWindow _windowNewClient = new NewClientWindow() { Owner = Application.Current.MainWindow };

            NewClientWindowViewModel newClientVM = new NewClientWindowViewModel();

            _windowNewClient.DataContext = newClientVM;

            _windowNewClient.ShowDialog();

            if (_windowNewClient.DialogResult == true)
            {
                BankClient<Account> newAccount = new BankClient<Account>(newClientVM.NewClient);

                if (!BankRepository.Contains(newAccount))
                {
                    BankRepository.Add(newAccount);

                    AllClients.Refresh();

                    OnEditClient(new InformationAboutChanges(DateTime.Now, this.EmployeeType,
                        $"Добавлен новый клиетна с ID: {newAccount.Owner.ID}", newAccount.Owner.ID));
                }
                else
                {
                    string path = Directory.GetCurrentDirectory() + @"\Images\A_logo.png";

                    new ToastContentBuilder()
                        .AddArgument("visual", "viewConversation")
                        .AddArgument("conversationId", 9813)
                        .AddText("Ощибка в данных")
                        .AddText($"Клиент с такими данными уже существует")
                        .AddAppLogoOverride(new Uri(path), ToastGenericAppLogoCrop.Circle)
                        .Show(toast =>
                        {
                            toast.ExpirationTime = DateTime.Now.AddMilliseconds(1000);
                        });
                }
            }
        }
        #endregion

        /// <summary>
        /// Логирование операций с клиентами
        /// </summary>
        /// <param name="args">InformationAboutChanges args - Информация об измененых данных о клиенте</param>
        private void OnEditClient(InformationAboutChanges args) {BankRepository.LogClient.Add(args); }

        /// <summary>
        /// Определяет условия для необходимости сохраниться
        /// </summary>
        /// <returns>true - если есть изменения
        ///  false - нет изменений</returns>
        private bool CanSaveRepo()
        {
            if (BankRepository != null)
            {
                foreach (var client in BankRepository) //или AllClient?
                {
                    if (client.Owner.IsChanged == true) 
                    
                    { return true; }

                    else return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Сохранение всех данных о клиенте
        /// </summary>
        private void SaveRepo()
        {
            var saveDlg = new SaveFileDialog { Filter = "Text files|*.json" , InitialDirectory = Directory.GetCurrentDirectory()};

            if (true == saveDlg.ShowDialog())
            {
                string fileName = saveDlg.FileName;

                string json = JsonConvert.SerializeObject(BankRepository, Formatting.Indented);

                File.WriteAllText(fileName, json);

                foreach (var client in BankRepository as List<BankClient<Account>>)
                {
                    client.Owner.IsChanged = false;

                    AllClients.Refresh();
                }

            }

            string json2 = JsonConvert.SerializeObject(BankRepository.LogClient, Formatting.Indented);

            File.WriteAllText(PATH, json2);
        }

        #region Закрытие модели?
        //private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null && e.NewItems.Count != 0)
        //        foreach (WorkspaceViewModel workspace in e.NewItems)
        //            workspace.RequestClose += this.OnWorkspaceRequestClose;

        //    if (e.OldItems != null && e.OldItems.Count != 0)
        //        foreach (WorkspaceViewModel workspace in e.OldItems)
        //            workspace.RequestClose -= this.OnWorkspaceRequestClose;
        //}

        //private void OnWorkspaceRequestClose(object sender, EventArgs e)
        //{
        //    WorkspaceViewModel workspace = sender as WorkspaceViewModel;
        //    workspace.Dispose();
        //}
        #endregion

        /// <summary>
        /// Метод установки корректного типа данных для выбранного режима работы
        /// </summary>
        private void СhangeCurrentClient()
        {
            if (employeeType == employeesTypeOptions[0]) //консультант
            {
                Consultant.Client = this.CurrentClient;

                this.Workspaces = Consultant;
            }

            if (employeeType == employeesTypeOptions[1]) // менеджер
            {
                Manager.Client = this.CurrentClient;

                this.Workspaces = Manager;
            }
        }

        /// <summary>
        /// Возвращает коллекцию клиентов банка со скрытими данными
        /// </summary>
        /// <returns>IEnumerable<BankAccount></returns>
        public ObservableCollection<BankClient<Account>> ViewClientsData(List<BankClient<Account>> bankRepository)
        {
            ObservableCollection<BankClient<Account>> clientsForConsultant = new ObservableCollection<BankClient<Account>>();

            foreach (BankClient<Account> client in bankRepository)
            {
                string concealment = ConcealmentOfSeriesAndPassportNumber(client.Owner.SeriesAndPassportNumber);

                Client temp = new Client(firstName: client.Owner.FirstName,
                                        middleName: client.Owner.MiddleName,
                                        secondName: client.Owner.SecondName,
                                           telefon: client.Owner.Telefon,
                           seriesAndPassportNumber: concealment,
                                          dateTime: client.Owner.DateOfEntry,
                                         currentId: client.Owner.ID,
                                         isChanged: client.Owner.IsChanged);

                temp.IsChanged = client.Owner.IsChanged;

                BankClient<Account> clone = new BankClient<Account>(temp);

                clone.Deposit = client.Deposit;

                clone.NoDeposit = client.NoDeposit;

                clientsForConsultant.Add(clone);
            }

            return clientsForConsultant; //clientsForConsultant;
        }

        /// <summary>
        /// Сокрыте паспортных данных клиента
        /// </summary>
        /// <param name="number">Паспорные данные</param>
        /// <returns>Скрытые данные либо "нет данных"</returns>
        private string ConcealmentOfSeriesAndPassportNumber(string number)
        {
            if (number.Length > 0 && number != null && number != String.Empty)
            {
                string data = number;

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < number.Length; i++)
                {
                    if (data[i] != ' ')
                    {
                        sb.Append('*');
                    }
                    else sb.Append(data[i]);
                }
                return sb.ToString();
            }

            else return "нет данных";
        }
    }
}