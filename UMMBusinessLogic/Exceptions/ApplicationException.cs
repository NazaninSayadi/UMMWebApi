namespace UMMBusinessLogic.Exceptions
{
    using System;
    public class ApplicationException : Exception
    {
        public ApplicationException(string businessMessage)
               : base(businessMessage)
        {
        }
    }
}
