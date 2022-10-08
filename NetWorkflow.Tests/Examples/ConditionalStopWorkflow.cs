
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalStopWorkflow : Workflow<object>
    {
        public override IWorkflowBuilder<object> Build(IWorkflowBuilder builder) =>
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
