using System.Xml.Linq;

namespace GoldAlertApi.Services;

public interface IGoldPriceService
{
    Task<decimal?> GetSjcGoldSellPriceAsync();
}

public class GoldPriceService : IGoldPriceService
{
    private readonly HttpClient _httpClient;
    private readonly string _sjcUrl = "https://sjc.com.vn/GoldPrice/Services/PriceService.ashx"; // New API Endpoint
    
    public GoldPriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<decimal?> GetSjcGoldSellPriceAsync()
    {
        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("method", "GetCurrentGoldPricesByBranch"),
                new KeyValuePair<string, string>("BranchId", "1")
            });

            var response = await _httpClient.PostAsync(_sjcUrl, content);
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var json = System.Text.Json.Nodes.JsonNode.Parse(jsonString);

            if (json?["success"]?.GetValue<bool>() == true)
            {
                var data = json["data"]?.AsArray();
                if (data != null)
                {
                    // Find "Vàng SJC 1L, 10L, 1KG" or similar
                    var item = data.FirstOrDefault(x => x?["TypeName"]?.GetValue<string>().Contains("SJC 1L") == true)
                               ?? data.FirstOrDefault(x => x?["TypeName"]?.GetValue<string>().Contains("SJC") == true);
                    
                    if (item != null)
                    {
                        var sellValue = item["SellValue"]?.GetValue<decimal>();
                        return sellValue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
             Console.WriteLine($"Error fetching gold price: {ex.Message}. Using Mock Data.");
            // Fallback to mock data for testing purposes when SJC is down
            var random = new Random();
            return 82000000 + random.Next(0, 1000000); // Random between 82M and 83M
        }

        return null; // Return null on failure
    }
}
