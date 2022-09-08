
namespace NetWorkflow
{
    public interface IWorkflowBuilder<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Func<WorkflowStep<Tout>> func);

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Func<TContext, WorkflowStep<Tout>> func);
    }
}
