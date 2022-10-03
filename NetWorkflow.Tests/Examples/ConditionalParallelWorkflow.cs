
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
                    .Parallel(() => new IWorkflowStepAsync<string, string>[]
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


        private class Step2 : IWorkflowStepAsync<string, string>
        {
            private readonly int _delay;

            public Step2(int delay)
            {
                _delay = delay;
            }

            public Task<string> RunAsync(string args, CancellationToken token = default)
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(_delay);

                    return $"{nameof(Step2)} ran";
                }, token);
            }
        }

        private class FirstStep : IWorkflowStep<string>
        {
            public string Run(CancellationToken token = default)
            {
                return "Failed";
            }
        }
        private class FlattenStep : IWorkflowStep<string[], string>
        {
            public string Run(string[] args, CancellationToken token = default)
            {
                return string.Join(", ", args);
            }
        }

        private class ConditionalStep : IWorkflowStep<string, int>
        {
            private readonly int _result;

            public ConditionalStep(int result)
            {
                _result = result;
            }

            public int Run(string args, CancellationToken token = default)
            {
                return _result;
            }
        }

        private class FinalStep : IWorkflowStep<object, int>
        {
            public int Run(object args, CancellationToken token = default)
            {
                return (int)args;
            }
        }
    }
}
