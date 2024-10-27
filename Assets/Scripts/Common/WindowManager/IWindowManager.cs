using Core.MVVM.UI;
namespace Game.UI.WindowManager
{
    public enum RenderLayer
    {
        Default = 7,
        Layer01 = 6,
        Layer02 = 5,
        Layer03 = 4,
        Layer04 = 3,
        Layer05 = 2,
        Layer06 = 1,
        Highest = 0
    }

    public enum OpenModel 
    {
        Override,
        PopUp,
        ShutdownOtherWindow
    }

    public interface IWindowManager
    {
        public T GetWindow<T>() where T : IView;

        public T RemoveWindow<T>() where T : IView;

        public T OpenWindow<T>(float fadeTime = 0f, bool shutDownPreviousWindow = true) where T : IView;

        public void CloseWindow<T>(float fadeTime = 0f) where T : IView;

        public void CloseAllWindow(float fadeTime = 0f);

        public IView GetCurrentWindow();

        public void CloseCurrentWindow(float fadeTime = 0f);

        public void UnLoadCache();
    }
}
