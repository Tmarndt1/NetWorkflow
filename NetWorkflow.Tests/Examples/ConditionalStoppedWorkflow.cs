
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalStoppedWorkflow : Workflow<object, object>
    {
        public ConditionalStoppedWorkflow(object context) : base(context)
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


        private class FirstStep : WorkflowStep<bool>
        {
            public override bool Run(CancellationToken token = default)
            {
                return false;
            }
        }
        private class ConditionalStep : WorkflowStep<bool, bool>
        {
            public override bool Run(bool args, CancellationToken token = default)
            {
                return args;
            }
        }

        private class FinalStep : WorkflowStep<object, object>
        {
            public override object? Run(object args, CancellationToken token = default)
            {
                return (bool)args ? new object() : null;
            }
        }
    }
}
