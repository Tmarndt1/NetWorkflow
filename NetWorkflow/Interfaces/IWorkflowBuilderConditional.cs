using NetWorkflow.Interfaces;
using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines all the useable methods extending from a conditional WorkflowBuilder type
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context type</typeparam>
    /// <typeparam name="Tin">The type of the incoming parameter</typeparam>
    public interface IWorkflowBuilderConditional<TContext, Tin> : IWorkflowBuilder<TContext, Tin>
    {
        /// <summary>
        /// Defines the WorkflowStep to execute if the condition is true
        /// </summary>
        /// <typeparam name="TNext">The return type of the WorkflowStep</typeparam>
        /// <param name="func">A function that returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<IWorkflowStep<Tin, TNext>>> func);

        /// <summary>
        /// Defines the WorkflowStep to execute if the condition is true
        /// </summary>
        /// <typeparam name="TNext">The return type of the WorkflowStep</typeparam>
        /// <param name="func">A function that requires the Workflow's context and returns a WorkflowStep</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalNext<TContext, Tin> Do<TNext>(Expression<Func<TContext, IWorkflowStep<Tin, TNext>>> func);

        /// <summary>
        /// Designates the Workflow to stop execution if the condition is true
        /// </summary>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalNext<TContext, Tin> Stop();

        /// <summary>
        /// Designates the Workflow to throw an exception if the condition is true
        /// </summary>
        /// <param name="func">A function that returns an exception</param>
        /// <returns>An instance of a WorkflowBuilder</returns>
        public IWorkflowBuilderConditionalNext<TContext, Tin> Throw(Expression<Func<Exception>> func);
    }

    public interface IWorkflowBuilderConditional<TContext, Tin, Tout> : IWorkflowBuilderConditional<TContext, Tout>
    {

    }
}
