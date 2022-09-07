
namespace NetWorkflow
{
    public interface IWorkflowBuilder<TContext>
    {
        public IWorkflowStepBuilder<TContext, Tout> StartWith<Tout>(Func<WorkflowStep<Tout>> func);

        public IWorkflowStepBuilder<TContext, Tout> StartWith<Tout>(Func<TContext, WorkflowStep<Tout>> func);
    }
}
