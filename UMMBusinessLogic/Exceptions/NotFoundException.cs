﻿namespace UMMBusinessLogic.Exceptions
{
    public sealed class NotFoundException : ApplicationException
    {
        public NotFoundException(string message)
            : base(message)
        { }
    }
}
