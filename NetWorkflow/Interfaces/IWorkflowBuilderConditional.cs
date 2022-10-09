using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines all the useable methods extending from a conditional WorkflowBuilder type.
    /// </summary>
    /// <typeparam name="TIn">The type of the incoming parameter.</typeparam>
    public interface IWorkflowBuilderConditional<TIn> : IWorkflowBuilder<TIn>
    {
        /// <summary>
        /// Defines the WorkflowStep to execute if the condition is true.
        /// </summary>
        /// <typeparam name="TNext">The return type of the WorkflowStep.</typeparam>
        /// <param name="func">A function that returns a WorkflowStep.</param>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        public IWorkflowBuilderConditionalNext<TIn> Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func);

        /// <summary>
        /// Designates the Workflow to stop execution if the condition is true.
        /// </summary>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        public IWorkflowBuilderConditionalNext<TIn> Stop();

        /// <summary>
        /// Designates the Workflow to throw an exception if the condition is true.
        /// </summary>
        /// <param name="func">A function that returns an exception.</param>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        public IWorkflowBuilderConditionalNext<TIn> Throw(Expression<Func<Exception>> func);

        /// <summary>
        /// Defines the Workflow to retry the previous WorkflowStep. 
        /// Optional max retries count can be passed in with the default value being 1.
        /// </summary>
        /// <param name="delay">The amount of time to delay before retrying.</param>
        /// <param name="maxRetries">Max number of retries before breaking.</param>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        public IWorkflowBuilderConditionalNext<TIn> Retry(TimeSpan delay, int maxRetries = 1);
    }

    public interface IWorkflowBuilderConditional<TIn, TOut> : IWorkflowBuilderConditional<TOut>
    {

    }
}
