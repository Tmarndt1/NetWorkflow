using System.Linq.Expressions;

namespace NetWorkflow
{
    public interface IWorkflowBuilder<TContext>
    {
        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<WorkflowStep<Tout>>> func);

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Expression<Func<TContext, WorkflowStep<Tout>>> func);
    }

    public interface IWorkflowBuilder<TContext, TResult>
    {

    }
}
