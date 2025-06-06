using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

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

var host = builder.Build();
host.Run();
