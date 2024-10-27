#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.MVVM.Binding;
using Core.MVVM.Utility;

namespace Core.MVVM.UI
{
    public class ViewBindingCollection<TView, TViewModel> where TView : class, IView<TViewModel> where TViewModel : ViewModelBase
    {
        private static readonly IBindingFactory _bindingFactory;

        private readonly ObservableValue<TViewModel> _bindingContextChangedNotifier;

        private readonly BindingCollection<TView, TViewModel> _bindingCollection;

        static ViewBindingCollection() {
            IntegrateBindingFactory bindingFactory = new IntegrateBindingFactory();
            bindingFactory.AddFactory(new CommandBindingFactory(), 0);
            bindingFactory.AddFactory(new DataBindingFactory(), 1);
            _bindingFactory = bindingFactory;
        }

        public ViewBindingCollection(TView view) {
            _bindingCollection = new BindingCollection<TView, TViewModel>(view, _bindingFactory);
            _bindingContextChangedNotifier = new ObservableValue<TViewModel>();
            OnBindingContextChanged += (_, newModel) => _bindingCollection.BindToBingContext(newModel);
        }

        public TViewModel? BindingContext {
            get => _bindingContextChangedNotifier.Value;
            set => _bindingContextChangedNotifier.Value = value;
        }
    
        public event IObservableValue<TViewModel>.ValueChangedHandler OnBindingContextChanged {
            add => _bindingContextChangedNotifier.OnValueChanged += value;
            remove => _bindingContextChangedNotifier.OnValueChanged -= value;
        }

        /// <summary>
        /// 常规变量
        /// </summary>
        public IBuildObservationTargetProcess<TViewModel> BuildDataBind<TValue>(Expression<Func<TView, object>> observer) {
            return _bindingCollection.CreateBindingBuilder().Bind<TValue>(observer);
        }

        /// <summary>
        /// 常规变量
        /// </summary>
        public IBuildObservationTargetProcess<TViewModel> BuildDataBind<TValue>(Expression<Func<TView, IObservableValue<TValue>>> observer) {
            return _bindingCollection.CreateBindingBuilder().Bind<TValue>(observer);
        }

        /// <summary>
        ///  集合(IList) 变量
        /// </summary>
        public IBuildObservationTargetProcess<TViewModel> BuildDataBind<TValue>(Expression<Func<TView, IList<TValue>>> observer) {
            return _bindingCollection.CreateBindingBuilder().Bind(observer);
        }

        /// <summary>
        /// 不带参数的事件处理函数
        /// </summary>
        public IBuildObservationTargetProcess<TViewModel> BuildCommandBind(Expression<Func<TView, Action>> callback) {
            return _bindingCollection.CreateBindingBuilder().Bind(callback);
        }

        /// <summary>
        /// 带一个参数的事件处理函数
        /// </summary>
        public IBuildObservationTargetProcess<TViewModel> BuildCommandBind<TArgs>(Expression<Func<TView, Action<TArgs>>> callback) {
            return _bindingCollection.CreateBindingBuilder().Bind(callback);
        }

        /// <summary>
        /// 常规变量
        /// </summary>
        public IBuildObservationTargetProcess<TView> BuildInvDataBind<TValue>(Expression<Func<TViewModel, object>> observer) {
            return _bindingCollection.CreateInverseBindingBuilder().Bind<TValue>(observer);
        }

        /// <summary>
        /// 可观察变量
        /// </summary>
        public IBuildObservationTargetProcess<TView> BuildInvDataBind<TValue>(Expression<Func<TViewModel, IObservableValue<TValue>>> observer) {
            return _bindingCollection.CreateInverseBindingBuilder().Bind(observer);
        }

        /// <summary>
        /// 不带参数的事件处理函数
        /// </summary>
        public IBuildObservationTargetProcess<TView> BuildInvCommandBind(Expression<Func<TViewModel, Action>> callback) {
            return _bindingCollection.CreateInverseBindingBuilder().Bind(callback);
        }

        /// <summary>
        /// 带一个参数的事件处理函数
        /// </summary>
        public IBuildObservationTargetProcess<TView> BuildInvCommandBind<TArgs>(Expression<Func<TViewModel, Action<TArgs>>> callback) {
            return _bindingCollection.CreateInverseBindingBuilder().Bind(callback);
        }
    }
}
