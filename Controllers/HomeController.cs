using System.Diagnostics;
using Microsoft.AspNetCore.Authorization; // 1. Add this namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph; // The SDK
using Microsoft.Identity.Web;
using M365WebApp.Models;
using Microsoft.Graph.Models;

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
            
            var viewModel = new UserProfileViewModel
            {
               DisplayName = user?.DisplayName ?? "Unknown User",
               JobTitle = user?.JobTitle ?? "No Title Set",
               OfficeLocation = user?.OfficeLocation ?? "Not Assigned",
            };

            // 2. Fetch Last 10 Emails
            // .Select limits the "payload" size—just like minimizing DMA transfers
            var messages = await _graphServiceClient.Me.Messages
            .GetAsync(requestConfiguration => {
                requestConfiguration.QueryParameters.Top = 10;
                requestConfiguration.QueryParameters.Select = new string[] { "webLink", "subject", "from", "receivedDateTime" };
                requestConfiguration.QueryParameters.Orderby = new string[] { "receivedDateTime desc" };
            });

            var mailList = messages?.Value?.Select(m => new MailboxItemViewModel
            {
                Subject = m.Subject ?? "No Subject",
                Sender = m.From?.EmailAddress?.Name ?? "Unknown",
                ReceivedDateTime = m.ReceivedDateTime,
                WebLink = m.WebLink,
            }).ToList() ?? new List<MailboxItemViewModel>();            

            ViewData["DisplayName"] = user?.DisplayName;
            ViewData["JobTitle"] = user?.JobTitle;
            ViewData["OfficeLocation"] = user?.OfficeLocation;
            return View(mailList);
        }
        catch (ServiceException ex) when (ex.Message.Contains("Continuous Access Evaluation"))
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
        catch (Exception ex) 
        {
            // This is your "Kernel Panic" - something totally unexpected happened.

//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//            return Content($"Graph API Error: {ex.Message} -- StackTrace: {ex.StackTrace}");
//            var scopes = builder.Configuration.GetSection("MicrosoftGraph:Scopes").Value;
//            return Content($"Error: {ex.Message} | Attempted Scopes: {scopes}");

            if (ex.Message.Contains("IDW10502") || ex.InnerException?.Message.Contains("IDW10502") == true)
            {
                throw; 
            }

            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//            return Content($"Error: {ex.Message}");
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
