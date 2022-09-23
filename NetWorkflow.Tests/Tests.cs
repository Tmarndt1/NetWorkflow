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
            var age = workflow.Run();

            // Assert
            Assert.Equal(1991, age);
        }

        [Fact]
        public void Parallel_Success()
        {
            // Arrange
            var workflow = new ParallelWorkflow(new object());

            // Act
            var results = workflow.Run();

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
            var result = workflow.Run();

            // Assert
            Assert.Equal(-1, result); // Since first step returns "failed" the final step returns -1
        }


        [Fact]
        public void ConditionalParallel_Success()
        {
            // Arrange
            var workflow = new ConditionalParallelWorkflow(new object());

            // Act
            var result = workflow.Run();

            // Assert
            Assert.Equal(1, result); // This test should return a favorable result
        }
    }
}