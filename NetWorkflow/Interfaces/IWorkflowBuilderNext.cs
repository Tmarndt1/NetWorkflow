
using NetWorkflow.Interfaces;
using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilderNext<TContext, Tout> : IWorkflowBuilder<TContext, Tout>
    {
        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<IWorkflowStep<Tout, TNext>>> func);

        public IWorkflowBuilderNext<TContext, Tout, TNext> Then<TNext>(Expression<Func<TContext, IWorkflowStep<Tout, TNext>>> func);

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<Tout, TNext>>>> func);

        public IWorkflowBuilderNext<TContext, TNext> Map<TNext>(Expression<Func<Tout, TNext>> func);

        public IWorkflowBuilderNext<TContext, TNext[]> Parallel<TNext>(Expression<Func<TContext, IEnumerable<IWorkflowStepAsync<Tout, TNext>>>> func);

        public IWorkflowBuilderConditional<TContext, Tout> If(Expression<Func<Tout, bool>> func);
    }

    public interface IWorkflowBuilderNext<TContext, Tin, Tout> : IWorkflowBuilderNext<TContext, Tout>
    {

    }
}
