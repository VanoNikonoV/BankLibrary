using Bank.Validators;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;

namespace Bank.Models

/// <summary>
/// Класс описывающий модель клиента
/// </summary>
{
    public partial class Client : INotifyPropertyChanged, IDataErrorInfo, IEquatable<Client>
    {
        #region IDataErrorInfo
        [JsonIgnore]
        public string this[string columnName]
        {
            get
            {
                if (validator == null)
                {
                    validator = new ClientValidator();
                }
                var firstOrDefault = validator.Validate(this)
                    .Errors.FirstOrDefault(lol => lol.PropertyName == columnName);
                return firstOrDefault?.ErrorMessage;
            }
        }
        [JsonIgnore]
        public string Error
        {
            get
            {
                var results = validator.Validate(this);

                if (results != null && results.Errors.Any())
                {
                    var errors = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());

                    return errors;
                }

                return string.Empty;
            }
        }
        #endregion

        public bool Equals(Client other)
        {
            if (this.FirstName == other.FirstName
                && this.SecondName == other.SecondName
                && this.MiddleName == other.MiddleName
                && this.SeriesAndPassportNumber == other.SeriesAndPassportNumber
                && this.Telefon == other.Telefon)
            {
                return true;
            }
            else{  return false;  }
        }
    }
}

