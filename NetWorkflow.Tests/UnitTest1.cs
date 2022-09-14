using NetWorkflow.Tests.Examples;

namespace NetWorkflow.Tests
{
    public class HelloWorld_Tests
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
            var workflow = new ParallelWorkflow(1991);

            // Act
            var results = workflow.Run();

            // Assert
            Assert.Equal($"{nameof(Step2)} ran", results?.First());
        }
    }
}