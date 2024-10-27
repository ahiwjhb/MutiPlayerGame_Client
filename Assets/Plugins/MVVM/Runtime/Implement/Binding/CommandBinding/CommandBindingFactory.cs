#nullable enable
using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Core.MVVM.Utility;
using System.Diagnostics.CodeAnalysis;
using Core.MVVM.Extension;

namespace Core.MVVM.Binding
{
    public class CommandBindingFactory : IBindingFactory
    {
        public bool TryCreate(BindingBuildInfo bindingBuildInfo, [NotNullWhen(true)] out IBinding? binding) {
            BinderConfig observerConfig = bindingBuildInfo.SourceBinderConfig;
            BinderConfig targetConfig = bindingBuildInfo.DestinationBinderConfig;

            if (observerConfig.BinderType != BinderEnum.Command ||
                targetConfig.BinderType is not BinderEnum.UGUIEventNotifier and not BinderEnum.UIToolkitEventNotifier and not BinderEnum.ObservableValue) {

                binding = null;
                return false;
            }

            try {
                object command = CreateCommand(observerConfig, out var commandArgsType);
                object notifier = CreateEventNotifier(targetConfig, out var eventArgsType);

                System.Diagnostics.Contracts.Contract.Assert(eventArgsType.IsAssignableFrom(commandArgsType), "The value type of callback and event must be the same");

                object[] args = new object[] { command, notifier, bindingBuildInfo.BindModel };
                BindingFlags findFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
                Type bindingType = commandArgsType == typeof(void) ? typeof(CommandBinding)
                    : typeof(CommandBinding<,>).MakeGenericType(commandArgsType, eventArgsType);
                binding = (IBinding)Activator.CreateInstance(bindingType, findFlag, binder: null, args, culture: null);
            }
            catch (Exception e) {
                throw new IBindingFactory.CreateFailedException($"{nameof(CommandBindingFactory)} Create Binding {bindingBuildInfo} Failed " + e.Message, e);
            }

            return true;
        }

        private object CreateCommand(BinderConfig callbackConfig, out Type commandArgsType) {
            commandArgsType = callbackConfig.ValueType;
            Delegate callback = (Delegate)callbackConfig.GetBinderInstance();

            Type commandType = commandArgsType == typeof(void) ? typeof(ReallyCommand) :
                 typeof(ReallyCommand<>).MakeGenericType(commandArgsType);
            return Activator.CreateInstance(commandType, callback);
        }

        private object CreateEventNotifier(BinderConfig notifierConfig, out Type eventArgsType) {
            eventArgsType = notifierConfig.ValueType;
            object eventNotifier;
            if (notifierConfig.BinderType == BinderEnum.ObservableValue) {
                eventNotifier = notifierConfig.GetBinderInstance();
            }
            else if (notifierConfig.BinderType == BinderEnum.UGUIEventNotifier) {
                var unityEvent = (UnityEventBase)notifierConfig.GetBinderInstance();

                Type notifierType = eventArgsType == typeof(void) ? typeof(UnityEventToNotifierAdapter) :
                    typeof(UnityEventToNotifierAdapter<>).MakeGenericType(eventArgsType);
                eventNotifier = Activator.CreateInstance(notifierType, unityEvent);
            }
            else if (notifierConfig.BinderType == BinderEnum.UIToolkitEventNotifier) {
                var uiToolKitEventConfig = (UIToolKitEventNotifierConfig)notifierConfig;
                var uiToolKitEvent = (CallbackEventHandler)uiToolKitEventConfig.GetBinderInstance();

                Type notifierType = typeof(UIToolKiEventToNotifierAdapter<>).MakeGenericType(eventArgsType);
                eventNotifier = Activator.CreateInstance(notifierType, uiToolKitEvent, uiToolKitEventConfig.NotifyModel);
            }
            else {
                throw new NotSupportedException($"No solution to parse {nameof(IEventNotifier)} interface");
            }

            if(eventArgsType != typeof(void) && notifierConfig.ValueCaseWrapper != null) {
                Type oldEventArgsType = eventArgsType;
                eventArgsType = notifierConfig.ValueCaseWrapper.Method.ReturnType;

                Type notifierType = typeof(EventNotifierValueCaseDecorator<,>).MakeGenericType(oldEventArgsType, eventArgsType);
                eventNotifier = Activator.CreateInstance(notifierType, eventNotifier, notifierConfig.ValueCaseWrapper);
            }

            return eventNotifier;
        }
    }
}
