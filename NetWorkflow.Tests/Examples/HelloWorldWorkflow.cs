
namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<bool>
    {
        private const string _helloWorld = "HelloWorld";

        public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new HelloWorld())
                    .Then(() => new GoodByeWorld());

        private class HelloWorld : IWorkflowStep<string>
        {
            public string Run(CancellationToken token = default)
            {
                return _helloWorld;
            }
        }

        private class GoodByeWorld : IWorkflowStep<string, bool>
        {
            public bool Run(string args, CancellationToken token = default)
            {
                return args == _helloWorld;
            }
        }
    }
}
