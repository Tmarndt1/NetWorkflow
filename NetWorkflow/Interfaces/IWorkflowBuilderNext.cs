
using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilderNext<TContext> : IWorkflowBuilder<TContext>
    {

    }

    public interface IWorkflowBuilderNext<TContext, Tout> : IWorkflowBuilderNext<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<WorkflowStep<Tout, TNext>>> func);

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<TContext, WorkflowStep<Tout, TNext>>> func);

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<WorkflowStepAsync<Tout, TNext>>>> func);

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<TContext, IEnumerable<WorkflowStepAsync<Tout, TNext>>>> func);
    }

    public interface IWorkflowBuilderNext<TContext, Tin, Tout> : IWorkflowBuilderNext<TContext, Tout>
    {

    }
}
