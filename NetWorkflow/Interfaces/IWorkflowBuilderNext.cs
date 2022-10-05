
using System.Linq.Expressions;

namespace NetWorkflow
{
    /// <summary>
    /// Interface that defines the base WorkflowSteps on a Workflow after the initial WorkflowStep has been determined.
    /// </summary>

    /// <typeparam name="TIn">The type of the incoming parameter.</typeparam>
    public interface IWorkflowBuilderNext<TIn> : IWorkflowBuilder<TIn>
    {
        /// <summary>
        /// Defines what WorkflowStep to execute next within the Workflow.
        /// </summary>
        /// <typeparam name="TNext">The WorkflowStep's output type.</typeparam>
        /// <param name="func">A function that returns a WorkflowStep.</param>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        public IWorkflowBuilderNext<TIn, TNext> Then<TNext>(Expression<Func<IWorkflowStep<TIn, TNext>>> func);

        /// <summary>
        /// Defines what asynchronous WorkflowSteps to execute next within the Workflow. Each WorkflowStep will execute on their own thread.
        /// </summary>
        /// <typeparam name="TNext">The WorkflowStep's output type.</typeparam>
        /// <param name="func">A function that returns an enumeration of WorkflowSteps.</param>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        /// <remarks>
        /// For compile time validation, each WorkflowStep must return the same type
        /// </remarks>
        public IWorkflowBuilderNext<TNext[]> Parallel<TNext>(Expression<Func<IEnumerable<IWorkflowStepAsync<TIn, TNext>>>> func);

        /// <summary>
        /// Defines the first conditional to execute following the last WorkflowStep.
        /// </summary>
        /// <param name="func">A function that requires the result of the previous Workflow and returns a boolean result.</param>
        /// <returns>An instance of a WorkflowBuilder.</returns>
        public IWorkflowBuilderConditional<TIn> If(Expression<Func<TIn, bool>> func);
    }

    public interface IWorkflowBuilderNext<TIn, TOut> : IWorkflowBuilderNext<TOut>
    {

    }
}
