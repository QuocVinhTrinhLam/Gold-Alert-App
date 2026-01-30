using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace GoldAlertConsole;

public class GoldPriceService
{
    private readonly HttpClient _httpClient;
    private readonly string _sjcUrl = "http://www.sjc.com.vn/xml/tygiavang.xml";

    public GoldPriceService()
    {
        _httpClient = new HttpClient();
        // Add User-Agent to avoid being blocked
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<decimal?> GetSjcGoldSellPriceAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(_sjcUrl);
            var xdoc = XDocument.Parse(response);

            // Structure roughly:
            // <root>
            //   <ratelist>
            //     <city name="Hồ Chí Minh">
            //       <item buy="..." sell="..." type="Vàng SJC ..."/>
            
            // We look for type containing "SJC" under city "Hồ Chí Minh" usually, or just take the first SJC 1L.
            
            var item = xdoc.Descendants("item")
                .FirstOrDefault(x => x.Attribute("type")?.Value.Contains("SJC") == true);

            if (item != null)
            {
                var sellStr = item.Attribute("sell")?.Value;
                if (decimal.TryParse(sellStr, out var sellPrice))
                {
                    // SJC prices are often in unit 1000 VND or similar based on 'unit' attr.
                    // Usually SJC XML 'sell' is string like "74,000" or raw number.
                    // Let's assume it's raw or has commas/dots.
                    // Note: If unit is 1000 VND, we might need to multiply by 1000.
                    // Let's check unit attribute from root/ratelist if possible.
                    
                    var unit = xdoc.Descendants("ratelist").FirstOrDefault()?.Attribute("unit")?.Value;
                    
                    // Simple normalization
                    if (unit != null && unit.Contains("1000"))
                    {
                         sellPrice *= 1000;
                    }
                    else if (sellPrice < 100000) // Heuristic check if it's in thousands
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

        return null;
    }
}
