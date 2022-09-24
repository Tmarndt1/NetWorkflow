namespace NetWorkflow
{
    public interface IWorkflowExecutor
    {
        public bool Stopped { get; }

        internal object? Run(object? args, CancellationToken token = default);
    }
}
