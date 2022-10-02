using NetWorkflow.Interfaces;
using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines the base methods on a WorkflowBuilder
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context type</typeparam>
    public interface IWorkflowBuilder<TContext>
    {
        /// <summary>
        /// Defines what WorkflowStep to execute first within a Workflow
        /// </summary>
        /// <typeparam name="TNext">The WorkflowStep's output type</typeparam>
        /// <param name="func">A function that returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderNext<TContext, TNext> StartWith<TNext>(Expression<Func<IWorkflowStep<TNext>>> func);


        /// <summary>
        /// Defines what WorkflowStep to execute first within a Workflow
        /// </summary>
        /// <typeparam name="TNext">The WorkflowStep's output type</typeparam>
        /// <param name="func">A function that requires the Workflow's context and returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderNext<TContext, TNext> StartWith<TNext>(Expression<Func<TContext, IWorkflowStep<TNext>>> func);
    }

    /// <summary>
    /// Extended generic IWorkflowBuilder interface that defines the base methods on a WorkflowBuilder
    /// </summary>
    /// <typeparam name="TContext">The context of the Workflow</typeparam>
    /// <typeparam name="TResult">The end result of the Workflow</typeparam>
    public interface IWorkflowBuilder<TContext, TResult>
    {

    }
}
