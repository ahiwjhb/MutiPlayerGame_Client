#nullable enable
using Core.MVVM.Extension;
using Core.MVVM.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Core.MVVM.Binding
{
    internal partial class BindingBuilder<TSource, TDestination> : IBindingBuilder, 
        IBuildObserverProcess<TSource, TDestination>, IBuildObservationTargetProcess<TDestination>, ISelectBindModelProcess, IObservationTargetValueWrapProcess
    {
        private readonly IBindingFactory _bindingFactory;

        private BinderConfig? _sourceBinderConfig;

        private BinderConfig? _destinationBinderConfig;

        private BindModel? _bindModel = null;

        private bool _hasForwardBind = false;

        private bool _hasReserveBind = false;


        public BindingBuilder(IBindingFactory bindingFactory) {
            _bindingFactory = bindingFactory;
        }

        public IBuildObservationTargetProcess<TDestination> Bind<TValue>(Expression<Func<TSource, object>> observer) {
            _sourceBinderConfig = new BinderConfig(observer, BinderEnum.SimpleValue, typeof(TValue));
            return this;
        }

        public IBuildObservationTargetProcess<TDestination> Bind<TValue>(Expression<Func<TSource, IObservableValue<TValue>>> observer) {
            _sourceBinderConfig = new BinderConfig(observer, BinderEnum.ObservableValue, typeof(TValue));
            return this;
        }

        public IBuildObservationTargetProcess<TDestination> Bind<TValue>(Expression<Func<TSource, IList<TValue>>> observer) {
            _sourceBinderConfig = new BinderConfig(observer, BinderEnum.SimpleList, typeof(TValue));
            return this;
        }

        public IBuildObservationTargetProcess<TDestination> Bind(Expression<Func<TSource, Action>> callback) {
            _sourceBinderConfig = new BinderConfig(callback, BinderEnum.Command, typeof(void));
            return this;
        }

        public IBuildObservationTargetProcess<TDestination> Bind<TValue>(Expression<Func<TSource, Action<TValue>>> callback) {
            _sourceBinderConfig = new BinderConfig(callback, BinderEnum.Command, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TDestination, IObservableValue<TValue>>> observationTarget) {
            _destinationBinderConfig = new BinderConfig(observationTarget, BinderEnum.ObservableValue, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TDestination, ObservableList<TValue>>> observationTarget) {
            _destinationBinderConfig = new BinderConfig(observationTarget, BinderEnum.ObservableList, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TDestination, TValue>> observationTarget, Expression<Func<TDestination, UnityEvent<TValue>>> updateTrigger) {
            _destinationBinderConfig = new UGUIValueBinderConfig(observationTarget, updateTrigger, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TDestination, INotifyValueChanged<TValue>>> observationTarget) {
            _destinationBinderConfig = new BinderConfig(observationTarget, BinderEnum.UIToolkitValue, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TDestination, IList<TValue>>> observationTarget) {
            _destinationBinderConfig = new BinderConfig(observationTarget, BinderEnum.SimpleList, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TDestination, object>> observationTarget) {
            _destinationBinderConfig = new BinderConfig(observationTarget, BinderEnum.SimpleValue, typeof(TValue));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TUnityEvent>(Expression<Func<TDestination, TUnityEvent>> notifier) where TUnityEvent : UnityEvent{
            _destinationBinderConfig = new BinderConfig(notifier, BinderEnum.UGUIEventNotifier, typeof(void));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TEventArgs>(Expression<Func<TDestination, UnityEvent<TEventArgs>>> notifier) {
            _destinationBinderConfig = new BinderConfig(notifier, BinderEnum.UGUIEventNotifier, typeof(TEventArgs));
            return this;
        }

        public IObservationTargetValueWrapProcess To<TEventArgs>(Expression<Func<TDestination, CallbackEventHandler>> notifier, TrickleDown notifyModel) where TEventArgs : EventBase<TEventArgs>, new() {
            _destinationBinderConfig = new UIToolKitEventNotifierConfig(notifier, notifyModel, typeof(TEventArgs));
            return this;
        }

        public ISelectBindModelProcess ValueWrap<TOldValue, TNewValue>(Func<TOldValue, TNewValue> valueWrapper) {
            if (_destinationBinderConfig != null) {
                _destinationBinderConfig.ValueCaseWrapper = valueWrapper;
            }
            return this;
        }

        [MemberNotNull(nameof(_bindModel))]
        public void OneTime() {
            _hasForwardBind = true;
            _hasReserveBind = false;
            _bindModel = BindModel.OneTime;
        }

        [MemberNotNull(nameof(_bindModel))]
        public void OneWay() {
            _hasForwardBind = true;
            _hasReserveBind = false;
            _bindModel = BindModel.OneWay;
        }

        [MemberNotNull(nameof(_bindModel))]
        public void TwoWay() {
            _hasForwardBind = true;
            _hasReserveBind = true;
            _bindModel = BindModel.OneWay;
        }

        ValueTuple<IBinding?, IBinding?> IBindingBuilder.Build(object source, object destination) {
            ExceptionEx.ThrowIfNull<ArgumentException>(_sourceBinderConfig, "the observer is not configure");
            ExceptionEx.ThrowIfNull<ArgumentException>(_destinationBinderConfig, "the target is not configure");

            _sourceBinderConfig.Context = source;
            _destinationBinderConfig.Context = destination;
            var bindingTuple = new ValueTuple<IBinding?, IBinding?>();
            if(_bindModel == null) {
                OneWay();
            }
            if (_hasForwardBind) {
                bindingTuple.Item1 = CreateBinding(new BindingBuildInfo(_sourceBinderConfig, _destinationBinderConfig, _bindModel.Value));
            }
            if (_hasReserveBind) {
                bindingTuple.Item2 = CreateBinding(new BindingBuildInfo(_destinationBinderConfig, _sourceBinderConfig, _bindModel.Value));
            }

            return bindingTuple;
        }

        private IBinding? CreateBinding(BindingBuildInfo bindingBuildInfo) {
            IBinding? binding = null;
            try {
                _bindingFactory.TryCreate(bindingBuildInfo, out binding);
            }
            catch(IBindingFactory.CreateFailedException e) {
                Debug.LogError(e);
            }
            return binding;
        }
    }
}
