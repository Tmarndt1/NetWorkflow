
using System;
using System.Threading;

namespace NetWorkflow.Tests.Examples
{
    public class HelloWorldWorkflow : Workflow<bool>
    {
        private const string _helloWorld = "HelloWorld";

        private readonly Action<string> _callback;

        public HelloWorldWorkflow() { }

        public HelloWorldWorkflow(Action<string> callback)
        {
            _callback = callback;
        }

        public override IWorkflowBuilder<bool> Build(IWorkflowBuilder builder) =>
            builder
                .StartWith(() => new HelloWorld(_callback))
                    .Then(() => new GoodbyeWorld(_callback));

        private class HelloWorld : IWorkflowStep<string>
        {
            private readonly Action<string> _callback;

            public HelloWorld() { }

            public HelloWorld(Action<string> callback)
            {
                _callback = callback;
            }

            public string Run(CancellationToken token = default)
            {
                _callback?.Invoke($"{nameof(HelloWorld)} ran");

                return _helloWorld;
            }
        }

        private class GoodbyeWorld : IWorkflowStep<string, bool>
        {
            private readonly Action<string> _callback;

            public GoodbyeWorld() { }

            public GoodbyeWorld(Action<string> callback)
            {
                _callback = callback;
            }

            public bool Run(string args, CancellationToken token = default)
            {
                _callback?.Invoke($"{nameof(GoodbyeWorld)} ran");

                return args == _helloWorld;
            }
        }
    }
}
