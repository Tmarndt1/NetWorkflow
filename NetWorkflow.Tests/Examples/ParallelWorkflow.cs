
using NetWorkflow.Interfaces;

namespace NetWorkflow.Tests.Examples
{
    public class ParallelWorkflow : Workflow<object, string[]>
    {
        private readonly bool _throw = false;

        public ParallelWorkflow(object context) : base(context)
        {
        }

        public ParallelWorkflow(object context, bool throwWithin) : base(context)
        {
            _throw = throwWithin;
        }

        public ParallelWorkflow(object context, CancellationTokenSource tokenSource) : base(context)
        {
            Task.Delay(75).ContinueWith(t =>
            {
                tokenSource.Cancel();
            });
        }

        public override IWorkflowBuilder<object, string[]> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new Step1())
                .Parallel(() => new IWorkflowStepAsync<Guid, string>[]
                {
                    new Step2(50, _throw),
                    new Step2(100, _throw)
                })
                .Parallel(() => new IWorkflowStepAsync<string[], string>[]
                {
                    new Step3(),
                    new Step4()
                });

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
                return Task.Run(() =>
                {
                    if (_throw) throw new InvalidOperationException("A test exception");
                    
                    Thread.Sleep(_delay);

                    return $"{nameof(Step2)} ran";
                }, token);
            }
        }

        private class Step3 : IWorkflowStepAsync<string[], string>
        {
            public Task<string> RunAsync(string[] args, CancellationToken token = default)
            {
                return Task.FromResult($"{nameof(Step3)} ran");
            }
        }

        private class Step4 : IWorkflowStepAsync<string[], string>
        {
            public Task<string> RunAsync(string[] args, CancellationToken token = default)
            {
                return Task.Run(() =>
                {
                    return $"{nameof(Step4)} ran";
                }, token);
            }
        }
    }
}
