#nullable enable
using System;

namespace Core.MVVM.Utility
{
    public interface IEventNotifier
    {
        public event Action OnEventHappened;
    }

    public interface IEventNotifier<TEventArgs>
    {
        public event Action<TEventArgs?> OnEventHappened;
    }
}

