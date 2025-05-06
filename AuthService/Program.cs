using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var jwt = configuration.GetSection("JWT");
//var userService = configuration.GetSection("UserServiceClient");

// Get BASE_URL from environment variables from docker-compose
var userService = Environment.GetEnvironmentVariable("UserServiceClient__PROD_BASE_URL")
                       ?? builder.Configuration.GetSection("UserServiceClient")["DEV_BASE_URL"];

Console.WriteLine(userService);
// Add services to the container.

builder.Services.AddScoped<ILogInService, LogInService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
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


builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Add HttpClient
builder.Services.AddHttpClient();



#region HttpClient for UserServiceClient
// Add HttpClient for UserServiceClient
builder.Services.AddHttpClient("UserServiceClient", client =>
{
    client.BaseAddress = new Uri(userService!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


// Add HttpClient for UserServiceClient
//builder.Services.AddHttpClient("UserServiceClient", client =>
//{
//    var baseUrl = builder.Environment.IsDevelopment()
//        ? userService["DEV_BASE_URL"]
//        : userService["PROD_BASE_URL"];
//    client.BaseAddress = new Uri(baseUrl!);
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});
#endregion

// Configure Swagger/OpenAPI
// add: NuGet\Install-Package Microsoft.AspNetCore.Grpc.Swagger -Version 0.9.0
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth Service API",
        Version = "v1",
        Description = "API documentation for the Auth Service"
    });

    // Configure Swagger to Use XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});





var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Redirect HTTP to HTTPS
// app.UseHttpsRedirection(); //for production use only

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
