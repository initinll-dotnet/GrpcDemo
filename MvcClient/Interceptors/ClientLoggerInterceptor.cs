using Grpc.Core.Interceptors;

namespace MvcClient.Interceptors;

public class ClientLoggerInterceptor : Interceptor
{
    private readonly ILogger<ClientLoggerInterceptor> logger;

    public ClientLoggerInterceptor(ILogger<ClientLoggerInterceptor> logger)
    {
        this.logger = logger;
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            var info = $"starting the client call of type: {context.Method.FullName}, {context.Method.Type}";
            logger.LogInformation(info);

            return continuation(request, context);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
