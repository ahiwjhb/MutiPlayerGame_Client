#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.MVVM.Utility
{
    public sealed class ObservableList<T> : IList<T>, IList, IReadOnlyList<T>, IEnumerable<T>
    {
        public delegate void AddedHandler(int index, T item);
        public delegate void RemovedHandler(int index, T item);
        public delegate void ChangedHandler(int index, T oldValue, T newValue);

        public event AddedHandler? ItemBeAdded;
        public event RemovedHandler? ItemBeRemoved;
        public event ChangedHandler? ItemBeChanged;

        private readonly List<T> _subject;

        public ObservableList() {
            _subject = new List<T>();
        }

        public ObservableList(IEnumerable<T> ie) : this(){
            foreach (var item in ie) {
                Add(item);
            }
        }

        public bool IsFixedSize => ((IList)_subject).IsFixedSize;

        public bool IsReadOnly => ((IList)_subject).IsReadOnly;

        public int Count => ((ICollection)_subject).Count;

        public bool IsSynchronized => ((ICollection)_subject).IsSynchronized;

        public object SyncRoot => ((ICollection)_subject).SyncRoot;

        object IList.this[int index] {
            get => _subject[index]!;
            set {
                object old = ((IList)_subject)[index];
                ((IList)_subject)[index] = value;
                ItemBeChanged?.Invoke(index, (T)old, (T)value);
            }
        }

        public T this[int index] {
            get => _subject[index];
            set {
                T old = _subject[index];
                _subject[index] = value;
                ItemBeChanged?.Invoke(index, old, value);
            }
        }

        public int Add(object value) {
            int index = ((IList)_subject).Add(value);
            ItemBeAdded?.Invoke(index, (T)value);
            return index;
        }

        public void Add(T item) {
            int index = _subject.Count;
            _subject.Add(item);
            ItemBeAdded?.Invoke(index, item);
        }

        public void Clear() {
            for(int i = _subject.Count - 1; i >= 0; --i) {
                RemoveAt(i);
            }
        }

        public bool Contains(object value) {
            return ((IList)_subject).Contains(value);
        }

        public bool Contains(T item) {
            return _subject.Contains(item);
        }

        public void CopyTo(Array array, int index) {
            ((ICollection)_subject).CopyTo(array, index);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _subject.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_subject).GetEnumerator();
        }

        public int IndexOf(object value) {
            return ((IList)_subject).IndexOf(value);
        }

        public int IndexOf(T item) {
            return _subject.IndexOf(item);
        }

        public void Insert(int index, object value) {
            ((IList)_subject).Insert(index, value);
            ItemBeAdded?.Invoke(index, (T)value);
        }

        public void Insert(int index, T item) {
            _subject.Insert(index, item);
            ItemBeAdded?.Invoke(index, item);
        }

        public void Remove(object value) {
            int index = IndexOf(value);
            _subject.RemoveAt(index);
            ItemBeRemoved?.Invoke(index, (T)value);
        }

        public bool Remove(T item) {
            int index = IndexOf(item);
            if (index == -1) return false;

            _subject.RemoveAt(index);
            ItemBeRemoved?.Invoke(index, item);
            return true;
        }

        public void RemoveAt(int index) {
            T item = this[index];
            _subject.RemoveAt(index);
            ItemBeRemoved?.Invoke(index, item);
        }

        public IEnumerator<T> GetEnumerator() {
            return _subject.GetEnumerator();
        }
    }
}
