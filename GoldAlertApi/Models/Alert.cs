namespace GoldAlertApi.Models;

public class Alert
{
    public int Id { get; set; }
    public int TargetPrice { get; set; }
    public string Condition { get; set; } = string.Empty; // "above" or "below"
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateAlertRequest
{
    public int TargetPrice { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
