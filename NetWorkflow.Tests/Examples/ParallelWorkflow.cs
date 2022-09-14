
namespace NetWorkflow.Tests.Examples
{
    public class ParallelWorkflow : Workflow<object, string[]>
    {
        public ParallelWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilder<object, string[]> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new Step1())
                .Parallel(() => new WorkflowStepAsync<Guid, string>[]
                {
                    new Step2(1000),
                    new Step2(2000)
                })
                .Parallel(() => new WorkflowStepAsync<string[], string>[]
                {
                    new Step3(),
                    new Step4()
                });
    }

    public class Step1 : WorkflowStep<Guid>
    {
        public override Guid Run(CancellationToken token = default)
        {
            return Guid.NewGuid();
        }
    }

    public class Step2 : WorkflowStepAsync<Guid, string>
    {
        private readonly int _delay;

        public Step2(int delay)
        {
            _delay = delay;
        }

        public override Task<string> RunAsync(Guid args, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(_delay);

                return $"{nameof(Step2)} ran";
            });
        }
    }

    public class Step3 : WorkflowStepAsync<string[], string>
    {
        public override Task<string> RunAsync(string[] args, CancellationToken token = default)
        {
            return Task.FromResult($"{nameof(Step3)} ran");
        }
    }

    public class Step4 : WorkflowStepAsync<string[], string>
    {
        public override Task<string> RunAsync(string[] args, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                return $"{nameof(Step4)} ran";
            });
        }
    }
}
