namespace UMMBusinessLogic.Exceptions
{
    public sealed class IncompatibleMetricException : ApplicationException
    {
        public IncompatibleMetricException(string message)
            : base(message)
        { }
    }
}
