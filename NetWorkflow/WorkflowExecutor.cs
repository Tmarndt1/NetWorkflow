using System.Linq.Expressions;
using System.Reflection;

namespace NetWorkflow
{
    public abstract class WorkflowExecutor<TContext>
    {
        protected readonly LambdaExpression _expression;

        protected readonly TContext _context;

        public WorkflowExecutor(LambdaExpression expression, TContext context)
        {
            _expression = expression;

            _context = context;
        }

        public abstract object? Run(object? args, CancellationToken token = default);
    }

    public class WorkflowExecutorEmpty<TContext> : WorkflowExecutor<TContext>
    {
        public WorkflowExecutorEmpty(TContext context) : base(() => true, context)
        {
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            return args;
        }
    }

    public class WorkflowExecutor<TContext, TResult> : WorkflowExecutor<TContext>
    {
        public WorkflowExecutor(LambdaExpression expression, TContext context) : base(expression, context) { }

        public override object? Run(object? args, CancellationToken token = default)
        {
            MethodInfo? executor = null;

            object? body = null;

            body = _expression.Parameters.Count == 1 ? _expression.Compile().DynamicInvoke(_context) : _expression.Compile().DynamicInvoke();

            if (body == null) throw new InvalidOperationException("WorkflowStep cannot be null");

            if (body is WorkflowStep step)
            {
                executor = step.GetType().GetMethod("Run");

                if (executor == null) throw new InvalidOperationException("Method not found");

                int count = executor.GetParameters().Length;

                if (count == 1)
                {
                    return (TResult?)executor.Invoke(body, new object[] { token });
                }
                else
                {
                    return (TResult?)executor.Invoke(body, new object?[] { args, token });
                }
            }
            else if (body is IEnumerable<WorkflowStepAsync> asyncSteps)
            {
                Task<TResult>?[] tasks = asyncSteps.Select(x =>
                {
                    executor = x.GetType().GetMethod("RunAsync");

                    if (executor == null) throw new InvalidOperationException("Internal error");

                    if (executor.GetParameters().Length > 1)
                    {
                        return ((Task<TResult>)executor.Invoke(x, new object?[] { args, token }));
                    }
                    else
                    {
                        return ((Task<TResult>)executor.Invoke(x, new object[] { token }));
                    }
                }).ToArray();

                Task.WaitAll(tasks);

                return tasks.Where(x => x != null).Select(x => x.Result).ToArray();
            }

            throw new InvalidOperationException("Internal error");
        }
    }

    public class WorkflowExecutorConditional<TContext, Tin> : WorkflowExecutor<TContext>
    {
        private List<Wrapper> _next = new List<Wrapper>();

        public WorkflowExecutorConditional(Expression<Func<Tin, bool>> expression, TContext context) : base(expression, context)
        {
            _next.Add(new Wrapper(expression));
        }

        public void Append(Expression<Func<Tin, bool>> expression)
        {
            if (_next == null) _next = new List<Wrapper>();

            _next.Add(new Wrapper(expression));
        }

        public void Append(WorkflowExecutor<TContext> executor)
        {
            _next.Last().Executor = executor;
        }

        public override object? Run(object? args, CancellationToken token = default)
        {
            var enumerator = _next.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (((Func<Tin, bool>)enumerator.Current.Expression.Compile()).Invoke((Tin)args))
                {
                    return enumerator.Current.Executor.Run(args, token);
                }
            }

            throw new Exception();
        }

        private class Wrapper
        {
            public LambdaExpression Expression { get; set; }

            public WorkflowExecutor<TContext>? Executor { get; set; }

            public Wrapper(LambdaExpression expression)
            {
                Expression = expression;
            }
        }
    }
}
