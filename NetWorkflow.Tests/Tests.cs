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
            Assert.Equal(1991, result.Data);
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
            Assert.Equal("Step3 ran", result.Data?.First());
            Assert.Equal("Step4 ran", result.Data?.ElementAt(1));
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
            Assert.Equal(-1, result.Data); // Since first step returns "failed" the final step returns -1
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
            Assert.Equal(1, result.Data); // This test should return a favorable result
        }

        [Fact]
        public void ConditionalStopped_Success()
        {
            // Arrange
            var workflow = new ConditionalStopWorkflow(new object());

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Null(result.Data); // Should be null if it passes
            Assert.True(result.IsCanceled);
            Assert.False(result.IsCompleted);
        }

        [Fact]
        public void ConditionalThrown_Success()
        {
            // Arrange
            var workflow = new ConditionalThrowWorkflow(new object());

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Null(result.Data); // Should be null if it passes
            Assert.False(result.IsCanceled);
            Assert.False(result.IsCompleted);
            Assert.True(result.IsFaulted);
            Assert.IsType<InvalidOperationException>(result.Exception);
        }
    }
}