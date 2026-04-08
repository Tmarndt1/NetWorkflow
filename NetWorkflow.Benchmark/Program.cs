using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NetWorkflow.Tests.Examples;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow.Benchmark
{
    public class WorkflowBenchmarks
    {
        private ConditionalWorkflow? _conditionalWorkflow = default;

        private ParallelWorkflow? _parallelWorkflow = default;

        [GlobalSetup]
        public void Setup()
        {
            // Initialize workflow (could use DI if necessary)
            _conditionalWorkflow = new ConditionalWorkflow();
            _parallelWorkflow = new ParallelWorkflow(false);
        }

        [Benchmark]
        public int RunConditionalWorkflow()
        {
            return _conditionalWorkflow?.Run();
        }

        [Benchmark]
        public int RunParallelWorkflow()
        {
            return _conditionalWorkflow?.Run();
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<WorkflowBenchmarks>();
        }
    }
}
