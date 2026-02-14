using System.Diagnostics;
using Microsoft.AspNetCore.Authorization; // 1. Add this namespace
using Microsoft.AspNetCore.Mvc;
using M365WebApp.Models;
using Microsoft.Graph; // The SDK
using Microsoft.Identity.Web; // This one resolves [AuthorizeForScopes]

namespace M365WebApp.Controllers;

[Authorize]
public class HomeController : Controller
{

    private readonly GraphServiceClient _graphServiceClient;

    // The runtime sees this and "Injects" the client we configured in Program.cs
    public HomeController(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

[AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
public async Task<IActionResult> Index()
{
    try 
    {
        var user = await _graphServiceClient.Me.GetAsync();

        ViewData["DisplayName"] = user?.DisplayName ?? "Unknown";
        ViewData["JobTitle"] = user?.JobTitle ?? "No Title";
        ViewData["OfficeLocation"] = user?.OfficeLocation ?? "No Location";

        return View();
    }
    // This specific exception tells the app: "The user needs to interact with the UI"
    catch (MicrosoftIdentityWebChallengeUserException)
    {
        // Re-throw it so the [AuthorizeForScopes] attribute can catch it 
        // and redirect the user to the Microsoft Consent page.
        throw; 
    }
    catch (Exception ex)
    {
        return Content($"General Fault: {ex.Message}");
    }
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
