using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines the following methods after the first conditional within a Workflow
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context type</typeparam>
    /// <typeparam name="TIn">The type of the incoming parameter</typeparam>
    public interface IWorkflowBuilderConditionalNext<TContext, TIn>
    {
        /// <summary>
        /// Defines a condition to execute in the Workflow if the condition is tre
        /// </summary>
        /// <param name="func">A function that requires the end result of the previous WorkflowStep and returns a boolean result</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditional<TContext, TIn> ElseIf(Expression<Func<TIn, bool>> func);

        /// <summary>
        /// Defines a catch all scenario in the Workflow to execute if previous conditions weren't met
        /// </summary>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalFinal<TContext, TIn> Else();

        /// <summary>
        /// Ends the conditional statement within the Workflow and consolidates the result to use in the next WorkflowStep
        /// </summary>
        /// <returns>An instance of a WorkflowBuilder</returns>
        /// <remarks>
        /// For compile time validation because each conditional WorkflowStep can return a different result
        /// the type is boxed into an object
        /// </remarks>
        public IWorkflowBuilderNext<TContext, object> EndIf();
    }
}
