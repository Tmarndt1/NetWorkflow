namespace NetWorkflow
{
    public abstract class Workflow : IWorkflow { }


    /// <summary>
    /// Defines a Workflow and runs the various steps in sequence
    /// </summary>
    /// <typeparam name="TContext">The Workflow's context to be passed between the steps</typeparam>
    public abstract class Workflow<TContext, TResult> : Workflow, IWorkflow<TContext, TResult>
    {
        private readonly WorkflowBuilder<TContext> _next;

        protected Workflow(TContext context)
        {
            _next = new WorkflowBuilder<TContext>(context);

            Build(_next);
        }

        /// <summary>
        /// Abstract method that injects a IWorkflowBuilder to build the steps of the Workflow
        /// </summary>
        /// <param name="builder">The IWorkflowBuilder to build the Workflow's steps</param>
        public abstract IWorkflowBuilder<TContext, TResult> Build(IWorkflowBuilder<TContext> builder);

        /// <summary>
        /// Runs the Workflow and provides a WorkflowResult
        /// </summary>
        /// <returns>The TResult of the Workflow</returns>
        public TResult? Run(CancellationToken token = default)
        {
            var results = _next.Run(null, token);

            return (TResult?)results;
        }
    }
}