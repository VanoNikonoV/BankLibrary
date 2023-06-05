using BankLibrary.Models;
using FluentValidation;
using System;
using System.Linq;
using System.Windows;

namespace BankLibrary.Validators
{
    /// <summary>
    /// Выполняет проверку вводимых данных о клиенте
    /// </summary>
    public class ClientValidator:AbstractValidator<Client>
    {
        public ClientValidator()
        {
                RuleFor(client => client.FirstName)
                    .NotEmpty().WithMessage("Имя не заполнено")
                    .Must(client => client.All(Char.IsLetter)).WithMessage("Имя должно собержать только буквы")
                    .Must(ExpansionString.StartsWithUpper).WithMessage("Имя должно начинаться с заглавной буквы");

                RuleFor(client => client.MiddleName)
                    .NotEmpty().WithMessage("Отчество не заполнено")
                    .Must(client => client.All(Char.IsLetter)).WithMessage("Отчество  должно собержать только буквы")
                    .Must(ExpansionString.StartsWithUpper).WithMessage("Отчество должно начинаться с заглавной буквы");

                RuleFor(client => client.SecondName)
                    .NotEmpty().WithMessage("Фамилия не заполнена")
                    .Must(client => client.All(Char.IsLetter)).WithMessage("Фамилия должно собержать только буквы")
                    .Must(ExpansionString.StartsWithUpper).WithMessage("Фамилия должно начинаться с заглавной буквы");

                RuleFor(client => client.SeriesAndPassportNumber)
                    .NotEmpty().WithMessage("Паспортные данные не заполнены");

                RuleFor(t => t.Telefon)
                    .NotEmpty().WithMessage("Нужно указать значение")
                    .Must(t => t.StartsWith("+79")).WithMessage("Номер долже начинаться +79...")
                    .Length(12).WithMessage("Длина должна быть {MinLength}. Текущая длина: {TotalLength}")
                    .Must(TelefonIsDigit).WithMessage("Номер долже содержать только цифры");
          

        }

        /// <summary>
        /// Показывает, относится ли указанные символы Юникода к категории десятичных цифр, 
        /// если value - не равно hull или string.Empty
        /// </summary>
        /// <param name="value">Номер телефона</param>
        /// <returns>true - если телефон состоит только из цифр,
        /// в противном случае - false</returns>
        private bool TelefonIsDigit(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            
            return value.Substring(1).All(Char.IsDigit) ? true : false;  
        }

    }
}
