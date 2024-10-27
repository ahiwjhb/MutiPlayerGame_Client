#nullable enable
using System;

namespace Core.MVVM.Binding
{
    public interface ICommand
    {
        public void Execute();
    }

    public interface ICommand<TArgs>
    {
        public void Execute(TArgs? args);
    }
}
