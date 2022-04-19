namespace UMMBusinessLogic.Exceptions
{
    public sealed class DuplicateMetricException : ApplicationException
    {
        public DuplicateMetricException(string message)
            : base(message)
        { }
    }
}
