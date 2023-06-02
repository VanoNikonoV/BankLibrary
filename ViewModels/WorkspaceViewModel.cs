using Bank.Commands;
using System;
using System.Windows.Input;

namespace Bank.ViewModels
{
    /// <summary>
    ///  Этот подкласс ViewModelBase запрашивает удаление из пользовательского интерфейса при выполнении его команды Close.
    /// </summary>
    public abstract class WorkspaceViewModel:ViewModel
    { 
        #region Constructor

        protected WorkspaceViewModel()
        {
        }

        #endregion // Constructor     

        #region RequestClose [event]

        /// <summary>
        /// Возникает, когда это рабочее пространство должно быть удалено из пользовательского интерфейса.
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

    }
}
