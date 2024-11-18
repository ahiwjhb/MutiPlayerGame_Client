using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Network.MessageChannel
{
    public delegate void EventHandler<TArgs>(MessageChannel sender, TArgs message);

    internal interface IEventNotifier 
    {
        public void Invoke(MessageChannel sender, object args);

        public void SafeInvokeToMultiThread(MessageChannel sender, object args);    
    }

    internal class EventNotifier<TArgs> : IEventNotifier
    {
        private readonly List<EventHandler<TArgs>> _listeners = new List<EventHandler<TArgs>>();

        private ILogger Logger { get; }

        public EventNotifier(ILogger logger) {
            Logger = logger;
        }

        public void AddListener(EventHandler<TArgs> callback) {
            _listeners.Add(callback);
        }

        public void RemoveListenner(EventHandler<TArgs> callback) {
            _listeners.Remove(callback);
        }

        public void Invoke(MessageChannel sender, object args) {
            for(int i = 0; i < _listeners.Count; ++i) {
                Invoke_Internal(_listeners[i], sender, args);
            }
        }

        public void SafeInvokeToMultiThread(MessageChannel sender, object args) {
            if (_listeners.Count < 5) {
                ThreadPool.QueueUserWorkItem(_ => Invoke(sender, args));
            }
            else {
                _listeners.AsParallel().ForAll(action => {
                    Invoke_Internal(action, sender, args);
                });
            }
        }

        private void Invoke_Internal(EventHandler<TArgs> handler, MessageChannel sender, object args) {
            try {
                handler?.Invoke(sender,(TArgs)args);
            }
            catch (Exception e) {
                Logger.Error("信息处理异常!", e);
            }
        }
    }
}
