using MessageConsumer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ChatConsumer>();

var host = builder.Build();
host.Run();
