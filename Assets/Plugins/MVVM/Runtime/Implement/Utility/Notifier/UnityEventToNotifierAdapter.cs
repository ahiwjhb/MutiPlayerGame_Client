#nullable enable
using System;
using UnityEngine.Events;
namespace Core.MVVM.Utility
{
    internal class UnityEventToNotifierAdapter : IEventNotifier, ICloseable
    {
        private readonly UnityEvent _subjectEvent;

        public event Action? OnEventHappened;

        public UnityEventToNotifierAdapter(UnityEvent unityEvent) {
            _subjectEvent = unityEvent;
            _subjectEvent.AddListener(SyncEventHappened_OnUnityEventTrigger);
        }
        public void Close() {
            _subjectEvent.RemoveListener(SyncEventHappened_OnUnityEventTrigger);
        }

        private void SyncEventHappened_OnUnityEventTrigger() {
            OnEventHappened?.Invoke();
        }
    }
}

namespace Core.MVVM.Utility
{
    internal class UnityEventToNotifierAdapter<TEventArgs> : IEventNotifier<TEventArgs>, ICloseable
    {
        private readonly UnityEvent<TEventArgs> _subjectEvent;

        public event Action<TEventArgs>? OnEventHappened;

        public UnityEventToNotifierAdapter(UnityEvent<TEventArgs> unityEvent) {
            _subjectEvent = unityEvent;
            _subjectEvent.AddListener(SyncEventHappened_OnUnityEventTrigger);
        }

        public void Close() {
            _subjectEvent.RemoveListener(SyncEventHappened_OnUnityEventTrigger);
        }

        private void SyncEventHappened_OnUnityEventTrigger(TEventArgs args) {
            OnEventHappened?.Invoke(args);
        }
    }
}
