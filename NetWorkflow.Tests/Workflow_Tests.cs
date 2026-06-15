using Microsoft.Extensions.DependencyInjection;
using NetWorkflow.Extensions;
using NetWorkflow.Tests.Examples;

namespace NetWorkflow.Tests
{
    public class Workflow_Tests
    {
        [Fact]
        public void HelloWorld_Success()
        {
            // Arrange
            var workflow = new HelloWorldWorkflow();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.True(result);
            Assert.True(result.TryGetOutput(out bool output));
            Assert.True(output);
            Assert.True(result.GetOutputOrThrow());
        }

        [Fact]
        public void HelloWorld_Ran_Twice_Success()
        {
            // Arrange
            var workflow = new HelloWorldWorkflow();

            // Act
            workflow.Run();

            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.True(result);
        }

        [Fact]
        public void Parallel_Success()
        {
            // Arrange
            var workflow = new ParallelWorkflow(false);

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.True(result);
        }

        [Fact]
        public async Task Parallel_Async_Success()
        {
            // Arrange
            var workflow = new ParallelWorkflow(false);

            // Act
            var result = await workflow.RunAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.True(result);
        }

        [Fact]
        public void Parallel_Cancel_Success()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var workflow = new ParallelWorkflow(tokenSource);

            // Act
            var result = workflow.Run(tokenSource.Token);

            // Assert
            Assert.Equal(WorkflowResultStatus.Canceled, result.Status);
            Assert.False(result.TryGetOutput(out bool output));
            Assert.False(output);
            Assert.Throws<OperationCanceledException>(() => result.GetOutputOrThrow());
        }

        [Fact]
        public void Parallel_Throw_Within_Task_Success()
        {
            // Arrange
            var workflow = new ParallelWorkflow(true);

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Faulted, result.Status);
            Assert.False(result.TryGetOutput(out bool output));
            Assert.False(output);
            Assert.Throws<InvalidOperationException>(() => result.GetOutputOrThrow());
        }

        [Fact]
        public void Conditional_Success()
        {
            // Arrange
            var workflow = new ConditionalWorkflow();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.Equal(1, result.Output);
        }

        [Fact]
        public void Conditional_Else_Success()
        {
            // Arrange
            var workflow = new ConditionalWorkflow("Unknown");

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Faulted, result.Status);
            Assert.Equal(0, result.Output);
        }


        [Fact]
        public void ConditionalParallel_Success()
        {
            // Arrange
            var workflow = new ConditionalParallelWorkflow();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.Equal(1, result.Output); // This test should return a favorable result
        }

        [Fact]
        public void ConditionalStop_Success()
        {
            // Arrange
            var workflow = new ConditionalStopWorkflow();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Null(result.Output); // Should be null if it passes
            Assert.Equal(WorkflowResultStatus.Canceled, result.Status);
        }

        [Fact]
        public void ConditionalThrow_Success()
        {
            // Arrange
            var workflow = new ConditionalThrowWorkflow();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Null(result.Output); // Should be null if it passes
            Assert.Equal(WorkflowResultStatus.Faulted, result.Status);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }

        [Fact]
        public void ConditionalThrow_WithOptions_Success()
        {
            // Arrange
            var workflow = new ConditionalThrowWorkflow(new WorkflowOptions()
            {
                RethrowExceptions = true
            });

            bool hit = false;

            // Act
            try
            {
                _ = workflow.Run();

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidOperationException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        [Fact]
        public void Catch_Exception_InStep_Success()
        {
            // Arrange
            var workflow = new ConditionalThrowWorkflow(true);

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Equal(WorkflowResultStatus.Faulted, result.Status);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }

        [Fact]
        public void AddWorkflow_Extensions_Success()
        {
            // Arrange
            var workflow = new ServiceCollection()
                .AddWorkflow<HelloWorldWorkflow, bool>(() => new HelloWorldWorkflow())
                .BuildServiceProvider()
                .GetRequiredService<HelloWorldWorkflow>();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.True(result);
        }

        [Fact]
        public void AddWorkflow_WithConstructorInjection_Extensions_Success()
        {
            // Arrange
            var workflow = new ServiceCollection()
                .AddSingleton<IDependencyMessageProvider, DependencyMessageProvider>()
                .AddWorkflowStep<DependencyMessageStep>()
                .AddWorkflow<DependencyWorkflow, string>()
                .BuildServiceProvider()
                .GetRequiredService<DependencyWorkflow>();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.Equal(DependencyMessageProvider.Message, result.Output);
        }

        [Fact]
        public async Task AddWorkflowRunner_Extensions_Success()
        {
            // Arrange
            var runner = new ServiceCollection()
                .AddSingleton<IDependencyMessageProvider, DependencyMessageProvider>()
                .AddWorkflowStep<DependencyMessageStep>()
                .AddWorkflow<DependencyWorkflow, string>()
                .BuildServiceProvider()
                .GetRequiredService<IWorkflowRunner<DependencyWorkflow, string>>();

            // Act
            var result = await runner.RunAsync();

            // Assert
            Assert.Equal(WorkflowResultStatus.Completed, result.Status);
            Assert.Equal(DependencyMessageProvider.Message, result.Output);
        }

        [Fact]
        public void Dispose_Success()
        {
            // Arrange
            var workflow = new ConditionalThrowWorkflow();

            bool hit = false;

            // Act
            try
            {
                workflow.Dispose();

                _= workflow.Run();

                hit = true;
            }
            catch (Exception ex)
            {
                Assert.IsType<ObjectDisposedException>(ex);
            }

            // Assert
            Assert.False(hit);
        }

        private interface IDependencyMessageProvider
        {
            string GetMessage();
        }

        private sealed class DependencyMessageProvider : IDependencyMessageProvider
        {
            public const string Message = "Injected workflow dependency";

            public string GetMessage()
            {
                return Message;
            }
        }

        private sealed class DependencyMessageStep : IWorkflowStep<string>
        {
            private readonly IDependencyMessageProvider _messageProvider;

            public DependencyMessageStep(IDependencyMessageProvider messageProvider)
            {
                _messageProvider = messageProvider;
            }

            public string Run(CancellationToken token = default)
            {
                return _messageProvider.GetMessage();
            }
        }

        private sealed class DependencyWorkflow : Workflow<string>
        {
            private readonly DependencyMessageStep _step;

            public DependencyWorkflow(DependencyMessageStep step)
            {
                _step = step;
            }

            public override IWorkflowBuilder<string> Build(IWorkflowBuilder builder)
            {
                return builder.StartWith(() => _step);
            }
        }
    }
}
