using Grpc.Core;

namespace GrpcServer.Tests.Unit.Helpers;

internal class TestServerCallContext : ServerCallContext
{
    private readonly Metadata _requestHeaders;
    private readonly CancellationToken _cancellationToken;
    private readonly Metadata _responseTrailers;
    private readonly AuthContext _authContext;
    private readonly Dictionary<object, object> _userState;

    public TestServerCallContext(Metadata requestHeaders = null, CancellationToken cancellationToken = default)
    {
        _requestHeaders = requestHeaders ?? new Metadata();
        _cancellationToken = cancellationToken;
        _responseTrailers = new Metadata();
        _authContext = new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>());
        _userState = new Dictionary<object, object>();
    }

    protected override string MethodCore => "TestMethod";
    protected override string HostCore => "testhost";
    protected override string PeerCore => "testpeer";
    protected override DateTime DeadlineCore => DateTime.MaxValue;
    protected override Metadata RequestHeadersCore => _requestHeaders;
    protected override CancellationToken CancellationTokenCore => _cancellationToken;
    protected override Metadata ResponseTrailersCore => _responseTrailers;
    protected override Status StatusCore { get; set; }
    protected override WriteOptions WriteOptionsCore { get; set; }
    protected override AuthContext AuthContextCore => _authContext;

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
    {
        throw new NotImplementedException();
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        return Task.CompletedTask;
    }

    protected override IDictionary<object, object> UserStateCore => _userState;
}
