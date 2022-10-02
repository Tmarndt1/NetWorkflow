namespace NetWorkflow.Interfaces
{
    /// <summary>
    /// Marker interface for the WorkflowStep
    /// </summary>
    public interface IWorkflowStep { }

    /// <summary>
    /// Interface that defines a WorkflowStep that requires no input and defines an output
    /// </summary>
    /// <typeparam name="TOut">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStep<TOut> : IWorkflowStep
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type TOut
        /// </summary>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A result of type TOut</returns>
        public TOut Run(CancellationToken token = default);
    }

    /// <summary>
    /// Interface that defines a WorkflowStep that requires an input and defines an output
    /// </summary>
    /// <typeparam name="TIn">The type of the previous output</typeparam>
    /// <typeparam name="TOut">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStep<TIn, TOut> : IWorkflowStep
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type TOut
        /// </summary>
        /// <param name="args">The required input of the WorkflowStep</param>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A result of type TOut</returns>
        public TOut Run(TIn args, CancellationToken token = default);
    }

    /// <summary>
    /// Marker interface that defines an async WorkflowStep
    /// </summary>
    public interface IWorkflowStepAsync : IWorkflowStep { }

    /// <summary>
    /// Interface that defines an async WorkflowStep that requires no input and defines an output
    /// </summary>
    /// <typeparam name="TOut">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStepAsync<TOut> : IWorkflowStepAsync
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type TOut
        /// </summary>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A task with a result of type TOut</returns>
        public Task<TOut> RunAsync(CancellationToken token = default);
    }

    /// <summary>
    /// Interface that defines an async WorkflowStep that requires an input and defines an output
    /// </summary>
    /// <typeparam name="TIn">The type of the previous output</typeparam>
    /// <typeparam name="TOut">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStepAsync<TIn, TOut> : IWorkflowStepAsync
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type TOut
        /// </summary>
        /// <param name="args">The required input of the async WorkflowStep</param>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A task with a result of type TOut</returns>
        public Task<TOut> RunAsync(TIn args, CancellationToken token = default);
    }
}
