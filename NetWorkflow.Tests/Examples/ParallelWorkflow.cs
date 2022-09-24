﻿
using NetWorkflow.Interfaces;

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
                .Parallel(() => new IWorkflowStepAsync<Guid, string>[]
                {
                    new Step2(1000),
                    new Step2(2000)
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

            public Step2(int delay)
            {
                _delay = delay;
            }

            public Task<string> RunAsync(Guid args, CancellationToken token = default)
            {
                return Task.Run(() =>
                {
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
