#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Core.MVVM.UI
{
    public abstract class ViewModelBase
    {
        private readonly Dictionary<Type, EventHandler> _resigterCallbacks = new();

        public ViewModelBase? Parent { get; set; }

        public bool IsOpenInProcess { get; private set; }

        public bool IsCloseInProcess { get; private set; }

        public bool IsOpen { get; private set; }

        public virtual void OnStartOpen(IView view) {
            IsOpenInProcess = true;
        }

        public virtual void OnOpenFinished(IView view) {
            IsOpenInProcess = false;
            IsOpen = true;
        }

        public virtual void OnStartColse(IView view) {
            IsCloseInProcess = true;
        }

        public virtual void OnCloseFinished(IView view) {
            IsCloseInProcess = false;
            IsOpen = false;
        }

        public void AddListener<EventArgsType>(EventHandler handler) where EventArgsType : EventArgs {
            Type argsType = typeof(EventArgsType);
            if (_resigterCallbacks.ContainsKey(argsType)) {
                _resigterCallbacks[argsType] += handler;
            }
            else {
                _resigterCallbacks.Add(argsType, handler);
            }
        }

        public void RemoveListener<EventArgsType>(EventHandler handler) where EventArgsType : EventArgs {
            Type argsType = typeof(EventArgsType);
            var eventHandlers = _resigterCallbacks[argsType];
            eventHandlers -= handler;
            if (eventHandlers == null) {
                _resigterCallbacks.Remove(argsType);
            }
        }

        protected void SendEvent<EventArgsType>(EventArgsType args, TrickleDown trickleDown = TrickleDown.NoTrickleDown) where EventArgsType : EventArgs {
            var ascestors = trickleDown == TrickleDown.NoTrickleDown ? Ancestors<ViewModelBase>() : Ancestors<ViewModelBase>().Reverse();
            foreach (var viewModel in ascestors) {
                if (viewModel._resigterCallbacks.TryGetValue(typeof(EventArgsType), out var eventHandlers)) {
                    eventHandlers.Invoke(this, args);
                }
            }
        }

        protected IEnumerable<T> Ancestors<T>() where T : ViewModelBase {
            var viewModel = this;
            while (viewModel != null) {
                if (viewModel is T t) {
                    yield return t;
                }
                viewModel = viewModel.Parent;
            }
        }
    }
}
