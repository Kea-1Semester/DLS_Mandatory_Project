using Blazored.LocalStorage;
using DLS_Mandatory_Project.Components;
using DLS_Mandatory_Project.Components.Account;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using DLS_Mandatory_Project.Client.Clients;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddScoped<IChatClient, ChatClient>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomServerAuthStateProvider>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddJwtBearer("Bearer", options =>
            //{
            //    options.RequireHttpsMetadata = false;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,

            //        ValidIssuer = builder.Configuration["AuthGateway"],
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
