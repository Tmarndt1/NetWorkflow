
namespace NetWorkflow.Tests.Examples
{
    public class RetryWorkflow : Workflow<int>
    {
        public override IWorkflowBuilder<int> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new FirstStep())
                    .If(x => x == "Success")
                        .Do(() => new ConditionalStep(1))
                    .Else()
                        .Retry(3)
                .EndIf()
                    .Then(() => new FinalStep());

        private class FirstStep : IWorkflowStep<string>
        {
            public string Run(CancellationToken token = default)
            {
                return "Failed";
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
