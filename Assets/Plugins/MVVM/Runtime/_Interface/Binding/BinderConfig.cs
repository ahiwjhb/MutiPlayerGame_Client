#nullable enable
using Core.MVVM.Extension;
using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.MVVM.Binding
{
    public enum BinderEnum {
        SimpleValue,
        ObservableValue,
        UGUIValue,
        UIToolkitValue,
        ObservableList,
        SimpleList,

        Command,

        UGUIEventNotifier,
        UIToolkitEventNotifier
    }

    public class BinderConfig 
    {
        private object _context = null!;

        /// <summary>
        /// Binder 的上下文, 构建绑定时以Context作为根引用再根据相对路径获取 Binder、 Trigger等引用
        /// </summary>
        public object Context{
            get {
                ExceptionEx.ThrowIfNull<MissingMemberException>(_context, "the binder context is not configure");
                return _context;
            }
            set {
                ExceptionEx.ThrowIfNull<ArgumentException>(value, "the binder context is not null");
                _context = value;
            }
        }

        public BinderEnum BinderType { get; private set; }

        /// <summary>
        /// 发起绑定的对象
        /// </summary>
        public LambdaExpression BinderVisitorExp { get; private set; }

        /// <summary>
        ///  如果绑定者的类型派生自 IValueChangedNotifier<TValue>, UnityEvent<TValue>, ObservableList<TValue> 等 则 ValueType 为 TValue 的类型 否则为 typeof(void)
        ///  仅当 binder被绑定(观察)时 构建Binding有用
        /// </summary>
        public Type ValueType { get; private set; }

        /// <summary>
        /// 持有者类型
        /// </summary>
        public Type OwnerType => GetOwner().GetType();

        /// <summary>
        /// 对 Binder 的值进行二次包装的委托, 仅当 Binder 被绑定(观察)时有用
        /// </summary>
        public Delegate? ValueCaseWrapper { get; set; }


        public BinderConfig(LambdaExpression binderVisitorExp, BinderEnum binderType, Type valueTYpe) {
            BinderVisitorExp = binderVisitorExp;
            BinderType = binderType;
            ValueType = valueTYpe;
        }

        /// <summary>
        /// 根据 Context 获取 Binder 持有者的引用
        /// </summary>
        public object GetOwner() {
            ParameterExpression parameExp = BinderVisitorExp.Parameters[0];
            Expression expression = BinderVisitorExp.Body;
            while(expression is UnaryExpression unary) {
                expression = unary.Operand;
            }

            if (expression is MemberExpression memberExpression) {
                Delegate ownerGetter = Expression.Lambda(memberExpression.Expression, parameExp).Compile();
                return ownerGetter.DynamicInvoke(Context);
            }
            else {
                var methodCallExpression = (MethodCallExpression)expression;
                Delegate ownerGetter = Expression.Lambda(methodCallExpression.Object, parameExp).Compile();
                return ownerGetter.DynamicInvoke(Context);
            }
        }

        /// <summary>
        /// 根据 Context 获取 Binder 的实例对象
        /// </summary>
        public object GetBinderInstance() {
            object instance = BinderVisitorExp.Compile().DynamicInvoke(Context);
            ExceptionEx.ThrowIfNull<ArgumentNullException>(instance, "the binder instance is not be null");
            return instance;
        }
    }
}

namespace Core.MVVM.Binding
{
    public class UGUIValueBinderConfig : BinderConfig
    {
        public LambdaExpression ValueUpdatedTriggerVisitorExp { get; private set; }

        public UGUIValueBinderConfig(LambdaExpression binderVisitorExp, LambdaExpression valueUpdatedTrigger, Type valueTYpe) 
            : base(binderVisitorExp, BinderEnum.UGUIValue, valueTYpe) {
            ValueUpdatedTriggerVisitorExp = valueUpdatedTrigger;
        }
    }
}

namespace Core.MVVM.Binding
{
    public class UIToolKitEventNotifierConfig : BinderConfig
    {
        public TrickleDown NotifyModel { get; }

        public UIToolKitEventNotifierConfig(LambdaExpression binderVisitorExp, TrickleDown notifyModel, Type eventArgsType)
            : base(binderVisitorExp, BinderEnum.UIToolkitEventNotifier, eventArgsType) {
            NotifyModel = notifyModel;
        }
    }
}

namespace Core.MVVM.Binding
{
}

//namespace Core.MVVM.Binding
//{
//    public class ListValueBinderConfig : BinderConfig
//    {
//        public LambdaExpression ItemBeAddedCallbackVistorExp { get; }

//        public LambdaExpression ItemBeRemovedCallbackVistorExp { get; }

//        public ListValueBinderConfig(LambdaExpression binderVisitorExp, Type valueType, LambdaExpression itemBeAddedCallbackVistorExp = null, LambdaExpression itemBeRemovedCallbackVistorExp = null)
//            : base(binderVisitorExp, BinderEnum.SimpleList, valueType) {
//            ItemBeAddedCallbackVistorExp = itemBeAddedCallbackVistorExp;
//            ItemBeRemovedCallbackVistorExp = itemBeRemovedCallbackVistorExp;
//        }
//    }
//}
