using DLS_Mandatory_Project.Client.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

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
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomClientAuthStateProvider>();
            builder.Services.AddScoped<IChatClient, ChatClient>();

            await builder.Build().RunAsync();
        }
    }
}
