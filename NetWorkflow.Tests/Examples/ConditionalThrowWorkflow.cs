
namespace NetWorkflow.Tests.Examples
{
    public class ConditionalThrowWorkflow : Workflow<object>
    {
        private readonly bool _throwInStep = false;

        public ConditionalThrowWorkflow() { }

        public ConditionalThrowWorkflow(WorkflowOptions options) : base(options) { }

        public ConditionalThrowWorkflow(bool throwInStep)
        {
            _throwInStep = throwInStep;
        }

        public override IWorkflowBuilder<object> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new FirstStep(_throwInStep))
                    .If(x => x)
                        .Throw(() => new InvalidOperationException("Invalid operation"))
                    .ElseIf(x => !x)
                        .Stop()
                .EndIf()
                    .Then(() => new FinalStep());


        private class FirstStep : IWorkflowStep<bool>
        {
            private readonly bool _throwInStep;

            public FirstStep(bool throwInStep)
            {
                _throwInStep = throwInStep;
            }

            public bool Run(CancellationToken token = default)
            {
                if (_throwInStep) throw new InvalidOperationException("Invalid operation");

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
