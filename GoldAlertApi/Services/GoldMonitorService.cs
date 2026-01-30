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
                                await emailService.SendEmailAsync(
                                    $"Gold Alert: Price {alert.Condition.ToUpper()} {alert.TargetPrice}", 
                                    $"Gold price is now {price.Value:N0} VND. Alert condition: {alert.Condition} {alert.TargetPrice:N0}.",
                                    _config.EmailTo
                                );
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
