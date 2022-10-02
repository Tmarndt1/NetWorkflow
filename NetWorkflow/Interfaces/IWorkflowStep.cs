namespace NetWorkflow.Interfaces
{
    /// <summary>
    /// Marker interface for the WorkflowStep
    /// </summary>
    public interface IWorkflowStep { }

    /// <summary>
    /// Interface that defines a WorkflowStep that requires no input and defines an output
    /// </summary>
    /// <typeparam name="Tout">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStep<Tout> : IWorkflowStep
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type Tout
        /// </summary>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A result of type Tout</returns>
        public Tout Run(CancellationToken token = default);
    }

    /// <summary>
    /// Interface that defines a WorkflowStep that requires an input and defines an output
    /// </summary>
    /// <typeparam name="Tin">The type of the previous output</typeparam>
    /// <typeparam name="Tout">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStep<Tin, Tout> : IWorkflowStep
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type Tout
        /// </summary>
        /// <param name="args">The required input of the WorkflowStep</param>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A result of type Tout</returns>
        public Tout Run(Tin args, CancellationToken token = default);
    }

    /// <summary>
    /// Marker interface that defines an async WorkflowStep
    /// </summary>
    public interface IWorkflowStepAsync : IWorkflowStep { }

    /// <summary>
    /// Interface that defines an async WorkflowStep that requires no input and defines an output
    /// </summary>
    /// <typeparam name="Tout">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStepAsync<Tout> : IWorkflowStepAsync
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type Tout
        /// </summary>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A task with a result of type Tout</returns>
        public Task<Tout> RunAsync(CancellationToken token = default);
    }

    /// <summary>
    /// Interface that defines an async WorkflowStep that requires an input and defines an output
    /// </summary>
    /// <typeparam name="Tin">The type of the previous output</typeparam>
    /// <typeparam name="Tout">The type of the WorkflowStep's output</typeparam>
    public interface IWorkflowStepAsync<Tin, Tout> : IWorkflowStepAsync
    {
        /// <summary>
        /// Runs the WorkflowStep and returns a result of type Tout
        /// </summary>
        /// <param name="args">The required input of the async WorkflowStep</param>
        /// <param name="token">A optional CancellationToken that is passed throughout the Workflow</param>
        /// <returns>A task with a result of type Tout</returns>
        public Task<Tout> RunAsync(Tin args, CancellationToken token = default);
    }
}
