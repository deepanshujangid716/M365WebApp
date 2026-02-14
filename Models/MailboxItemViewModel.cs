namespace M365WebApp.Models;

public class MailboxItemViewModel
{
    public string Subject { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public DateTimeOffset? ReceivedDateTime { get; set; }
    public string? WebLink { get; set; } // Useful for the "View" action later
}