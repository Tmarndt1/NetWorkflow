
using NetWorkflow.Interfaces;

namespace NetWorkflow.Tests.Examples
{
    public class ConditionalWorkflow : Workflow<object, int>
    {
        public ConditionalWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilder<object, int> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new FirstStep())
                .If(x => x == "Success")
                    .Do(() => new ConditionalStep(1))
                .ElseIf(x => x == "Failed")
                    .Do(() => new ConditionalStep(-1))
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
