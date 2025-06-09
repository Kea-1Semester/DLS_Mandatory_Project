using DLS_Mandatory_Project.Client.Clients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using DLS_Mandatory_Project.Client.Services;

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

            //builder.Services.AddAuthenticationStateDeserialization();

            builder.Services.AddMudServices();
            //builder.Services.AddBlazoredLocalStorage();

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:8086") });

            //builder.Services.AddAuthorizationCore();
            //builder.Services.AddCascadingAuthenticationState();
            //builder.Services.AddScoped<AuthenticationStateProvider, CustomClientAuthStateProvider>();
            builder.Services.AddScoped<IChatClient, ChatClient>();
            //builder.Services.AddScoped<AuthService>();

            builder.Services.AddHttpClient("messageapi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            });

            await builder.Build().RunAsync();
        }
    }
}
