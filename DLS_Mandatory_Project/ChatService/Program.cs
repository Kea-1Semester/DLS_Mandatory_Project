using ChatService;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
}

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
});

builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(option =>
    {
        option.AllowAnyOrigin();
        option.AllowAnyHeader();
        option.AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapHub<ChatHub>("/ChatHub", options =>
{
    options.Transports =
        HttpTransportType.WebSockets |
        HttpTransportType.ServerSentEvents;
});

app.UseCors();

app.Run();
