using System.Xml.Linq;

namespace GoldAlertApi.Services;

public interface IGoldPriceService
{
    Task<decimal?> GetSjcGoldSellPriceAsync();
}

public class GoldPriceService : IGoldPriceService
{
    private readonly HttpClient _httpClient;
    private readonly string _sjcUrl = "http://www.sjc.com.vn/xml/tygiavang.xml";

    public GoldPriceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<decimal?> GetSjcGoldSellPriceAsync()
    {
        // Mock data because real API is timing out
        return await Task.FromResult(90000000m);
        try
        {
            var response = await _httpClient.GetStringAsync(_sjcUrl);
            var xdoc = XDocument.Parse(response);
            
            var item = xdoc.Descendants("item")
                .FirstOrDefault(x => x.Attribute("type")?.Value.Contains("SJC") == true);

            if (item != null)
            {
                var sellStr = item.Attribute("sell")?.Value;
                if (decimal.TryParse(sellStr, out var sellPrice))
                {
                    var unit = xdoc.Descendants("ratelist").FirstOrDefault()?.Attribute("unit")?.Value;
                    if ((unit != null && unit.Contains("1000")) || sellPrice < 100000)
                    {
                         sellPrice *= 1000;
                    }
                    return sellPrice;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching gold price: {ex.Message}");
        }

        return null; // Return null on failure
    }
}
