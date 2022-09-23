using NetWorkflow.Interfaces;
using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilderConditional<TContext, Tin> : IWorkflowBuilder<TContext, Tin>
    {
        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<IWorkflowStep<Tin, TNext>>> func);

        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<TContext, IWorkflowStep<Tin, TNext>>> func);

        public IWorkflowBuilderConditionalNext<TContext, Tin> Stop();
    }

    public interface IWorkflowBuilderConditional<TContext, Tin, Tout> : IWorkflowBuilderConditional<TContext, Tout>
    {

    }
}
