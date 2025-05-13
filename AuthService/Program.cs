using AuthService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var jwt = configuration.GetSection("JWT");

//var userService = configuration.GetSection("UserServiceClient");
var googleUserInfoUrl = configuration.GetSection("Authorization:Google");


// Get BASE_URL from environment variables from docker-compose
var userService = Environment.GetEnvironmentVariable("UserServiceClient__PROD_BASE_URL")
                       ?? builder.Configuration.GetSection("UserServiceClient")["DEV_BASE_URL"];

Console.WriteLine(userService);
// Add services to the container.

builder.Services.AddScoped<ILogInService, LogInService>();


//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
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
            RoleClaimType = "Role",
            NameClaimType = "Name",
        };
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = googleUserInfoUrl["ClientId"]!;
        options.ClientSecret = googleUserInfoUrl["ClientSecret"]!;
        options.SaveTokens = true;
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.CallbackPath = "/signin-google";
        options.AccessType = "offline";
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



/*
 * To be able to use Google authentication behind a reverse proxy i.e. LocalTunnel or ngrok we need to configure the ForwardedHeadersOptions
 * This is because we are using Docker, and we are calling on service level in docker-compose
 * So to be able to call redirect URL in google throw ngrok or localtunnel we need to configure the ForwardedHeadersOptions to let http requests pass through HTTPs
 *
 * And we need to use app.UseForwardedHeaders() in the pipeline before app.UseAuthentication()
 */

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


var app = builder.Build();

#region mini-API

//https://localhost:7046/api/account/login-google
app.MapGet("/api/account/login-google", async (HttpContext context) =>
    {
        // Initiates the Google authentication challenge
        var props = new AuthenticationProperties
        {
            RedirectUri = "/api/account/google-callback" // Must match registered redirect URI
        };
        await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
    })
    .AllowAnonymous();

app.MapGet("/api/account/google-callback", async (HttpContext context) =>
{
    var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (!result.Succeeded)
        return Results.Unauthorized();

    // Access the authentication properties i.e. tokens
    var authProperties = result.Properties;

    // Extract tokens from the authentication properties - we can use it fetch user data and refresh tokens
    //TODO: Create a method to refresh tokens and save them in the database
    var accessToken = authProperties.GetTokenValue("access_token")!;
    var idToken = authProperties.GetTokenValue("id_token");
    var refreshToken = authProperties.GetTokenValue("refresh_token");

    // Extract claims i.e. user information
    var claims = result.Principal.Claims.ToDictionary(c => c.Type, c => c.Value);
    // Generate JWT token
    var jwtToken = GenerateJwtToken(claims, accessToken);

    return Results.Ok(new
    {
        JwtToken = jwtToken,
    });
});


string GenerateJwtToken(Dictionary<string, string> claims, string accessToken)
{
    var expirationDate = DateTime.UtcNow.AddMinutes(2);

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Token"]!));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    var tokenClaims = claims.Select(c => new Claim(c.Key, c.Value)).ToList();
    tokenClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
    tokenClaims.Add(new Claim(ClaimTypes.Role, "User"));
    tokenClaims.Add(new Claim("Role", "User"));
    //tokenClaims.Add(new Claim("google_access_token", accessToken)); // optional 
    var token = new JwtSecurityToken(
        issuer: jwt["Issuer"],
        audience: jwt["Audience"],
        claims: tokenClaims,
        expires: DateTime.UtcNow.AddHours(1), 
        signingCredentials: creds


        );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

#endregion



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

app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
