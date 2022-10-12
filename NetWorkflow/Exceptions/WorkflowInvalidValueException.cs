using System;

namespace NetWorkflow.Exceptions
{
    /// <summary>
    /// Exception that is thrown when an invalid value is set
    /// </summary>
    public class WorkflowInvalidValueException : Exception
    {
        public WorkflowInvalidValueException(string message) : base(message)
        {
        }
    }
}

