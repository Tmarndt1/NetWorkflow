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
            int age = workflow.Run();

            // Assert
            Assert.Equal(1991, age);
        }

        [Fact]
        public void Parallel_Success()
        {
            // Arrange
            var workflow = new ParallelWorkflow(new object());

            // Act
            string[]? results = workflow.Run();

            // Assert
            Assert.Equal("Step3 ran", results?.First());
            Assert.Equal("Step4 ran", results?.ElementAt(1));
        }

        [Fact]
        public void Conditional_Success()
        {
            // Arrange
            var workflow = new ConditionalWorkflow(new object());

            // Act
            int result = workflow.Run();

            // Assert
            Assert.Equal(-1, result); // Since first step returns "failed" the final step returns -1
        }


        [Fact]
        public void ConditionalParallel_Success()
        {
            // Arrange
            var workflow = new ConditionalParallelWorkflow(new object());

            // Act
            int result = workflow.Run();

            // Assert
            Assert.Equal(1, result); // This test should return a favorable result
        }

        [Fact]
        public void ConditionalStopped_Success()
        {
            // Arrange
            bool called = false;

            var workflow = new ConditionalStoppedWorkflow(new object())
                .OnStopped(() => called = true);

            // Act
            object? result = workflow.Run();

            // Assert
            Assert.Null(result); // Should be null if it passes
            Assert.True(workflow.Stopped);
            Assert.True(called);
        }
    }
}