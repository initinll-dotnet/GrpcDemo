using FluentAssertions;

using GrpcServer.Services;
using GrpcServer.Tests.Unit.Helpers;

using HelloWorld;

namespace GrpcServer.Tests.Unit;

public class HelloServiceTests
{
    private readonly IHelloService sut;

    public HelloServiceTests()
    {
        sut = new HelloService();
    }

    [Fact]
    public async Task Unary_ShouldReturn_an_Object()
    {
        // Arrange
        var context = new TestServerCallContext();
        var request = new Request
        {
            Content = "message"
        };

        var expectedResponse = new Response
        {
            Message = $"Data Received on Server : {request.Content}"
        };

        // Act
        var actualResponse = await sut.Unary(request, context);

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
}