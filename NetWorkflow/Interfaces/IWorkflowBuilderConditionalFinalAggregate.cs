namespace NetWorkflow
{
    /// <summary>
    /// Interace that defines the aggregation after else is used within a conditional WorkflowBuilder
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context type</typeparam>
    public interface IWorkflowBuilderConditionalFinalAggregate<TContext>
    {
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
