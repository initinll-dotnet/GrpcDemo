using Auth;

using GrpcServer.Interceptors;
using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

using System.IO.Compression;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(option =>
{
    option.Interceptors.Add<ServerLoggerInterceptor>();
    option.ResponseCompressionAlgorithm = "gzip";
    option.ResponseCompressionLevel = CompressionLevel.SmallestSize;
}).AddJsonTranscoding();

// add authentication
builder
    .Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
        options.TokenValidationParameters = new TokenValidationParameters
        { 
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateActor = false,
            ValidateLifetime = false,
            IssuerSigningKey = JwtHelper.SecurityKey
        });

// add authorization
builder
    .Services
    .AddAuthorization(option =>
        option.AddPolicy(
            JwtBearerDefaults.AuthenticationScheme,
            policy =>
            {
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Name);
            }
        )
    );

builder
    .Services
    .AddGrpcHealthChecks()
    .AddCheck("my cool service", () => HealthCheckResult.Healthy(), new[] { "grpc", "live" });

builder.Services.AddGrpcReflection();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HelloService>();
app.MapGrpcHealthChecksService();
app.MapGrpcReflectionService();


app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program { }
