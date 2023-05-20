using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetWorkflow
{
    internal class WorkflowStepAsyncExecutor<TIn, TOut> : IWorkflowExecutor<TIn, TOut>
    {
        private readonly LambdaExpression _expression;

        private bool _disposedValue;

        public WorkflowStepAsyncExecutor(LambdaExpression expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public TOut Run(TIn args, CancellationToken token = default)
        {
            if (_expression.Parameters.Count > 0)
                throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            var body = _expression.Compile().DynamicInvoke();

            if (body == null)
                throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
                throw new WorkflowStoppedException();

            if (!(body is IWorkflowStepAsync<TIn, TOut> stepAsync))
                throw new InvalidOperationException("Internal error");

            var executingMethod = stepAsync.GetType().GetMethod(nameof(IWorkflowStepAsync<TIn, TOut>.RunAsync));

            if (executingMethod == null)
                throw new InvalidOperationException("WorkflowStep executing method not found");

            var task = (Task<TOut>)executingMethod.Invoke(stepAsync, new object[] { args, token });

            task.Wait(token);

            return task.GetAwaiter().GetResult();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    (_expression as IDisposable)?.Dispose();
                }

                // Clean up unmanaged resources
                // (none in this case)

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
