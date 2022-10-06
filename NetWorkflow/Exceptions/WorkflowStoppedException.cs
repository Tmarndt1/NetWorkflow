
namespace NetWorkflow
{
    /// <summary>
    /// Exception that is thrown when the Workflow has stopped
    /// </summary>
    public class WorkflowStoppedException : Exception
    {
        public WorkflowStoppedException() : base("The Workflow has stopped")
        {

        }
    }
}
