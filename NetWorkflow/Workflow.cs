namespace NetWorkflow
{
    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context to be passed between the steps</typeparam>
    public abstract class Workflow<TContext> : IWorkflow<TContext>
    {
        private readonly TContext _context;

        private readonly WorkflowBuilder<TContext> _builder;

        public Workflow(TContext context)
        {
            _context = context;

            _builder = new WorkflowBuilder<TContext>(_context);

            Build(_builder);
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract void Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <returns>A WorkflowResult</returns>
        public void Run(CancellationToken token = default) => _builder.Run(token);
    }

    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context to be passed between the steps</typeparam>
    public abstract class Workflow<TContext, TResult> : IWorkflow<TContext, TResult>
    {
        private readonly TContext _context;

        private readonly WorkflowBuilder<TContext> _builder;

        public Workflow(TContext context)
        {
            _context = context;

            _builder = new WorkflowBuilder<TContext>(_context);

            Build(_builder);
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract void Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <returns>The TResult of the Workflow</returns>
        public TResult Run(CancellationToken token = default) => (TResult)_builder.Run(token);
    }
}