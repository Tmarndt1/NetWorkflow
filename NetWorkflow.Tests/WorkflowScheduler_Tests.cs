using NetWorkflow.Tests.Examples;

namespace NetWorkflow.Tests
{
    public class WorkflowScheduler_Tests
    {
        [Fact]
        public void Frequency_Success()
        {
            // Arrange
            int count = 0;

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow>()
                .Use(() => new HelloWorldWorkflow((stepName) =>
                {
                    count++;
                }))
                .Configure(options =>
                {
                    options.Frequency = TimeSpan.FromMilliseconds(200);
                });

            // Act
            scheduler.StartAsync();

            // Assert
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(450));

                scheduler.Stop();

                break;
            }

            Assert.Equal(4, count);
        }

        [Fact]
        public void AtTime_Success()
        {
            // Arrange
            int count = 0;

            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second + 1);

            var scheduler = new WorkflowScheduler<HelloWorldWorkflow>()
                .Use(() => new HelloWorldWorkflow((stepName) =>
                {
                    count++;
                }))
                .Configure(options =>
                {
                    options.AtTime = timeSpan;
                });

            // Act
            scheduler.StartAsync();

            // Assert
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(200));

                scheduler.Stop();

                break;
            }

            Assert.Equal(2, count);
        }
    }
}
