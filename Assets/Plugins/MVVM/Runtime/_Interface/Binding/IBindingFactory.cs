#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.MVVM.Binding
{
    public partial interface IBindingFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingInfo"></param>
        /// <returns>return the factory cant or cannot be created product by bindingdescription</returns>
        /// <exception cref="IBindingFactory.CreateFailedException">throw exception when factory can be created product but create failed</exception>
        public bool TryCreate(BindingBuildInfo bindingBuildInfo, [NotNullWhen(true)] out IBinding? binding);


        public class CreateFailedException : Exception
        {
            public CreateFailedException(string message, Exception inner) : base(message, inner) { }

            public CreateFailedException(string message) : base(message) { }

            public CreateFailedException() { }
        }
    }
}
