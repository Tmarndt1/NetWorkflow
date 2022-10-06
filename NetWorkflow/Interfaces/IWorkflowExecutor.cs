namespace NetWorkflow
{
    public interface IWorkflowExecutor { }

    public interface IWorkflowExecutor<TIn, TOut> : IWorkflowExecutor   
    {
        public TOut? Run(TIn? args, CancellationToken token = default);
    }
}
