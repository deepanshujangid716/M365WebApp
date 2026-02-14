namespace M365WebApp.Models;

public class UserProfileViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string OfficeLocation { get; set; } = string.Empty;
    public string? PhotoBase64 { get; set; } // For future: Profile Image
}