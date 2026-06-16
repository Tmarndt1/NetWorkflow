
namespace NetWorkflow.Tests.Examples
{
    public class ParallelWorkflow : Workflow<bool>
    {
        private readonly bool _throw = false;

        public ParallelWorkflow(bool throwWithin)
        {
            _throw = throwWithin;
        }

        public ParallelWorkflow(CancellationTokenSource tokenSource)
        {
            Task.Delay(10).ContinueWith(t =>
            {
                tokenSource.Cancel();
            });
        }

        public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new Step1())
                    .Parallel(() => new IWorkflowStepAsync<Guid, string>[]
                    {
                        new Step2(100, _throw),
                        new Step2(50, _throw),
                    })
                    .Then(() => new Step3())
                        .Then(() => new Step4());

        private class Step1 : IWorkflowStep<Guid>
        {
            public Guid Run(CancellationToken token = default)
            {
                return Guid.NewGuid();
            }
        }

        private class Step2 : IWorkflowStepAsync<Guid, string>
        {
            private readonly int _delay;

            private readonly bool _throw = false;

            public Step2(int delay)
            {
                _delay = delay;
            }

            public Step2(int delay, bool throwWithin)
            {
                _delay = delay;

                _throw = throwWithin;
            }

            public Task<string> RunAsync(Guid args, CancellationToken token = default)
            {
                return Task.Delay(_delay, token).ContinueWith(t =>
                {
                    if (_throw) throw new InvalidOperationException("A test exception");

                    return $"{nameof(Step2)} ran with delay {_delay}";
                }, token);
            }
        }

        private class Step3 : IWorkflowStep<IEnumerable<string>, string>
        {
            public string Run(IEnumerable<string> args, CancellationToken token = default)
            {
                if (!args.Any()) return string.Empty;

                return $"{nameof(Step3)} ran";
            }
        }

        private class Step4 : IWorkflowStep<string, bool>
        {
            public bool Run(string args, CancellationToken token = default)
            {
                return args == $"{nameof(Step3)} ran";
            }
        }
    }
}
