namespace NetWorkflow
{
    public abstract class Workflow : IWorkflow { }

    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context to be passed between the steps</typeparam>
    public abstract class Workflow<TContext> : Workflow, IWorkflow<TContext>
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
        public abstract void Build(IWorkflowBuilderInitial<TContext> builder);

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
    public abstract class Workflow<TContext, TResult> : Workflow, IWorkflow<TContext, TResult>
    {
        private readonly TContext _context;

        private readonly WorkflowBuilder<TContext> _next;

        public Workflow(TContext context)
        {
            _context = context;

            _next = new WorkflowBuilder<TContext>(_context);

            Build(_next);
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract IWorkflowBuilderNext<TContext, TResult> Build(IWorkflowBuilderInitial<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <returns>The TResult of the Workflow</returns>
        public TResult Run(CancellationToken token = default)
        {
            var results = _next.Run(null, token);

            return (TResult)results;
        }
    }
}