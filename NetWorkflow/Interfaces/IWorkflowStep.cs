namespace NetWorkflow.Interfaces
{
    public interface IWorkflowStep { }

    public interface IWorkflowStep<Tout> : IWorkflowStep
    {
        public Tout Run(CancellationToken token = default);
    }

    public interface IWorkflowStep<Tin, Tout> : IWorkflowStep
    {
        public Tout Run(Tin args, CancellationToken token = default);
    }

    public interface IWorkflowStepAsync : IWorkflowStep { }

    public interface IWorkflowStepAsync<Tout> : IWorkflowStepAsync
    {
        public Task<Tout> RunAsync(CancellationToken token = default);
    }

    public interface IWorkflowStepAsync<Tin, Tout> : IWorkflowStepAsync
    {
        public Task<Tout> RunAsync(Tin args, CancellationToken token = default);
    }
}
