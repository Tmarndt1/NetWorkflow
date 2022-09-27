using NetWorkflow.Interfaces;

namespace NetWorkflow.Tests.Examples
{
    public class ConditionalThrowWorkflow : Workflow<object, object>
    {
        private readonly bool _throwInStep = false;

        public ConditionalThrowWorkflow(object context) : base(context) { }

        public ConditionalThrowWorkflow(object context, WorkflowOptions options) : base(context, options) { }

        public ConditionalThrowWorkflow(object context, bool throwInStep) : base(context)
        {
            _throwInStep = throwInStep;
        }

        public override IWorkflowBuilder<object, object> Build(IWorkflowBuilder<object> builder) =>
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
