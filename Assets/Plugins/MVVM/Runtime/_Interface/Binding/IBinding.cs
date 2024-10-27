#nullable enable
using Core.MVVM.Utility;

namespace Core.MVVM.Binding
{
    public interface IBinding : ICloseable
    {
        /// <summary>
        /// 开启绑定
        /// </summary>
        public void Enable();

        /// <summary>
        /// 关闭绑定
        /// </summary>
        public void Disable();
    }
}
