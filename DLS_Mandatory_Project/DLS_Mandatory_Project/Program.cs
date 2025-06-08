using Blazored.LocalStorage;
using DLS_Mandatory_Project.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using DLS_Mandatory_Project.Client.Clients;
using DLS_Mandatory_Project.Client.Services;
using DLS_Mandatory_Project.Client;
using DLS_Mandatory_Project.Components.Account;

namespace DLS_Mandatory_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR.Client", LogLevel.Debug);
                builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
            }

            // Add MudBlazor services
            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddAuthenticationStateSerialization();

            builder.Services.AddHttpClient("messageapi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            });
            builder.Services.AddScoped<IChatClient, ChatClient>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomServerAuthStateProvider>();

            builder.Services.AddAuthentication();
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //.AddJwtBearer(options =>
                //{
                //    options.TokenValidationParameters = new TokenValidationParameters
                //    {
                //        ValidateIssuer = true,
                //        ValidateAudience = true,
                //        ValidateLifetime = true,
                //        ValidateIssuerSigningKey = true,
                //        ValidIssuer = builder.Configuration.GetSection("JWT")["Issuer"],
                //        ValidAudience = builder.Configuration.GetSection("JWT")["Audience"],
                //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT")["Token"]!)),
                //    };
                //});

            builder.Services.AddAuthorization();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.Run();
        }
    }
}
