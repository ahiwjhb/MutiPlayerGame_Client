#nullable enable
using System;

namespace Core.MVVM.Utility
{
    public class EventNotifierValueCaseDecorator<TOldValue, TNewValue> : IEventNotifier<TNewValue>, ICloseable
    {
        private readonly IEventNotifier<TOldValue> _subjectNotifier;

        private readonly Func<TOldValue?, TNewValue?> _valueCaseDelegate;

        public event Action<TNewValue?>? OnEventHappened;

        public EventNotifierValueCaseDecorator(IEventNotifier<TOldValue> eventNotifier, Func<TOldValue?, TNewValue?> valueCaseDelegate) {
            _subjectNotifier = eventNotifier;
            _valueCaseDelegate = valueCaseDelegate;
            _subjectNotifier.OnEventHappened += SyncNotify_OnSubjectEventHappend;
        }

        public void Close() {
            _subjectNotifier.OnEventHappened -= SyncNotify_OnSubjectEventHappend;
        }

        private void SyncNotify_OnSubjectEventHappend(TOldValue? value) {
            OnEventHappened?.Invoke(_valueCaseDelegate(value));
        }
    }
}