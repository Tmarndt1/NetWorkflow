
namespace NetWorkflow.Tests.Examples
{
    public class ParallelWorkflow : Workflow<object, string[]>
    {
        public ParallelWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilderNext<object, string[]> Build(IWorkflowBuilderInitial<object> builder) =>
            builder.StartWith(ctx => new StepOne())
                .Parallel(ctx => new WorkflowStepAsync<Guid, string>[]
                {
                    new Step2(1000),
                    new Step2(2000)
                })
                .Parallel(ctx => new WorkflowStepAsync<string[], string>[]
                {
                    new StepThree(),
                    new StepFour()
                });
    }

    public class StepOne : WorkflowStep<Guid>
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

    public class StepThree : WorkflowStepAsync<string[], string>
    {
        public override Task<string> RunAsync(string[] args, CancellationToken token = default)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }

    public class StepFour : WorkflowStepAsync<string[], string>
    {
        public override Task<string> RunAsync(string[] args, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                return $"{nameof(StepFour)} ran";
            });
        }
    }
}
