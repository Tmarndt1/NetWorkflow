using System.Linq.Expressions;

namespace NetWorkflow
{
    public class WorkflowStoppedResult : WorkflowResult
    {
        public WorkflowStoppedResult(object args) : base($"{args} equals {args}")
        {
            
        }
    }
}
