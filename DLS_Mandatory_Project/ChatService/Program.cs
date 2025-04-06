using ChatService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
//builder.Services.AddAuthentication("Bearer")
//    .AddBearerToken();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");

app.UseCors();

app.Run();
