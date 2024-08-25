using Auth;

using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;

using HelloWorld;

using static Grpc.Core.Metadata;

Console.WriteLine("Console client started .. ");
Console.WriteLine();

var option = new GrpcChannelOptions
{

};

using var channel = GrpcChannel.ForAddress("https://localhost:7058", option);

// health checks
var healthClient = new Health.HealthClient(channel);
var healthResult = await healthClient.CheckAsync(new HealthCheckRequest());

Console.WriteLine($"Health status: {healthResult.Status}");

var client = new HelloServiceDefinition.HelloServiceDefinitionClient(channel);

CallUnary(client);
//await StartClientStreaming(client);
//await StartServerStreaming(client);
//await StartBiDirectionalStream(client);


Console.ReadLine();

void CallUnary(HelloServiceDefinition.HelloServiceDefinitionClient client)
{
    try
    {
        var token = JwtHelper.GenerateJwtToken("MVC");

        // adding compression
        var metadata = new Metadata
        {
            { "grpc-accept-encoding", "gzip" },
            { "Authorization", $"Bearer {token}" }
        };

        var request = new Request
        {
            Content = "Hey"
        };

        var response = client.Unary(
            request: request,
            headers: metadata);

        Console.WriteLine($"CallUnary Result: {response.Message}");
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("Cancelled");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
    }
}

async Task StartClientStreaming(HelloServiceDefinition.HelloServiceDefinitionClient client)
{
    try
    {
        using var clientStream = client.ClientStream();

        for (var i = 0; i < 100; i++)
        {
            Thread.Sleep(500);
            var message = new Request
            {
                Content = $"Hello: {i + 1}"
            };

            Console.WriteLine($"Sending - {message.Content}");

            await clientStream.RequestStream.WriteAsync(message);
        }

        await clientStream.RequestStream.CompleteAsync();

        var response = await clientStream;

        Console.WriteLine($"Message from Server - {response.Message}");
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("Cancelled");
    }
    catch (Exception)
    {
        Console.WriteLine("Exception");
    }
}

async Task StartServerStreaming(HelloServiceDefinition.HelloServiceDefinitionClient client)
{
    try
    {
        var cancellationToken = new CancellationTokenSource();

        var metadata = new Metadata();
        metadata.Add(new Entry("my-first-key", "my-first-value"));
        metadata.Add(new Entry("my-second-key", "my-second-value"));

        var request = new Request
        {
            Content = "Hey"
        };

        using var serverStream = client
            .ServerStream(
                request: request,
                headers: metadata);

        var randomCheck = new Random().Next(1, 100);

        await foreach (var response in serverStream.ResponseStream.ReadAllAsync(cancellationToken.Token))
        {
            Console.WriteLine($"Message from Server - {response.Message}");

            if (response.Message.Contains(randomCheck.ToString()))
            {
                //cancellationToken.Cancel();
            }
        }

        var trailers = serverStream.GetTrailers();
        var trailer = trailers.Get("my-trailer-key");

        Console.WriteLine($"Trailer - Key: {trailer.Key} | Value: {trailer.Value}");
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("Cancelled");
    }
    catch (Exception)
    {
        Console.WriteLine("Exception");
    }
}

async Task StartBiDirectionalStream(HelloServiceDefinition.HelloServiceDefinitionClient client)
{
    try
    {
        var cancellationToken = new CancellationTokenSource();

        var request = new Request
        {
            Content = "Hey"
        };

        using var biStream = client.BiDirectionalStream();

        // send to server
        for (var i = 0; i < 100; i++)
        {
            Thread.Sleep(200);
            var message = new Request
            {
                Content = $"Hello: {i + 1}"
            };

            Console.WriteLine($"Sending - {message.Content}");

            await biStream.RequestStream.WriteAsync(message, cancellationToken.Token);
        }

        // response from server
        await foreach (var response in biStream.ResponseStream.ReadAllAsync(cancellationToken.Token))
        {
            Thread.Sleep(200);
            Console.WriteLine($"Message from Server - {response.Message}");
        }
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
    {
        Console.WriteLine("Cancelled");
    }
    catch (Exception)
    {
        Console.WriteLine("Exception");
    }
}