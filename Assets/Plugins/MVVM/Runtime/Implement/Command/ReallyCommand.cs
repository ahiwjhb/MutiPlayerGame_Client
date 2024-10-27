#nullable enable
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Core.MVVM.Binding
{
    public class ReallyCommand : ICommand
    {
        private readonly Action _command;

        public ReallyCommand(Delegate @delegate) {
            MethodInfo method = @delegate.Method;
            Contract.Requires(method.ReturnType == typeof(void));
            Contract.Requires(method.GetParameters().Length == 0);

            _command = (Action)Delegate.CreateDelegate(typeof(Action), @delegate.Target, method);
        }

        public void Execute() {
            _command.Invoke();
        }
    }
}

namespace Core.MVVM.Binding
{
    public class ReallyCommand<TArgs> : ICommand<TArgs>
    {
        private readonly Action<TArgs?> _command;

        public ReallyCommand(Delegate @delegate) {
            MethodInfo method = @delegate.Method;
            Contract.Requires(method.ReturnType == typeof(void));
            Contract.Requires(method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(TArgs));

            _command = (Action<TArgs?>)Delegate.CreateDelegate(typeof(Action<TArgs>), @delegate.Target, method);
        }

        public void Execute(TArgs? args) {
            _command(args);
        }
    }
}
