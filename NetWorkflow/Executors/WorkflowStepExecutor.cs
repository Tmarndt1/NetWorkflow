using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace NetWorkflow
{
    internal class WorkflowStepExecutor<TIn, TOut> : IWorkflowExecutor<TIn, TOut>
    {
        private LambdaExpression _expression;

        private bool _disposedValue;

        public WorkflowStepExecutor(LambdaExpression expression)
        {
            _expression = expression;
        }

        public TOut Run(TIn args, CancellationToken token = default)
        {
            MethodInfo executingMethod = null;

            object body = null;

            if (_expression.Parameters.Count > 0) throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            body = _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
            {
                throw new WorkflowStoppedException();
            }

            if (body is IWorkflowStep step)
            {
                executingMethod = step.GetType().GetMethod(nameof(IWorkflowStep<TOut>.Run));

                if (executingMethod == null) throw new InvalidOperationException("Workflow executing method not found");

                int count = executingMethod.GetParameters().Length;

                if (count == 1)
                {
                    return (TOut)executingMethod.Invoke(body, new object[] { token });
                }
                else
                {
                    return (TOut)executingMethod.Invoke(body, new object[] { args, token });
                }
            }

            throw new InvalidOperationException("Internal error");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _expression = null;
                }

                _disposedValue = true;
            }
        }

        ~WorkflowStepExecutor()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }
    }
}
