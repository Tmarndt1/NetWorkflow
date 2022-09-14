namespace NetWorkflow.Interfaces
{
    public interface IWorkflowBuilderFinal<TContext>
    {
    }

    public interface IWorkflowBuilderFinal<TContext, TResult> : IWorkflowBuilderFinal<TContext>
    {

    }
}
