
using System;

namespace NetWorkflow
{
    /// <summary>
    /// Exception that is thrown when the maximum ammount of retries has been met within a Workflow
    /// </summary>
    public class WorkflowNoConditionMetException : Exception
    {
        public WorkflowNoConditionMetException() : base("No condition was met in the Workflow.")
        {

        }
    }
}
