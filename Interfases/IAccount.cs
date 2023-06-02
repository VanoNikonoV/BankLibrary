using Bank.Models;

namespace Bank.Interfases
{
    // Интерфейс с параметром ковариантного типа позволяет своим методам
    // возвращать аргументы производных типов, степень наследования у которых больше, чем у параметра типа.
    public interface ICovAccount<out T>
    {
        /// <summary>
        /// Пополнение счета, начальный баланс должен быть положительным
        /// </summary>
        /// <param name="amount">Денежная сумма</param>
        /// <returns></returns>
        T MakeDeposit(decimal amount);
    }
    //Интерфейс с параметром контравариантного типа позволяет своим методам принимать
    //аргументы производных типов, степень наследования у которых меньше, чем у параметра типа интерфейса.
    public interface IContrAccount<in K> where K : Account
    {
        /// <summary>
        /// Списание средст со счета, любой вывод не должен создавать отрицательный баланс
        /// </summary>
        /// <param name="client">Клент со счета которого нужно списать деньги</param>
        /// <param name="amount">Сумма списания</param>
        void MakeWithdrawal(K client, decimal amount);
       
    }
   

}
