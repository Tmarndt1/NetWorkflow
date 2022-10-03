using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines all the useable methods extending from a conditional WorkflowBuilder type
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context type</typeparam>
    /// <typeparam name="TIn">The type of the incoming parameter</typeparam>
    public interface IWorkflowBuilderConditionalFinal<TContext, TIn> : IWorkflowBuilder<TContext, TIn>
    {
        /// <summary>
        /// Defines the WorkflowStep to execute if the condition is true
        /// </summary>
        /// <typeparam name="TNext">The return type of the WorkflowStep</typeparam>
        /// <param name="func">A function that returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalFinalAggregate<TContext> Do<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func);

        /// <summary>
        /// Defines the WorkflowStep to execute if the condition is true
        /// </summary>
        /// <typeparam name="TNext">The return type of the WorkflowStep</typeparam>
        /// <param name="func">A function that requires the Workflow's context and returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalFinalAggregate<TContext> Do<TNext>(Expression<Func<TContext, IWorkflowStep<TIn, TNext>>> func);

        /// <summary>
        /// Designates the Workflow to stop execution if the condition is true
        /// </summary>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalFinalAggregate<TContext> Stop();

        /// <summary>
        /// Designates the Workflow to throw an exception if the condition is true
        /// </summary>
        /// <param name="func">A function that returns an exception</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalFinalAggregate<TContext> Throw(Expression<Func<Exception>> func);
    }
}
