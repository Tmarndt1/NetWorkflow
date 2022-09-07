
namespace NetWorkflow
{
    public interface IWorkflowStepFinal<in TContext>
    {
        public IWorkflowStepFinal<TContext> ExpireAt(TimeSpan timespan);
    }
}
