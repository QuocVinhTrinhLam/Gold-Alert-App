namespace GoldAlertApi;

public class AppConfig
{
    public string EmailFrom { get; set; } = string.Empty;
    public string EmailPassword { get; set; } = string.Empty;
    public string EmailTo { get; set; } = string.Empty; // Default recipient if needed, though alerts might be generic
    public int CheckIntervalSeconds { get; set; } = 60;
}
