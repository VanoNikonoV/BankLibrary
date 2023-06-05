using BankLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using System.Windows;

namespace BankLibrary.DataAccesses
{
    /// <summary>
    /// Репозиторий клиентов банка
    /// </summary>
    public class BankRepository : List<BankClient<Account>>
    {
        public BankRepository(string path)
        {
           LoadData(path);  
            //GetClientsRep(50);
        }

        /// <summary>
        /// Загрузка данных
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        private void LoadData(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);

                List<BankClient<Account>> temp = JsonConvert.DeserializeObject<List<BankClient<Account>>>(json);

                foreach (BankClient<Account> client in temp)
                {   // вызывается коструктор Клиента для интерации статического свойства ID

                    BankClient<Account> tempClient = new BankClient<Account>
                                    (new Client(firstName: client.Owner.FirstName,
                                                middleName: client.Owner.MiddleName,
                                                secondName: client.Owner.SecondName,
                                                    telefon: client.Owner.Telefon,
                                    seriesAndPassportNumber: client.Owner.SeriesAndPassportNumber,
                                                    dateTime: client.Owner.DateOfEntry));

                    tempClient.AddAccount(AccountType.Deposit, client.Deposit.Balance);

                    tempClient.AddAccount(AccountType.NoDeposit, client.NoDeposit.Balance);

                    this.Add(tempClient);
                }
            }
            else
            {
                string message = "Выберите файл с данными?";

                MessageBoxResult result = MessageBox.Show(message, caption: "Сбой загрузки данных", MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (result == MessageBoxResult.Yes)
                {
                    var openDlg = new OpenFileDialog()
                    {
                        Filter = "Text files|*.json",
                        InitialDirectory = Directory.GetCurrentDirectory()
                    };

                    if (true == openDlg.ShowDialog())
                    {
                        string fileName = openDlg.FileName;

                       
                            string json = File.ReadAllText(fileName);

                            List<BankClient<Account>> temp = JsonConvert.DeserializeObject<List<BankClient<Account>>>(json);

                            if (temp != null)
                            {
                                foreach (BankClient<Account> client in temp) //NullReferenceException
                                {
                                    BankClient<Account> tempClient = new BankClient<Account>
                                                    (new Client(firstName: client.Owner.FirstName,
                                                                middleName: client.Owner.MiddleName,
                                                                secondName: client.Owner.SecondName,
                                                                    telefon: client.Owner.Telefon,
                                                    seriesAndPassportNumber: client.Owner.SeriesAndPassportNumber,
                                                                    dateTime: client.Owner.DateOfEntry));

                                    tempClient.AddAccount(AccountType.Deposit, client.Deposit.Balance);

                                    tempClient.AddAccount(AccountType.NoDeposit, client.NoDeposit.Balance);

                                    this.Add(tempClient);
                                }
                            }
                            else
                            {
                                path = Environment.CurrentDirectory + @"\Data\Default.json";

                                File.Create(path).Close();

                                MessageBox.Show("Создан файл Default.json\n" + path, caption: "Не удалось открыть файл", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
     
                    }
                }
            }

        }

        private ObservableCollection<InformationAboutChanges> logClient;

        /// <summary>
        /// Журнал событий проиcходящих с клиентами
        /// </summary>
        public ObservableCollection<InformationAboutChanges> LogClient
        {
            get => logClient ?? (logClient = new ObservableCollection<InformationAboutChanges>());

            //private set
            //{
            //    if (this.logClient == value) return;

            //    this.logClient = value;
                
            //}
        }

        #region Автогенерация данных
        private void GetClientsRep(int count)
        {
            long telefon = 79020000000;
            long passport = 6650565461;

            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                telefon += i;

                passport += random.Next(1, 500);

                Client _c = new Client(firstNames[BankRepository.randomize.Next(BankRepository.firstNames.Length)],
                    middleNames[BankRepository.randomize.Next(BankRepository.middleNames.Length)],
                    secondNames[BankRepository.randomize.Next(BankRepository.secondNames.Length)],
                    telefon.ToString(),
                    passport.ToString());

                this.Add(new BankClient<Account> (_c));

                this[i].AddAccount(AccountType.Deposit, 100);
                this[i].AddAccount(AccountType.NoDeposit, 200);

            }
        }

        static readonly string[] firstNames;

        static readonly string[] middleNames;

        static readonly string[] secondNames;

        /// <summary>
        /// Генератор псевдослучайных чисел
        /// </summary>
        static Random randomize;

        /// <summary>
        /// Статический конструктор, в котором "хранятся"
        /// данные о именах и фамилиях баз данных firstNames и lastNames
        /// </summary>
        static BankRepository()
        {
            randomize = new Random();

            firstNames = new string[] {
                "Агата",
                "Агнес",
                "Мария",
                "Аделина",
                "Ольга",
                "Людмила",
                "Аманда",
                "Татьяна",
                "Вероника",
                "Жанна",
                "Крестина",
                "Анжела",
                "Маргарита"
            };

            middleNames = new string[]
            {
                "Ивановна",
                "Петровна",
                "Васильевна",
                "Сергеевна",
                "Дмитриевна",
                "Владимировна",
                "Александровна",
                "Тимофеевна"

            };

            secondNames = new string[]
            {
                "Иванова",
                "Петрова",
                "Васильева",
                "Кузнецова",
                "Ковалёва",
                "Попова",
                "Пономарёва",
                "Дьячкова",
                "Коновалова",
                "Соколова",
                "Лебедева",
                "Соловьёва",
                "Козлова",
                "Волкова",
                "Зайцева",
                "Ершова",
                "Карпова",
                "Щукина",
                "Виноградова",
                "Цветкова",
                "Калинина"
            };

        }
        #endregion
    }
}
