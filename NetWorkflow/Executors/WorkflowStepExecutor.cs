using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NetWorkflow
{
    public class WorkflowStepExecutor<TIn, TOut> : IWorkflowExecutor<TIn, TOut>
    {
        private readonly LambdaExpression _expression;

        public WorkflowStepExecutor(LambdaExpression expression)
        {
            _expression = expression;
        }

        public TOut? Run(TIn? args, CancellationToken token = default)
        {
            MethodInfo? executor = null;

            object? body = null;

            if (_expression.Parameters.Count > 0) throw new InvalidOperationException("Parameter count within lambda cannot be greater than 0");

            body = _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("IWorkflowStep cannot be null");

            if (token.IsCancellationRequested)
            {
                throw new WorkflowStoppedException();
            }

            if (body is IWorkflowStep step)
            {
                executor = step.GetType().GetMethod(nameof(IWorkflowStep<TOut>.Run));

                if (executor == null) throw new InvalidOperationException("Run method not found");

                int count = executor.GetParameters().Length;

                if (count == 1)
                {
                    return (TOut?)executor.Invoke(body, new object[] { token });
                }
                else
                {
                    return (TOut?)executor.Invoke(body, new object?[] { args, token });
                }
            }

            throw new InvalidOperationException("Internal error");
        }
    }
}
