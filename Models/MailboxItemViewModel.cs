namespace M365WebApp.Models;

public class MailboxItemViewModel
{
    public string Subject { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public DateTimeOffset? ReceivedDateTime { get; set; }
    public string OutlookDeepLink => $"https://outlook.office.com/mail/item/{System.Net.WebUtility.UrlEncode(MessageId)}";
    public string? MessageId { get; set; } // Useful for the "View" action later
}