using System;
using System.Linq.Expressions;
using System.Threading;

namespace NetWorkflow
{
    internal class WorkflowStepExecutor<TIn, TOut> : IWorkflowExecutor<TIn, TOut>, IDisposable
    {
        private readonly LambdaExpression _expression;

        private bool _disposedValue;

        public WorkflowStepExecutor(LambdaExpression expression)
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

            if (!(body is IWorkflowStep step))
                throw new InvalidOperationException("Internal error");

            var executingMethod = step.GetType().GetMethod(nameof(IWorkflowStep<TOut>.Run));

            if (executingMethod == null)
                throw new InvalidOperationException("Workflow executing method not found");

            var parameters = executingMethod.GetParameters();

            if (parameters.Length == 1)
                return (TOut)executingMethod.Invoke(step, new object[] { token });
            else
                return (TOut)executingMethod.Invoke(step, new object[] { args, token });
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
