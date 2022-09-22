
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalWorkflow : Workflow<object, int>
    {
        public ConditionalWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilder<object, int> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new Step1())
                .If(x => false)
                    .Do(() => new Step2(1))
                .ElseIf(x => false)
                    .Do(() => new Step2(2))
                .Else()
                    .Do(() => new Step2(3))
                .EndIf()
                    .Then(() => new Step3());


        private class Step1 : WorkflowStep<string>
        {
            public override string Run(CancellationToken token = default)
            {
                return "Step 1 ran";
            }
        }
        private class Step2 : WorkflowStep<string, int>
        {
            private readonly int _repeater;

            public Step2(int repeater)
            {
                _repeater = repeater;
            }

            public override int Run(string args, CancellationToken token = default)
            {
                return _repeater;
            }
        }

        private class Step3 : WorkflowStep<object, int>
        {
            public override int Run(object args, CancellationToken token = default)
            {
                return (int)args;
            }
        }
    }
}
