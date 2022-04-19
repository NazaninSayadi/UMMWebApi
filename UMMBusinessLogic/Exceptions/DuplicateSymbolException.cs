namespace UMMBusinessLogic.Exceptions
{
    public sealed class DuplicateSymbolException : ApplicationException
    {
        public DuplicateSymbolException(string message)
            : base(message)
        { }
    }
}
