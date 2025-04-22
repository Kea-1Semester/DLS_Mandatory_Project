using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region ocelot Configuration to point to the endpoint API Gateway
// Add Ocelot
builder.Configuration.AddJsonFile("ocelot_localhost.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
#endregion


builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.AllowAnyClientCertificate(); // Trust self-signed certificates
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redirect HTTP to HTTPS
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Use Ocelot middleware
app.UseOcelot().Wait();

app.Run();
