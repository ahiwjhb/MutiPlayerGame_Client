#nullable enable
#nullable enable
using Microsoft.Extensions.DependencyInjection;

namespace Logger
{
    public static class LogServiceExtension {
        public static void AddLogService(this IServiceCollection services) {
            services.AddSingleton<ILogger, DebugLogger>();
        }
    }
}
