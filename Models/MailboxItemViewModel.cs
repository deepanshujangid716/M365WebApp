namespace M365WebApp.Models;

public class MailboxItemViewModel
{
    public string Subject { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public DateTimeOffset? Received { get; set; }
    public string BodyPreview { get; set; } = string.Empty;
}