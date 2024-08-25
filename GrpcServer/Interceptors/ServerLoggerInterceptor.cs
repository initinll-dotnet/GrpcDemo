using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcServer.Interceptors;

public class ServerLoggerInterceptor : Interceptor
{
    private readonly ILogger<ServerLoggerInterceptor> logger;

    public ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger)
    {
        this.logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var info = $"starting the server call of type: {context.Method}, {context.Status}";
            logger.LogInformation(info);

            return await continuation(request, context);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
