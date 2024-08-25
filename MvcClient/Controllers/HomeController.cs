using HelloWorld;

using Microsoft.AspNetCore.Mvc;

using MvcClient.Models;

using System.Diagnostics;

namespace MvcClient.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HelloServiceDefinition.HelloServiceDefinitionClient client;

    public HomeController(ILogger<HomeController> logger, 
        HelloServiceDefinition.HelloServiceDefinitionClient client)
    {
        _logger = logger;
        this.client = client;
    }

    public IActionResult Index()
    {
        var request = new Request
        {
            Content = "Hello from Client"
        };

        var resposne = client.Unary(request);
        return View((object)resposne.Message);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
