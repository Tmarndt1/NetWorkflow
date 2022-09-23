using NetWorkflow.Interfaces;
using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilder<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<IWorkflowStep<Tout>>> func);

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<TContext, IWorkflowStep<Tout>>> func);
    }

    public interface IWorkflowBuilder<TContext, TResult>
    {

    }
}
