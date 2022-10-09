
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalParallelWorkflow : Workflow<int>
    {
        public override IWorkflowBuilder<int> Build(IWorkflowBuilder builder) =>
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
                        .ThenAsync(() => new FinalStepAsync());


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

        private class FinalStepAsync : IWorkflowStepAsync<object, int>
        {
            public Task<int> RunAsync(object args, CancellationToken token = default)
            {
                return Task.FromResult((int)args);
            }
        }
    }
}
