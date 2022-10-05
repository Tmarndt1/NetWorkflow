using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines the base methods on a WorkflowBuilder
    /// </summary>
    public interface IWorkflowBuilder
    {
        /// <summary>
        /// Defines what WorkflowStep to execute first within a Workflow
        /// </summary>
        /// <typeparam name="TNext">The WorkflowStep's output type</typeparam>
        /// <param name="func">A function that returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderNext<TNext> StartWith<TNext>(Expression<Func<IWorkflowStep<TNext>>> func);
    }

    /// <summary>
    /// Extended generic IWorkflowBuilder interface that defines the base methods on a WorkflowBuilder
    /// </summary>
    /// <typeparam name="TResult">The end result of the Workflow</typeparam>
    public interface IWorkflowBuilder<TResult>
    {

    }
}
