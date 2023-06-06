namespace BankLibrary
{
    [System.Serializable]
    public class AccountException : System.ApplicationException
    {
        public AccountException() { }
        public AccountException(string message) : base(message) { }
        public AccountException(string message, System.Exception inner) : base(message, inner) { }
        protected AccountException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    
}
