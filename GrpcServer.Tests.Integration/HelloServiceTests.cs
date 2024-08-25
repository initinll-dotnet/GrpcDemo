using FluentAssertions;

using HelloWorld;

namespace GrpcServer.Tests.Integration;

public class HelloServiceTests : IClassFixture<MyFactory<Program>>
{
    private readonly MyFactory<Program> factory;

    public HelloServiceTests(MyFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public void GetUnaryMessage()
    {
        // Arrange
        var client = factory.CreateGrpcClient();
        var request = new Request
        {
            Content = "message"
        };

        var expectedResponse = new Response
        {
            Message = $"Data Received on Server : {request.Content}"
        };

        // Act
        var actualResponse = client.Unary(request);

        // Assert
        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
}