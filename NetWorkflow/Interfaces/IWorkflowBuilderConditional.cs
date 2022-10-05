﻿using System.Linq.Expressions;

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
    }

    public interface IWorkflowBuilderConditional<TIn, TOut> : IWorkflowBuilderConditional<TOut>
    {

    }
}
