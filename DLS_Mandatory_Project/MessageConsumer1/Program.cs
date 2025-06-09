using MassTransit;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("rabbitmq", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
    cfg.ReceiveEndpoint("lobby-message-queue", e =>
    {
        e.Consumer<LobbyMessageConsumer>();
    });
});

Console.WriteLine("Starting consumer...");
bus.Start();

try
{
    Console.WriteLine("Receiving messages. Press a key to stop...");
    await Task.Run(() => Console.ReadKey(true));
}
finally
{
    Console.WriteLine("Stopping consumer...");
    bus.Stop();
}