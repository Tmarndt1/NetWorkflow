namespace NetWorkflow
{
    public class WorkflowStoppedException : Exception
    {
        public WorkflowStoppedException(WorkflowStoppedResult result) 
            : base($"The Workflow stopped because the following condition was met: {result.Message}")
        {

        }
    }
}
