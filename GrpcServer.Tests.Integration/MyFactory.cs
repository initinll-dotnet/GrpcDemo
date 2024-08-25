using Grpc.Net.Client;

using HelloWorld;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace GrpcServer.Tests.Integration;

public class MyFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
        });

        builder.UseTestServer();
    }

    public HelloServiceDefinition.HelloServiceDefinitionClient CreateGrpcClient()
    {
        var httpClient = CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient,
        });

        var grpcClient = new HelloServiceDefinition.HelloServiceDefinitionClient(channel);

        return grpcClient;
    }
}
