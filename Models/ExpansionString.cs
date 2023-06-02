namespace Bank.Models
{
    public static class ExpansionString
    {
        /// <summary>
        /// Метод расшерения - определяет является ли первым символом 
        /// текущего экземпляра строки символ верхнего регистра
        /// </summary>
        /// <param name="str">Проеряемая строка</param>
        /// <returns>true - если превая буква заглавная
        /// false - если превая буква незаглавная </returns>
        public static bool StartsWithUpper(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            char ch = str[0];
            return char.IsUpper(ch);
        }
    }
}
