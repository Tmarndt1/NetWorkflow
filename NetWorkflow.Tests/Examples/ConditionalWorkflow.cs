
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalWorkflow : Workflow<int>
    {
        private readonly string _message = "Success";

        public ConditionalWorkflow() { }

        public ConditionalWorkflow(string message)
        {
            _message = message;
        }

        public override IWorkflowBuilder<int> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new FirstStep(_message))
                    .If(x => x == "Success")
                        .Do(() => new ConditionalStep(1))
                    .ElseIf(x => x == "Failed")
                        .Do(() => new ConditionalStep(-1))
                    .Else()
                        .Throw(() => new InvalidOperationException("TEST"))
                .EndIf()
                    .Then(() => new FinalStep());


        private class FirstStep : IWorkflowStep<string>
        {
            private readonly string _message;

            internal FirstStep(string message)
            {
                _message = message;
            }

            public string Run(CancellationToken token = default)
            {
                return _message;
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
