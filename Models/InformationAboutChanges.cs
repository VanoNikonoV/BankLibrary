using System;
using System.ComponentModel;

namespace Bank.Models
{
    /// <summary>
    /// Информация об измененых данных о клиенте
    /// </summary>
    public class InformationAboutChanges : INotifyPropertyChanged
{
        /// <summary>
        /// Информация об измененых данных о клиенте
        /// </summary>
        /// <param name="dateTime">Дата изменения</param>
        /// <param name="whoChangedIt">Кто произмел изменение</param>
        /// <param name="note">Заметка о записи</param>
        /// <param name="idClient">ID клиента запись о котором изменялась</param>
        public InformationAboutChanges(DateTime dateTime, string whoChangedIt, string note, int idClient) =>
            
            (this.DateChenges, 
            this.WhoChangedIt, this.Note, this.IdClient) =

            (dateTime, whoChangedIt, note, idClient);
            
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime DateChenges { get;  }

        /// <summary>
        /// Кто произмел изменение
        /// </summary>
        public string WhoChangedIt { get; }

        /// <summary>
        /// Заметка о записи
        /// </summary>
        public string Note { get;}

        /// <summary>
        /// ID клиента запись о котором изменялась
        /// </summary>
        public int IdClient { get; }


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

    }

}
