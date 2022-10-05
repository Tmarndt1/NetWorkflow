
namespace NetWorkflow
{
    /// <summary>
    /// Exception that is thrown when the maximum ammount of retries has been met within a Workflow
    /// </summary>
    public class WorkflowMaxRetryException : Exception
    {
        public WorkflowMaxRetryException() : base("Max retry limit has been met within the Workflow")
        {

        }
    }
}
