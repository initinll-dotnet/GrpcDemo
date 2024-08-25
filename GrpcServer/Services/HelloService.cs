using Auth;

using Grpc.Core;

using HelloWorld;

using Microsoft.AspNetCore.Authorization;

namespace GrpcServer.Services;

public class HelloService : HelloServiceDefinition.HelloServiceDefinitionBase, IHelloService
{
    public override Task<Response> Token(Request request, ServerCallContext context)
    {
        try
        {
            var token = JwtHelper.GenerateJwtToken(request.Content);

            var response = new Response
            {
                Message = token
            };

            return Task.FromResult(response);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    //[Authorize]
    public override Task<Response> Unary(Request request, ServerCallContext context)
    {
        try
        {
            // avoiding compression
            //context.WriteOptions = new WriteOptions(WriteFlags.NoCompress);

            var response = new Response
            {
                Message = $"Data Received on Server : {request.Content}"
            };

            return Task.FromResult(response);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public override async Task<Response> ClientStream(IAsyncStreamReader<Request> requestStream, ServerCallContext context)
    {
        try
        {
            var messageCount = 0;
            await foreach (var message in requestStream.ReadAllAsync().WithCancellation(context.CancellationToken))
            {
                messageCount++;
                Console.WriteLine($"Receiving - Message: {message}");
            }

            var response = new Response
            {
                Message = $"Data Stream - {messageCount} Received on Server"
            };

            return response;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public override async Task ServerStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        try
        {
            // receiving metadata from client request
            var metadataFirst = context.RequestHeaders.Get("my-first-key");
            var metadataSecond = context.RequestHeaders.Get("my-second-key");

            Console.WriteLine($"Metadata first - Key: {metadataFirst?.Key} | Value: {metadataFirst?.Value}");
            Console.WriteLine($"Metadata second - Key: {metadataSecond?.Key} | Value: {metadataSecond?.Value}");

            // sending trailers to client via context response trailers
            var trailer = new Metadata.Entry("my-trailer-key", "my-trailer-value");
            context.ResponseTrailers.Add(trailer);

            Console.WriteLine($"Received - Message: {request.Content}");

            for (int i = 0; i < 100; i++)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var response = new Response
                {
                    Message = i.ToString()
                };

                Console.WriteLine($"Sending - {response.Message}");
                await responseStream.WriteAsync(response, context.CancellationToken);
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public override async Task BiDirectionalStream(IAsyncStreamReader<Request> requestStream, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        try
        {
            await foreach (var message in requestStream.ReadAllAsync().WithCancellation(context.CancellationToken))
            {
                Thread.Sleep(200);
                Console.WriteLine($"Received - Message: {message.Content}");

                var response = new Response
                {
                    Message = message.Content
                };

                if (context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await responseStream.WriteAsync(response, context.CancellationToken);

                Console.WriteLine($"Send - Message: {message.Content}");
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
