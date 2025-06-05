using ChatService;
using Microsoft.AspNetCore.Http.Connections;
using StackExchange.Redis;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
    builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
}

builder.Services.AddScoped<IChatProducer, ChatProducer>();

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
})
.AddStackExchangeRedis(builder.Configuration.GetConnectionString("redis-backplane")!, options =>
{
    options.Configuration.ChannelPrefix = RedisChannel.Literal("ChatApp");
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

app.MapHub<ChatHub>("/ChatHub");

app.UseCors();

app.Run();
