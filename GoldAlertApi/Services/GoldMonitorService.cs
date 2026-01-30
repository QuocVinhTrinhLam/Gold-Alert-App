using Microsoft.Extensions.Options;
using GoldAlertApi.Models;

namespace GoldAlertApi.Services;

public class GoldMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppConfig _config;
    private readonly ILogger<GoldMonitorService> _logger;

    public GoldMonitorService(IServiceProvider serviceProvider, IOptions<AppConfig> config, ILogger<GoldMonitorService> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Gold Monitor Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var goldService = scope.ServiceProvider.GetRequiredService<IGoldPriceService>();
                    var alertStore = scope.ServiceProvider.GetRequiredService<IAlertStore>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var price = await goldService.GetSjcGoldSellPriceAsync();

                    if (price.HasValue)
                    {
                        var alerts = alertStore.GetAll();
                        foreach (var alert in alerts)
                        {
                            bool triggered = false;
                            if (alert.Condition == "above" && price.Value > alert.TargetPrice)
                            {
                                triggered = true;
                            }
                            else if (alert.Condition == "below" && price.Value < alert.TargetPrice)
                            {
                                triggered = true;
                            }

                            if (triggered)
                            {
                                _logger.LogInformation($"Alert Triggered! Price {price.Value} is {alert.Condition} {alert.TargetPrice}");
                                // For now, send to the default configured email as per Console App logic, 
                                // since Alert model doesn't have a specific user email yet.
                                // We could assume the user configured in AppConfig is the recipient.
                                string color = alert.Condition == "above" ? "#d32f2f" : "#2e7d32"; // Red for high, Green for low
                                string conditionText = alert.Condition == "above" ? "LỚN HƠN" : "NHỎ HƠN";
                                
                                string emailBody = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                    <style>
                                        body {{ font-family: 'Arial', sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
                                        .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 8px rgba(0,0,0,0.1); }}
                                        .header {{ background-color: #ffd700; color: #333; padding: 20px; text-align: center; }}
                                        .header h1 {{ margin: 0; font-size: 24px; }}
                                        .content {{ padding: 30px; text-align: center; }}
                                        .price {{ font-size: 48px; font-weight: bold; color: #b8860b; margin: 20px 0; }}
                                        .alert-box {{ background-color: #f9f9f9; border-left: 5px solid {color}; padding: 15px; margin: 20px 0; text-align: left; }}
                                        .footer {{ background-color: #333; color: #fff; padding: 10px; text-align: center; font-size: 12px; }}
                                        .btn {{ display: inline-block; padding: 10px 20px; background-color: #333; color: #ffd700; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>
                                            <h1>🔔 Cảnh Báo Giá Vàng SJC</h1>
                                        </div>
                                        <div class='content'>
                                            <p>Giá vàng SJC hiện tại:</p>
                                            <div class='price'>{price.Value:N0} đ</div>
                                            
                                            <div class='alert-box'>
                                                <h3>Chi tiết cảnh báo:</h3>
                                                <p><strong>Điều kiện:</strong> Giá {conditionText} mức mục tiêu</p>
                                                <p><strong>Mức mục tiêu:</strong> {alert.TargetPrice:N0} đ</p>
                                            </div>

                                            <p>Hãy kiểm tra ngay để không bỏ lỡ cơ hội!</p>
                                            <a href='http://localhost:5173' class='btn'>Truy cập Dashboard</a>
                                        </div>
                                        <div class='footer'>
                                            &copy; {DateTime.Now.Year} GoldAlert System. All rights reserved.
                                        </div>
                                    </div>
                                </body>
                                </html>";

                                try 
                                {
                                    await emailService.SendEmailAsync(
                                        $"🔥 CẢNH BÁO: Vàng SJC đạt mức {price.Value:N0}đ", 
                                        emailBody, 
                                        !string.IsNullOrEmpty(alert.Email) ? alert.Email : _config.EmailTo
                                    );
                                    _logger.LogInformation("Email sent successfully.");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to send email inside Monitor Service.");
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to fetch gold price.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GoldMonitorService");
            }

            await Task.Delay(TimeSpan.FromSeconds(_config.CheckIntervalSeconds), stoppingToken);
        }
    }
}
