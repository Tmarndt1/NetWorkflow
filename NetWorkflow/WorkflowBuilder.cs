
using System.Reflection;

namespace NetWorkflow
{
    public class WorkflowBuilder<TContext> : IWorkflowBuilder<TContext>
    {
        private readonly TContext _context;

        private WorkflowBuilderNext? _builder;

        private WorkflowBuilderNext? _current;

        public WorkflowBuilder(TContext context)
        {
            _context = context;
        }

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Func<WorkflowStep<Tout>> func)
        {
            _builder = new WorkflowBuilderNext<TContext, Tout>(func, _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_builder;
        }

        public IWorkflowBuilderNext<TContext, Tout> StartWith<Tout>(Func<TContext, WorkflowStep<Tout>> func)
        {
            _builder = new WorkflowBuilderNext<TContext, Tout>(() => func.Invoke(_context), _context);

            return (IWorkflowBuilderNext<TContext, Tout>)_builder;
        }

        private bool MoveNext()
        {
            _current = _current == null ? _builder : _current.Next;

            return _current != null;
        }

        public object? Run(CancellationToken token = default)
        {
             if (!MoveNext()) throw new Exception("Fake");

            object? result = _current?.GetType()?.GetMethod("Run")?.Invoke(_current, new object[] { token });

            while (MoveNext())
            {
                if (_current == null) break;

                MethodInfo? method = _current.GetType().GetMethod("Run");

                if (method == null) break;

                var parameters = method.GetParameters().Length > 1 ? new object?[] { result, token } : new object[] { token };

                result = method.Invoke(_current, parameters);

                if (_current is WorkflowBuilderParallel && result is IEnumerable<Task> tasks)
                {
                    result = tasks.Select(x => x.GetType().GetProperty("Result")?.GetValue(x)).ToArray();
                }
            }

            return result;
        }
    }
}

