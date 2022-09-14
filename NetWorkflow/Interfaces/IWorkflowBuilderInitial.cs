
using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilderInitial<TContext> : IWorkflowBuilder<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<WorkflowStep<Tout>>> func);

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<TContext, WorkflowStep<Tout>>> func);
    }
}
