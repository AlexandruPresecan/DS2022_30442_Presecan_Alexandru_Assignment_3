using DS2022_30442_Presecan_Alexandru_Assignment_3;
using DS2022_30442_Presecan_Alexandru_Assignment_3.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Any, 443, listenOptions =>
    {
        listenOptions.UseHttps("/https/grpc.pfx", "pass");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<Connections<Message>>();
builder.Services.AddSingleton<Connections<IsTyping>>();
builder.Services.AddSingleton<Connections<Seen>>();

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
// Configure the HTTP request pipeline.
app.UseGrpcWeb();
app.MapGrpcService<ChatService>().EnableGrpcWeb();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
