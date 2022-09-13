
namespace NetWorkflow
{
    public interface IWorkflowBuilderNext<TContext>
    {

    }

    public interface IWorkflowBuilderNext<TContext, Tout> : IWorkflowBuilderNext<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Func<WorkflowStep<Tout, TNext>> func);

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Func<TContext, WorkflowStep<Tout, TNext>> func);

        public IWorkflowBuilderNext<TContext, object[]> Parallel<TNext>(Func<IEnumerable<WorkflowStepAsync<Tout, TNext>>> func);

        public IWorkflowBuilderNext<TContext, object[]> Parallel<TNext>(Func<TContext, IEnumerable<WorkflowStepAsync<Tout, TNext>>> func);
    }

    public interface IWorkflowBuilderNext<TContext, Tin, Tout> : IWorkflowBuilderNext<TContext, Tout>
    {

    }
}
