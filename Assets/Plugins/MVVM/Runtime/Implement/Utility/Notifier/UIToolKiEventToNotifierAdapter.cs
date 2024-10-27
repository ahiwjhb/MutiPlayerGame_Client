#nullable enable
using System;
using UnityEngine.UIElements;

namespace Core.MVVM.Utility
{
    internal class UIToolKiEventToNotifierAdapter<TEventArgs> : IEventNotifier<TEventArgs>, ICloseable where TEventArgs : EventBase<TEventArgs>, new()
    {
        private readonly CallbackEventHandler _subjectEvent;

        public event Action<TEventArgs>? OnEventHappened;
        
        public UIToolKiEventToNotifierAdapter(CallbackEventHandler uiToolKitEvent, TrickleDown notifyModel = TrickleDown.NoTrickleDown) {
            _subjectEvent = uiToolKitEvent;
            _subjectEvent.RegisterCallback<TEventArgs>(SyncEventHappened_OnUIToolKitEventTrigger, notifyModel);
        }

        public void Close() {
            _subjectEvent.UnregisterCallback<TEventArgs>(SyncEventHappened_OnUIToolKitEventTrigger);
        }

        private void SyncEventHappened_OnUIToolKitEventTrigger(TEventArgs eventArgs) {
            OnEventHappened?.Invoke(eventArgs);
        }

    }
}
