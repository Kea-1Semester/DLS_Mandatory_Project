using DLS_Mandatory_Project.Client.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace DLS_Mandatory_Project.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            if (builder.HostEnvironment.IsDevelopment())
            {
                builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
            }

            builder.Services.AddMudServices();

            builder.Services.AddSingleton<IChatClient, ChatClient>();

            await builder.Build().RunAsync();
        }
    }
}
