
namespace NetWorkflow
{
    public class WorkflowStopped : WorkflowResult
    {
        public WorkflowStopped(object? args) : base($"{args} equals {args}")
        {
            
        }
    }
}
