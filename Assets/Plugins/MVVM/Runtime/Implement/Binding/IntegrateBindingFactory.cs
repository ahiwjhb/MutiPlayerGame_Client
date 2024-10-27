#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Core.MVVM.Binding
{
    public class IntegrateBindingFactory : IBindingFactory
    {
        private readonly List<ValueTuple<IBindingFactory, int>> _bindingFactories = new();

        public bool TryCreate(BindingBuildInfo bindingBuildInfo, [NotNullWhen(true)] out IBinding? binding) {
            foreach (var item in _bindingFactories) {
                var factory = item.Item1;
                if(factory.TryCreate(bindingBuildInfo, out binding)) {
                    return true;
                }
            }
            throw new IBindingFactory.CreateFailedException($"No match factory to create Binding {bindingBuildInfo}");
        }

        public void AddFactory(IBindingFactory bindingFactory, int usePriority = 0) {
            _bindingFactories.Add(new (bindingFactory, usePriority));
            _bindingFactories.Sort((lwh, rwh) => lwh.Item2.CompareTo(rwh.Item2));
        }

    }
}
