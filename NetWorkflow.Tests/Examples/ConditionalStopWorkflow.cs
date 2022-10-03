
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalStopWorkflow : Workflow<object, object>
    {
        public ConditionalStopWorkflow(object context) : base(context)
        {
        }

        public override IWorkflowBuilder<object, object> Build(IWorkflowBuilder<object> builder) =>
            builder
                .StartWith(() => new FirstStep())
                    .If(x => x)
                        .Do(() => new ConditionalStep())
                    .ElseIf(x => !x)
                        .Stop()
                .EndIf()
                    .Then(() => new FinalStep());


        private class FirstStep : IWorkflowStep<bool>
        {
            public bool Run(CancellationToken token = default)
            {
                return false;
            }
        }
        private class ConditionalStep : IWorkflowStep<bool, bool>
        {
            public bool Run(bool args, CancellationToken token = default)
            {
                return args;
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
