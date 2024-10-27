#nullable enable
using Core.MVVM.UI;
using Core.MVVM.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Core.MVVM.Binding
{
    public interface IBindingBuilder
    {
        /// <summary>
        /// <para> 构建绑定 </para>
        /// <para> item1(正向绑定) 可能为空 </para>
        /// <para> item2(反向绑定) 可能为空 </para>
        /// </summary>
        public ValueTuple<IBinding?, IBinding?> Build(object source, object target);
    }

    public interface IBuildObserverProcess<TSource, TBindingContext> 
    {
        /// <summary>
        /// 集合(IList)变量
        /// </summary>
        public IBuildObservationTargetProcess<TBindingContext> Bind<TValue>(Expression<Func<TSource, IList<TValue>>> observer);

        /// <summary>
        /// 常规变量
        /// </summary>
        public IBuildObservationTargetProcess<TBindingContext> Bind<TValue>(Expression<Func<TSource, object>> observer);

        /// <summary>
        /// 可观察变量
        /// </summary>
        public IBuildObservationTargetProcess<TBindingContext> Bind<TValue>(Expression<Func<TSource, IObservableValue<TValue>>> observer);

        /// <summary>
        /// 不带参数的 事件处理函数
        /// </summary>
        public IBuildObservationTargetProcess<TBindingContext> Bind(Expression<Func<TSource, Action>> callback);

        /// <summary>
        /// 带一个参数的 事件处理函数
        /// </summary>
        public IBuildObservationTargetProcess<TBindingContext> Bind<TArgs>(Expression<Func<TSource, Action<TArgs>>> callback);
    }

    public interface IBuildObservationTargetProcess<TBindingContext>
    {
        /// <summary>
        /// 绑定到 可观测变量(IObservable)
        /// </summary>>
        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TBindingContext, IObservableValue<TValue>>> observationTarget);

        /// <summary>
        /// 绑定到 可观察集合(ObservableList)
        /// </summary>
        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TBindingContext, ObservableList<TValue>>> observationTarget);

        /// <summary>
        /// 绑定到 UGUI 变量
        /// </summary>
        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TBindingContext, TValue>> observationTarget, Expression<Func<TBindingContext, UnityEvent<TValue>>> updateTrigger);

        /// <summary>
        /// 绑定到UI Tool Kit 变量
        /// </summary>
        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TBindingContext, INotifyValueChanged<TValue>>> observationTarget);

        /// <summary>
        /// 绑定到 List 变量
        /// </summary>
        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TBindingContext, IList<TValue>>> observationTarget);

        /// <summary>
        /// 绑定到常规类型变量
        /// </summary>
        public IObservationTargetValueWrapProcess To<TValue>(Expression<Func<TBindingContext, object>> observationTarget);

        /// <summary>
        /// 绑定到 UGUI 事件
        /// </summary>
        public IObservationTargetValueWrapProcess To<TUnityEvent>(Expression<Func<TBindingContext, TUnityEvent>> notifier) where TUnityEvent : UnityEvent;

        /// <summary>
        /// 绑定到 UGUI 事件
        /// </summary>
        public IObservationTargetValueWrapProcess To<TEventArgs>(Expression<Func<TBindingContext, UnityEvent<TEventArgs>>> notifier);

        /// <summary>
        /// 绑定到 UI Tool kit 事件
        /// </summary>
        public IObservationTargetValueWrapProcess To<TEventArgs>(Expression<Func<TBindingContext, CallbackEventHandler>> notifier, TrickleDown notifyModel) where TEventArgs : EventBase<TEventArgs>, new();
    }


    public interface IObservationTargetValueWrapProcess : ISelectBindModelProcess
    {
        public ISelectBindModelProcess ValueWrap<TOldValue, TNewValue>(Func<TOldValue, TNewValue> valueWrapper);
    }

    public interface ISelectBindModelProcess 
    {
        public void OneTime();

        public void OneWay();

        public void TwoWay();
    }
}
