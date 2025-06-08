using ChatClassLibrary;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MessageDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChatDb")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LobbyMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("lobby-message-queue", e =>
        {
            e.ConfigureConsumer<LobbyMessageConsumer>(context);
        });
    });
});

var app = builder.Build();

app.Run();
