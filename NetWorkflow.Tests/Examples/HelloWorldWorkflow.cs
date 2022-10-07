
namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<bool>
    {
        private const string _helloWorld = "HelloWorld";

        public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new HelloWorld())
                    .Then(() => new HelloWorld2());

        private class HelloWorld : IWorkflowStep<string>
        {
            public string Run(CancellationToken token = default)
            {
                return _helloWorld;
            }
        }

        private class HelloWorld2 : IWorkflowStep<string, bool>
        {
            public bool Run(string args, CancellationToken token = default)
            {
                return args == _helloWorld;
            }
        }
    }
}
