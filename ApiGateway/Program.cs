using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
var jwt = configuration.GetSection("JWT");

# region Jwt Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        //options.Authority = jwt["Authority"]; // e.g. authService http://..
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Token"]!)),
            RequireExpirationTime = false, //TODO: Set to true for production
        };
    });

#endregion

// CORS
#region CORS Configuration
var allowAll = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowAll, policy =>
    {
        policy.WithHeaders("Authorization", "Content-Type")
            .WithOrigins("http://127.0.0.1:5500")
            .AllowAnyMethod();
            //.WithMethods("GET", "POST", "OPTIONS");
    }); 
});
#endregion


#region ocelot Configuration to point to the endpoint API Gateway

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath).AddOcelot(); // single ocelot.json file in read-only mode
builder.Services.AddOcelot(builder.Configuration);

#endregion


#region TLS self-signed certificate
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.AllowAnyClientCertificate(); // Trust self-signed certificates
    });
});

#endregion

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    builder.Logging.AddConsole();
}

// Redirect HTTP to HTTPS
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Use Ocelot middleware
app.UseOcelot().Wait();

//app.Run();
await app.RunAsync();
