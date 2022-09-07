
namespace NetWorkflow
{
    public interface IWorkflowStepBuilder<TContext>
    {

    }

    public interface IWorkflowStepBuilder<TContext, Tout> : IWorkflowStepBuilder<TContext>
    {
        public IWorkflowStepBuilder<TContext, Tout, TNext> Then<TNext>(Func<WorkflowStep<Tout, TNext>> func);

        public IWorkflowStepBuilder<TContext, Tout, TNext> Then<TNext>(Func<TContext, WorkflowStep<Tout, TNext>> func);
    }

    public interface IWorkflowStepBuilder<TContext, Tin, Tout> : IWorkflowStepBuilder<TContext, Tout>
    {

    }
}
