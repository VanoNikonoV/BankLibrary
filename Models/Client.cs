using Bank.Validators;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Bank.Models

/// <summary>
/// Класс описывающий модель клиента
/// </summary>
{
    public partial class Client : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Статический конструктор
        private static int id;

        private static int NextID()
        {
            id++;
            return id;
        }
        static Client()
        {
            id = 0;
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор для нового клиента
        /// </summary>
        public Client() : this("Имя", "Отчество", "Фамилия", "+79000000000", "66 00 000000")
        { 
            --id;

        } 

        /// <summary>
        /// Вызывается при создании нового клиента
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="secondName"></param>
        /// <param name="telefon"></param>
        /// <param name="seriesAndPassportNumber"></param>
        public Client(string firstName, string middleName,
                      string secondName, string telefon,
                      string seriesAndPassportNumber) =>

                    (this.FirstName, this.MiddleName,
                     this.SecondName, this.Telefon,
                     this.SeriesAndPassportNumber, this.DateOfEntry,
                     this.ID, this.IsChanged) =

                    (firstName, middleName,
                     secondName, telefon,
                     seriesAndPassportNumber, DateTime.Now,
                     Client.NextID(), false);

        /// <summary>
        /// Вызывается при редактировании, перезаписывании клиента
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="middleName"></param>
        /// <param name="secondName"></param>
        /// <param name="telefon"></param>
        /// <param name="seriesAndPassportNumber"></param>
        /// <param name="currentId"></param>
        [JsonConstructor]
        public Client(string firstName, string middleName,
                          string secondName, string telefon,
                          string seriesAndPassportNumber,
                          int currentId, DateTime dateTime,
                          bool isChanged) =>

                         (this.FirstName, this.MiddleName,
                         this.SecondName, this.Telefon,
                         this.SeriesAndPassportNumber, this.DateOfEntry,
                         this.ID, this.IsChanged) =

                         (firstName, middleName,
                         secondName, telefon,
                         seriesAndPassportNumber, dateTime,
                         currentId, isChanged); 

        // для загрузки данных
        public Client(string firstName, string middleName,
                          string secondName, string telefon,
                          string seriesAndPassportNumber,
                          DateTime dateTime)

                          : this(firstName, middleName, secondName,
                                 telefon, seriesAndPassportNumber)
        {
            this.DateOfEntry = dateTime;
            this.IsChanged = false;
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName
        {
            get { return this.firstName; }

            set
            {
                if (this.firstName == value) return;

                    this.firstName = value;
                    OnPropertyChanged(nameof(FirstName));
            }
        }
        /// <summary>
        /// Отчество клиента
        /// </summary>
        public string MiddleName
        {
            get { return this.middleName; }

            set
            {
                if (middleName == value) return;
                
                this.middleName = value;
                OnPropertyChanged(nameof(MiddleName)); 
            }
        }
        /// <summary>
        /// Фамилия клиента
        /// </summary>
        public string SecondName
        {
            get { return this.secondName; }

            set 
            {
                if (secondName == value) return;
               
                this.secondName = value;
                OnPropertyChanged(nameof(SecondName));
            }
        }
        /// <summary>
        /// Телефон клиента
        /// </summary>
        public string Telefon
        {
            get { return this.telefon; }

            set 
            {
                if (telefon == value) return;
           
                this.telefon = value;
                OnPropertyChanged(nameof(Telefon));
            }
        }
        [JsonProperty("ID")]
        public int ID { get; private set; }

        /// <summary>
        /// Серия и номер паспотра клиента
        /// </summary>
        public string SeriesAndPassportNumber
        {
            get {return this.seriesAndPassportNumber;}
            set
            {
                this.seriesAndPassportNumber = value;
                OnPropertyChanged(nameof(SeriesAndPassportNumber));
            }
        }

        /// <summary>
        /// Индикатор наличия измнений
        /// </summary>
        [JsonIgnore]
        public bool IsChanged 
        { 
            get { return this.isChanged; }
            set
            {
                if (isChanged == value) return;
                {
                    this.isChanged = value;
                    OnPropertyChanged(nameof(IsChanged));
                } 
            }
        }
        
        /// <summary>
        /// Дата внесения изменений
        /// </summary>
        public DateTime DateOfEntry 
        { 
            get { return this.dateOfEntry; } 
            set
            {
                this.dateOfEntry = value;
            }
        }
        #endregion

        #region Поля
        private string firstName;
        private string secondName;  
        private string middleName;
        private string telefon;
        private string seriesAndPassportNumber;
        private DateTime dateOfEntry;
        private bool isChanged;
        private ClientValidator validator;

        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            if (propName != nameof(IsChanged))
            {
                this.IsChanged = true;
            }
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}

