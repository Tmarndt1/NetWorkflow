using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using NetWorkflow.Extensions;
using NetWorkflow.Tests.Examples;
using Xunit;

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
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
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
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
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
            Assert.False(result.IsCompleted);
            Assert.False(result.IsFaulted);
            Assert.True(result.IsCanceled);
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
            Assert.False(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.True(result.IsFaulted);
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
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
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
            Assert.False(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.True(result.IsFaulted);
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
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
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
            Assert.True(result.IsCanceled);
            Assert.False(result.IsCompleted);
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
            Assert.False(result.IsCanceled);
            Assert.False(result.IsCompleted);
            Assert.True(result.IsFaulted);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }

        [Fact]
        public void ConditionalThrow_WithOptions_Success()
        {
            // Arrange
            var workflow = new ConditionalThrowWorkflow(new WorkflowOptions()
            {
                Rethrow = true
            });

            bool hit = false;

            // Act
            try
            {
                var result = workflow.Run();

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
            Assert.False(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }

        [Fact]
        public void AddWorkflow_Extensions_Success()
        {
            // Arrange
            var workflow = new ServiceCollection()
                .AddWorkflow(() => new HelloWorldWorkflow())
                .BuildServiceProvider()
                .GetRequiredService<HelloWorldWorkflow>();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
            Assert.True(result);
        }

        [Fact]
        public void Retry_WorkflowStep_Success()
        {
            // Arrange
            var workflow = new RetryWorkflow();

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Equal(2, RetryWorkflow.FirstStep.RanCount);
            Assert.False(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.IsType<WorkflowMaxRetryException>(result.Exception);
        }
    }
}