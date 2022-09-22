namespace NetWorkflow
{
    public interface IWorkflowExecutor
    {
        internal object? Run(object? args, CancellationToken token = default);
    }
}
