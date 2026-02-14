using Microsoft.Graph.Models;

namespace M365WebApp.Models;

public class HomeViewModel
{
    // The "Header" data: User Profile
    public User? UserProfile { get; set; }

    // The "Array" of data: List of Emails
    public List<Message>? Messages { get; set; }
}