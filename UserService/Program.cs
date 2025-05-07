using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using UserService.DbContext;
using UserService.Service;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var jwt = configuration.GetSection("JWT");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        //TODO: options.Authority = jwt["Authority"]; // e.g. authService http://..

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



// Get the connection string from environment variables from docker-compose
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("UserDb");

//console log
Console.WriteLine($"Connection String: {connectionString}");

builder.Services.AddDbContext<ObjectDbContext>(options =>
    options.UseNpgsql(connectionString));

//builder.Services.AddDbContextPool<ObjectDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb")));

builder.Services.AddScoped<UserCommands>();
builder.Services.AddScoped<UserQueries>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });



    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Service API",
        Version = "v1",
        Description = "API documentation for the User Service"
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}
// Redirect HTTP to HTTPS
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Migrate the database on startup if needed.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ObjectDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
