using ChatClassLibrary;
using MassTransit;
using MessageConsumer;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<MessageDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ChatDatabase"));
});

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
            e.AutoDelete = false;
            e.Durable = true;
            e.ConfigureConsumer<LobbyMessageConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
