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

        /// <summary>
        /// Property that determines if the workflow has been stopped
        /// </summary>
        public bool Stopped { get; private set; }

        private Action? _stoppedCallback;

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

        public Workflow<TContext, TResult> OnStopped(Action callback)
        {
            _stoppedCallback = callback;

            return this;
        }

        /// <summary>
        /// Runs the Workflow and returns a final result if executed step has executed successfully
        /// </summary>
        /// <param name="token">The CancellationToken to cancel the workflow</param>
        /// <returns></returns>
        public TResult? Run(CancellationToken token = default)
        {
            try
            {
                var result = _next.Run(null, token);

                if (result is WorkflowStoppedResult stopped)
                {
                    Stopped = true;

                    if (_stoppedCallback != null) _stoppedCallback.Invoke();

                    return default(TResult?);
                }

                return (TResult?)result;
            }
            catch
            {
                Stopped = true;

                throw;
            }
        }
    }
}