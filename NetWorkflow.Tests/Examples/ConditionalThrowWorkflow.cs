using NetWorkflow.Interfaces;

namespace NetWorkflow.Tests.Examples
{
    public class ConditionalThrowWorkflow : Workflow<object, object>
    {
        public ConditionalThrowWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilder<object, object> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new FirstStep())
                .If(x => x)
                    .Throw(() => new InvalidOperationException("Invalid operation"))
                .ElseIf(x => !x)
                    .Stop()
                .EndIf()
                    .Then(() => new FinalStep());


        private class FirstStep : IWorkflowStep<bool>
        {
            public bool Run(CancellationToken token = default)
            {
                return true;
            }
        }

        private class FinalStep : IWorkflowStep<object, object>
        {
            public object Run(object args, CancellationToken token = default)
            {
                return (bool)args ? new object() : null;
            }
        }
    }
}
