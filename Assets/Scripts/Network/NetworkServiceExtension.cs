using Microsoft.Extensions.DependencyInjection;
using Network.MessageChannel;
using MultiPlayerGame.Network;

public static class NetworkServiceExtension
{
    public static void AddNetworkService(this IServiceCollection services) {
        services.AddTransient<MessageChannel, MessageChannel>();
        services.AddSingleton<Client, Client>();
    }
}
