
namespace NetWorkflow
{
    public abstract class WorkflowStep { }

    public abstract class WorkflowStep<Tout> : WorkflowStep
    {
        public abstract Tout Run(CancellationToken token = default);
    }

    public abstract class WorkflowStep<Tin, Tout> : WorkflowStep
    {
        public abstract Tout Run(Tin args, CancellationToken token = default);
    }

    public abstract class WorkflowStepAsync : WorkflowStep { }

    public abstract class WorkflowStepAsync<Tout> : WorkflowStepAsync
    {
        public abstract Task<Tout> RunAsync(CancellationToken token = default);
    }

    public abstract class WorkflowStepAsync<Tin, Tout> : WorkflowStepAsync
    {
        public abstract Task<Tout> RunAsync(Tin args, CancellationToken token = default);
    }
}
