#nullable enable
using Core.MVVM.Utility;
using System.Collections.Generic;

namespace Core.MVVM.Binding
{
    public sealed class ListDdataBinding<TObserver, TTarget> : IBinding where TTarget : TObserver
    {
        private readonly IList<TObserver> _observer;

        private readonly ObservableList<TTarget> _observableTarget;

        private readonly BindModel _bindModel;

        private bool _disposedValue;

        private bool _enable;


        public ListDdataBinding(IList<TObserver> observer, ObservableList<TTarget> observableTarget, BindModel bindModel) {
            _observer = observer;
            _observableTarget = observableTarget;
            _bindModel = bindModel;
        }

        public void Enable() {
            if (_enable) return;

            _observer.Clear();
            foreach(var item in _observableTarget) {
                _observer.Add(item);
            }

            if(_bindModel == BindModel.OneWay) {
                _observableTarget.ItemBeAdded += AddItem_ObservableListAdded;
                _observableTarget.ItemBeChanged += ChangeItem_ObservableListChanged;
                _observableTarget.ItemBeRemoved += RemoveItem_ObservableListRemoved;
            }

            _enable = true;
        }

        public void Disable() {
            if (!_enable) return;

            if(_bindModel == BindModel.OneWay) {
                _observableTarget.ItemBeAdded -= AddItem_ObservableListAdded;
                _observableTarget.ItemBeChanged -= ChangeItem_ObservableListChanged;
                _observableTarget.ItemBeRemoved -= RemoveItem_ObservableListRemoved;
            }

            _enable = false;
        }

        public void Close() {
            Disable();
        }

        private void AddItem_ObservableListAdded(int index, TTarget value) {
            _observer.Insert(index, value);
        }

        private void RemoveItem_ObservableListRemoved(int index, TTarget value) {
            _observer.RemoveAt(index);
        }

        private void ChangeItem_ObservableListChanged(int index, TTarget oldValue, TTarget newValue) {
            _observer[index] = newValue;
        }
    }
}