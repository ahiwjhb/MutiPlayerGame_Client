#nullable enable
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core.MVVM.Binding
{
    public class BindingBuildInfo
    {
        public BinderConfig SourceBinderConfig { get;}

        public BinderConfig DestinationBinderConfig { get;}

        public BindModel BindModel { get;}

        public LambdaExpression Observer => SourceBinderConfig.BinderVisitorExp;

        public LambdaExpression ObservationTarget => DestinationBinderConfig.BinderVisitorExp;

        public BindingBuildInfo(BinderConfig sourceConfig, BinderConfig destinationConfig, BindModel bindModel) {
            BindModel = bindModel;
            SourceBinderConfig = sourceConfig;
            DestinationBinderConfig = destinationConfig;
        }

        public override string ToString() {
            return $"{SourceBinderConfig.Context.GetType()} bind \"{Observer.Body}\" To {DestinationBinderConfig.Context.GetType()} \"{ObservationTarget.Body}\" {BindModel}";
        }
    }
}