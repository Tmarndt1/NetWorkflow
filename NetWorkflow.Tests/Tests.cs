using NetWorkflow.Tests.Examples;

namespace NetWorkflow.Tests
{
    public class Tests
    {
        [Fact]
        public void HelloWorld_Success()
        {
            // Arrange
            var workflow = new HelloWorldWorkflow(1991);

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
            Assert.Equal(1991, result.Output);
            Assert.Equal(1991, (int)result);
        }

        [Fact]
        public void Parallel_Success()
        {
            // Arrange
            var workflow = new ParallelWorkflow(new object());

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
            Assert.Equal("Step3 ran", result.Output?.First());
            Assert.Equal("Step4 ran", result.Output?.ElementAt(1));
        }

        [Fact]
        public void Parallel_Cancel_Success()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var workflow = new ParallelWorkflow(new object(), tokenSource);

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
            var workflow = new ParallelWorkflow(new object(), true);

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
            var workflow = new ConditionalWorkflow(new object());

            // Act
            var result = workflow.Run();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.False(result.IsFaulted);
            Assert.Equal(-1, result.Output); // Since first step returns "failed" the final step returns -1
        }


        [Fact]
        public void ConditionalParallel_Success()
        {
            // Arrange
            var workflow = new ConditionalParallelWorkflow(new object());

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
            var workflow = new ConditionalStopWorkflow(new object());

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
            var workflow = new ConditionalThrowWorkflow(new object());

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
            var workflow = new ConditionalThrowWorkflow(new object(), new WorkflowOptions()
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
            var workflow = new ConditionalThrowWorkflow(new object(), true);

            // Act
            var result = workflow.Run();

            // Assert
            Assert.False(result.IsCompleted);
            Assert.False(result.IsCanceled);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }
    }
}