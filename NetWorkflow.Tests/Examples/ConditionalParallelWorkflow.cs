
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalParallelWorkflow : Workflow<object, int>
    {
        public ConditionalParallelWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilder<object, int> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new FirstStep())
                .Parallel(() => new WorkflowStepAsync<string, string>[]
                {
                    new Step2(50),
                    new Step2(100),
                })
                .Then(() => new FlattenStep())
                .If(x => x.Contains(","))
                    .Do(() => new ConditionalStep(1))
                .ElseIf(x => x == "Failed")
                    .Do(() => new ConditionalStep(-1))
                .EndIf()
                    .Then(() => new FinalStep());


        private class Step2 : WorkflowStepAsync<string, string>
        {
            private readonly int _delay;

            public Step2(int delay)
            {
                _delay = delay;
            }

            public override Task<string> RunAsync(string args, CancellationToken token = default)
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(_delay);

                    return $"{nameof(Step2)} ran";
                }, token);
            }
        }

        private class FirstStep : WorkflowStep<string>
        {
            public override string Run(CancellationToken token = default)
            {
                return "Failed";
            }
        }
        private class FlattenStep : WorkflowStep<string[], string>
        {
            public override string Run(string[] args, CancellationToken token = default)
            {
                return string.Join(", ", args);
            }
        }

        private class ConditionalStep : WorkflowStep<string, int>
        {
            private readonly int _result;

            public ConditionalStep(int result)
            {
                _result = result;
            }

            public override int Run(string args, CancellationToken token = default)
            {
                return _result;
            }
        }

        private class FinalStep : WorkflowStep<object, int>
        {
            public override int Run(object args, CancellationToken token = default)
            {
                return (int)args;
            }
        }
    }
}
