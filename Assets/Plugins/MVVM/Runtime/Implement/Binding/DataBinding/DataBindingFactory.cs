#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Core.MVVM.Extension;
using Core.MVVM.Utility;

namespace Core.MVVM.Binding
{
    public partial class DataBindingFactory : IBindingFactory
    {
        public bool TryCreate(BindingBuildInfo bindingBuildInfo, [NotNullWhen(true)] out IBinding? binding) {
            BinderConfig observerConfig = bindingBuildInfo.SourceBinderConfig;
            BinderConfig targetConfig = bindingBuildInfo.DestinationBinderConfig;

            if (observerConfig.BinderType is not BinderEnum.SimpleValue and not BinderEnum.SimpleList and not BinderEnum.ObservableValue and not BinderEnum.UIToolkitValue and not BinderEnum.UGUIValue ||
                targetConfig.BinderType is not BinderEnum.SimpleValue and not BinderEnum.SimpleList and not BinderEnum.ObservableValue and not BinderEnum.UIToolkitValue and not BinderEnum.UGUIValue) {

                binding = null;
                return false;
            }

            try {
                if(observerConfig.BinderType is BinderEnum.SimpleList or BinderEnum.ObservableList && targetConfig.BinderType is BinderEnum.SimpleList or BinderEnum.ObservableList) {
                    binding = CreateListDataBinding(observerConfig, targetConfig, bindingBuildInfo.BindModel);
                }
                else {
                    binding = CreateSimpleDataBinding(observerConfig, targetConfig, bindingBuildInfo.BindModel);
                }
            }
            catch (Exception e) {
                throw new IBindingFactory.CreateFailedException($"{nameof(DataBindingFactory)} Create Binding {bindingBuildInfo} Failed", e);
            }

            return true;
        }

        private IBinding CreateListDataBinding(BinderConfig observerConfig, BinderConfig targetConfig, BindModel bindModel) {
            object observerList = observerConfig.GetBinderInstance();
            object obserableList = targetConfig.BinderType == BinderEnum.ObservableList ? targetConfig.GetBinderInstance():
                Activator.CreateInstance(typeof(ObservableList<>).MakeGenericType(targetConfig.ValueType), targetConfig.GetBinderInstance());

            ExceptionEx.ThrowIf<NotSupportedException>(targetConfig.ValueCaseWrapper != null, "The use of \"value wrapper\" for \"observableList\" is not supported");

            object[] args = new object[] { observerList, obserableList, bindModel };
            BindingFlags findFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            Type bindingType = typeof(ListDdataBinding<,>).MakeGenericType(observerConfig.ValueType, targetConfig.ValueType);
            return (IBinding)Activator.CreateInstance(bindingType, findFlag, binder: null, args, culture: null);
        }

        private IBinding CreateSimpleDataBinding(BinderConfig observerConfig, BinderConfig targetConfig, BindModel bindModel) {
            object observerWriter = CreateWriteable(observerConfig);
            object obervableTarget = CreateObservableValue(targetConfig, out Type valueTypeOfObserableTarget);

            ExceptionEx.ThrowIf<InvalidCastException>(observerConfig.ValueType.IsAssignableFrom(valueTypeOfObserableTarget) == false, "The value type of observation target must be divied from observer");

            object[] args = new object[] { observerWriter, obervableTarget, bindModel };
            BindingFlags findFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            Type bindingType = typeof(DataBinding<,>).MakeGenericType(observerConfig.ValueType, valueTypeOfObserableTarget);
            return  (IBinding)Activator.CreateInstance(bindingType, findFlag, binder: null, args, culture: null);
        }


        private IWriteable CreateWriteable(BinderConfig binderConfig) {
            BinderConfig observerConfig = binderConfig;
            object instance = observerConfig.GetBinderInstance();

            IWriteable? observerWriter;
            if (typeof(IWriteable).IsAssignableFrom(instance.GetType())) {
                observerWriter = (IWriteable)instance;
            }
            else if (observerConfig.BinderVisitorExp.Body is MemberExpression memberExp) {
                Type writerType = typeof(MemberInfoToWriteableAdapter<,>).MakeGenericType(observerConfig.OwnerType, observerConfig.ValueType);
                observerWriter = (IWriteable)Activator.CreateInstance(writerType, observerConfig.GetOwner(), memberExp.Member);
            }
            else if(observerConfig.BinderVisitorExp.Body is UnaryExpression unary) {
                memberExp = (MemberExpression)unary.Operand;
                Type writerType = typeof(MemberInfoToWriteableAdapter<,>).MakeGenericType(observerConfig.OwnerType, observerConfig.ValueType);
                observerWriter = (IWriteable)Activator.CreateInstance(writerType, observerConfig.GetOwner(), memberExp.Member);
            }
            else {
                throw new NotSupportedException($"For expression \"{observerConfig.BinderVisitorExp.Body}\" no define resolution straregy to create {nameof(IWriteable)} interface");
            }

            return observerWriter;
        }

        private object CreateObservableValue(BinderConfig binderConfig, out Type valueType) {
            valueType = binderConfig.ValueType;

            object observableValue;

            if (binderConfig.BinderType == BinderEnum.ObservableValue) {
                observableValue =  binderConfig.GetBinderInstance();
            }
            else if (binderConfig.BinderType == BinderEnum.UGUIValue) { //适配 UGUI
                var uguiValueConfig = (UGUIValueBinderConfig)binderConfig;

                object defaultValue = uguiValueConfig.GetBinderInstance();
                object valueUpdatedTrigger = uguiValueConfig.ValueUpdatedTriggerVisitorExp.Compile().DynamicInvoke(uguiValueConfig.Context);

                Type observableType = typeof(UnityEventToObservableAdapter<>).MakeGenericType(valueType);
                observableValue =  Activator.CreateInstance(observableType, defaultValue, valueUpdatedTrigger);
            }
            else if (binderConfig.BinderType == BinderEnum.UIToolkitValue) { //适配 UI ToolKit
                Type observableType = typeof(UIToolKitValueToObservableAdapter<>).MakeGenericType(valueType);
                observableValue =  Activator.CreateInstance(observableType, binderConfig.GetBinderInstance());
            }
            else {
                Type observableType = typeof(ObservableValue<>).MakeGenericType(binderConfig.ValueType);
                observableValue =  Activator.CreateInstance(observableType, binderConfig.GetBinderInstance());
            }

            if(binderConfig.ValueCaseWrapper != null) {
                Type oldValueType = valueType;
                valueType = binderConfig.ValueCaseWrapper.Method.ReturnType;
                Type observableType = typeof(ObservableValueCaseDecorator<,>).MakeGenericType(oldValueType, valueType);
                observableValue = Activator.CreateInstance(observableType, observableValue, binderConfig.ValueCaseWrapper);
            }

            return observableValue;
        }
    }
}
