
namespace NetWorkflow
{
    public interface IWorkflowBuilderNext<TContext>
    {

    }

    public interface IWorkflowBuilderNext<TContext, Tout> : IWorkflowBuilderNext<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Func<WorkflowStep<Tout, TNext>> func);

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Func<TContext, WorkflowStep<Tout, TNext>> func);

        public IWorkflowBuilderNext<TContext, object[]> Parallel(Func<IEnumerable<WorkflowStep<Tout, object>>> func);

        public IWorkflowBuilderNext<TContext, object[]> Parallel(Func<TContext, IEnumerable<WorkflowStep<Tout, object>>> func);
    }

    public interface IWorkflowBuilderNext<TContext, Tin, Tout> : IWorkflowBuilderNext<TContext, Tout>
    {

    }
}
