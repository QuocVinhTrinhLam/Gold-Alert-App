namespace GoldAlertConsole;

public class AppConfig
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string EmailFrom { get; set; } = string.Empty;
    public string EmailPassword { get; set; } = string.Empty;
    public string EmailTo { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; } = 3600;
}
