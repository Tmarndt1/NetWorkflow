
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
            Task.Delay(75).ContinueWith(t =>
            {
                tokenSource.Cancel();
            });
        }

        public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new Step1())
                    .Parallel(() => new IWorkflowStepAsync<Guid, string>[]
                    {
                        new Step2(50, _throw),
                        new Step2(100, _throw)
                    })
                    .ThenAsync(() => new Step3())
                    .ThenAsync(() => new Step4());

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

                    return $"{nameof(Step2)} ran";
                }, token);
            }
        }

        private class Step3 : IWorkflowStepAsync<string[], string>
        {
            public Task<string> RunAsync(string[] args, CancellationToken token = default)
            {
                if (args.Length < 1) return Task.FromResult(string.Empty);

                return Task.FromResult($"{nameof(Step3)} ran");
            }
        }

        private class Step4 : IWorkflowStepAsync<string, bool>
        {
            public Task<bool> RunAsync(string args, CancellationToken token = default)
            {
                return Task.FromResult(args == $"{nameof(Step3)} ran");
            }
        }
    }
}
