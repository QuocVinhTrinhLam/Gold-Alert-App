using Microsoft.Extensions.Configuration;
using GoldAlertConsole;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Gold Alert Console App...");

        // Load Configuration
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var appConfig = config.GetSection("AppConfig").Get<AppConfig>() ?? new AppConfig();

        Console.WriteLine($"Monitoring Gold Price...");
        Console.WriteLine($"Range: {appConfig.MinPrice:N0} - {appConfig.MaxPrice:N0} VND");
        Console.WriteLine($"Check Interval: {appConfig.CheckIntervalSeconds} seconds");

        var goldService = new GoldPriceService();
        var emailService = new EmailService(appConfig);

        while (true)
        {
            try
            {
                Console.WriteLine($"Checking price at {DateTime.Now}...");
                var price = await goldService.GetSjcGoldSellPriceAsync();

                if (price.HasValue)
                {
                    Console.WriteLine($"Current SJC Price: {price.Value:N0} VND");

                    if (price.Value > appConfig.MaxPrice)
                    {
                        Console.WriteLine("Price limit exceeded (MAX)!");
                        await emailService.SendEmailAsync("Gold Alert: Price HIGH", 
                            $"Gold price is now {price.Value:N0} VND. It exceeded your max limit of {appConfig.MaxPrice:N0}.");
                    }
                    else if (price.Value < appConfig.MinPrice)
                    {
                        Console.WriteLine("Price limit exceeded (MIN)!");
                        await emailService.SendEmailAsync("Gold Alert: Price LOW", 
                            $"Gold price is now {price.Value:N0} VND. It dropped below your min limit of {appConfig.MinPrice:N0}.");
                    }
                    else
                    {
                        Console.WriteLine("Price is within range.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to fetch price.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            await Task.Delay(appConfig.CheckIntervalSeconds * 1000);
        }
    }
}
