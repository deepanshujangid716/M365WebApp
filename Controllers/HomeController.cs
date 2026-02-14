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
            // Attempting the "Read" operation
            var user = await _graphServiceClient.Me.GetAsync();

            ViewData["DisplayName"] = user?.DisplayName ?? "Unknown User";
            ViewData["JobTitle"] = user?.JobTitle ?? "No Title Set";
            ViewData["OfficeLocation"] = user?.OfficeLocation ?? "Not Assigned";

            return View();
        }
        catch (ServiceException ex) 
        {
            // 1. Handle "MsalUiRequiredException" - This happens if the token is 
            // stale and the user needs to physically log in again.
            if (ex.InnerException is Microsoft.Identity.Client.MsalUiRequiredException)
            {
                throw; // This triggers the [AuthorizeForScopes] to redirect the user to Microsoft Login
            }

            // 2. Handle Rate Limiting (HTTP 429) - Like a bus busy signal
            if (ex.ResponseStatusCode == 429)
            {
                ViewData["ErrorMessage"] = "The system is busy. Please wait a moment before retrying.";
                return View("Error");
            }

            // 3. Log other Graph-specific errors
            ViewData["ErrorMessage"] = $"M365 API Error: {ex.Message}";
            return View("Error");
        }
        catch (Exception)
        {
            // This is your "Kernel Panic" - something totally unexpected happened.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
