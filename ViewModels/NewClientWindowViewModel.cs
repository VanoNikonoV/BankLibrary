using Bank.Commands;
using Bank.Models;
using Bank.Validators;
using System.Windows;

namespace Bank.ViewModels
{
    /// <summary>
    /// Логика создание нового клиента
    /// </summary>
    internal class NewClientWindowViewModel
    {
        public NewClientWindowViewModel()
        {
            newClient = new Client();

            validator = new ClientValidator();
        }

        private Client newClient;

        private ClientValidator validator;

        /// <summary>
        /// Посути - результат работы класса, новый клиент
        /// </summary>
        public Client NewClient { get => newClient; }
        #region Команды

        private RelayCommandT<Window> addClientCommand = null;
        /// <summary>
        /// Команда для создания клиента
        /// </summary>
        public RelayCommandT<Window> AddClientCommand => addClientCommand ?? (new RelayCommandT<Window>(AddClient, CanAddClient));

        private RelayCommandT<Window> cancelCommand = null;

        public RelayCommandT<Window> CancelCommand => cancelCommand ?? (new RelayCommandT<Window>(Cancel, CanCancel));
        #endregion

        private bool CanCancel(Window window) { return true; }

        /// <summary>
        /// Зактытие окна бесподверждения сохранения клиента
        /// </summary>
        /// <param name="window"></param>
        private void Cancel(Window window) { window.DialogResult = false; }

        /// <summary>
        /// Выпоняется проверка данных клиента
        /// </summary>
        /// <param name="window">view - нового клиента</param>
        /// <returns>true - если данные валидны
        /// false - если есть ощибки(а)</returns>
        private bool CanAddClient(Window window) 
        {
            var result = validator.Validate(newClient);

            if (result.IsValid)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Создает нового клиента
        /// </summary>
        private void AddClient(Window window) { window.DialogResult = true;  }   
    }
}
