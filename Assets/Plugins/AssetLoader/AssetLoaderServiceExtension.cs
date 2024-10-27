#nullable enable
using Microsoft.Extensions.DependencyInjection;

namespace Core.AssetLoader
{
    public static class AssetLoaderServiceExtension
    {
        public static void AddAssetLoadService(this IServiceCollection services) {
            services.AddSingleton<IAssetLoader, ResourceLoader.ResourceLoader>();
        }
    }
}
