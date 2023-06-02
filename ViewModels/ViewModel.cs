using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bank.ViewModels
{
    /// <summary>
    /// Базовый класс модели представления
    /// Основная задача viewModel: содержание набора свойств которые нужно привязать
    /// к элемента интерфейса (MainWindow.xaml). Вся логика viewMode состоит в 
    /// изменение этих свойств, а элементы интерфейса будут обнаруживать 
    /// эти изменения и обновляться.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public virtual string DisplayName { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывается, когда этот объект удаляется из приложения и будет подвергнут сборке мусора.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Дочерние классы могут переопределять этот метод для выполнения
        /// логики очистки, такой как удаление обработчиков событий.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        /// <summary>
        /// Определяет изменилось ли свойство
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">поле свойства</param>
        /// <param name="value">новое значение</param>
        /// <param name="propName">наименование свойства</param>
        /// <returns></returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propName = null)
        {
           if(Equals(field, value))  return false;
           field = value;
           OnPropertyChanged(propName);
           return true;
        }
    }
}
