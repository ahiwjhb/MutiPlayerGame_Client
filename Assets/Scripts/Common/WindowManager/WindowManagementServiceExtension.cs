using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace Game.UI.WindowManager
{
    public static class WindowManagementServiceExtension
    {
        public static void AddWindowManagementService(this IServiceCollection services) {
            services.AddSingleton<WindowManager>(_ => WindowManager.Instance);
        }
    }
}
