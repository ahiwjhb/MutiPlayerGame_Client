#nullable enable
using System.Collections.Generic;

namespace Core.MVVM.Binding
{
    public class BindingCollection<TSource, TBindingContext> where TSource : notnull where TBindingContext : class
    {
        private readonly IBindingFactory _bindingFactory;

        private readonly List<IBindingBuilder> _bindingBuilders;

        private readonly List<IBindingBuilder> _inverseBindingBuilders;

        private readonly List<IBinding> _bindings;

        private readonly TSource _source;

        private TBindingContext? _currentBindingContext;

        public BindingCollection(TSource source, IBindingFactory bindingFactory) {
            _source = source;
            _bindingBuilders = new List<IBindingBuilder>();
            _inverseBindingBuilders = new List<IBindingBuilder>();
            _bindings = new List<IBinding>();
            _bindingFactory = bindingFactory;
        }

        public IBuildObserverProcess<TSource, TBindingContext> CreateBindingBuilder() {
            var builder = new BindingBuilder<TSource, TBindingContext>(_bindingFactory);
            _bindingBuilders.Add(builder);
            return builder;
        }

        public IBuildObserverProcess<TBindingContext, TSource> CreateInverseBindingBuilder() {
            var builder = new BindingBuilder<TBindingContext, TSource>(_bindingFactory);
            _inverseBindingBuilders.Add(builder);
            return builder;
        }

        public void EnableBinding() {
            foreach(var binding in _bindings) {
                binding.Enable();
            }
        }

        public void DisableBinding() {
            foreach (var binding in _bindings) {
                binding.Disable();
            }
        }

        public void BindToBingContext(TBindingContext? bindingContext) {
            if (_currentBindingContext != null) {
                Clear();
            }

            if (bindingContext != null) {
                _currentBindingContext = bindingContext;
                CreateNewBinding(bindingContext);
                EnableBinding();
            }
        }

        private void CreateNewBinding(TBindingContext bindingContext) {
            foreach (var builder in _bindingBuilders) {
                var (forwardBinding, reverseBinding) = builder.Build(_source, bindingContext);
                if (forwardBinding != null) {
                    _bindings.Add(forwardBinding);
                }
                if (reverseBinding != null) {
                    _bindings.Add(reverseBinding);
                }
            }

            foreach (var builder in _inverseBindingBuilders) {
                var (forwardBinding, reverseBinding) = builder.Build(bindingContext, _source);
                if (forwardBinding != null) {
                    _bindings.Add(forwardBinding);
                }
                if (reverseBinding != null) {
                    _bindings.Add(reverseBinding);
                }
            }
        }

        public void Clear() {
            foreach (var binding in _bindings) {
                binding.Close();
            }
            _bindings.Clear();
            _currentBindingContext = null;
        }
    }
}
