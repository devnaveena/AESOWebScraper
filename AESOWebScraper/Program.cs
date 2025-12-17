using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main()
    {
        string url = "http://ets.aeso.ca/ets_web/ip/Market/Reports/CSDReportServlet";
        string output = "WindGeneration.csv";

        var html = await new HttpClient().GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        string lastUpdate = ExtractLastUpdate(doc);
        if (string.IsNullOrEmpty(lastUpdate))
        {
            Console.WriteLine("Last Update not found!");
            return;
        }

        var rows = doc.DocumentNode.SelectNodes("//tr");
        if (rows == null)
        {
            Console.WriteLine("No rows found");
            return;
        }

        bool insideWindSection = false;
        var csv = new StringBuilder();
        csv.AppendLine("LastUpdate,Asset,AssetID,TotalNetGeneration");

        foreach (var row in rows)
        {
            string rowText = row.InnerText.Trim();

            // Detect WIND header
            if (rowText.Equals("WIND", StringComparison.OrdinalIgnoreCase))
            {
                insideWindSection = true;
                continue;
            }

            // Stop at next Headers
            if (insideWindSection &&
                (rowText.Equals("SOLAR") ||
                 rowText.Equals("HYDRO") ||
                 rowText.Equals("ENERGY STORAGE") ||
                 rowText.Equals("GAS")))
            {
                break;
            }

            if (!insideWindSection)
                continue;

            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count < 3)
                continue;

            string assetRaw = Clean(cells[0].InnerText);
            string tng = Clean(cells[2].InnerText);

            if (string.IsNullOrWhiteSpace(assetRaw))
                continue;

            // Extract Asset ID
            var match = Regex.Match(assetRaw, @"\((.*?)\)");
            string assetId = match.Success ? match.Groups[1].Value : "";
            string assetName = Regex.Replace(assetRaw, @"\s*\(.*?\)", "").Trim();

            if (string.IsNullOrWhiteSpace(tng))
                tng = "0";

            csv.AppendLine($"{lastUpdate},{assetName},{assetId},{tng}");
        }

        File.WriteAllText(output, csv.ToString());
        Console.WriteLine("Wind CSV generated successfully with LastUpdate");
    }

    static string ExtractLastUpdate(HtmlDocument doc)
    {
        var match = Regex.Match(
            doc.DocumentNode.InnerText,
            @"Last\s+Update\s*:\s*(\w+\s+\d{1,2},\s+\d{4}\s+\d{2}:\d{2})"
        );

        return match.Success ? match.Groups[1].Value : "";
    }

    static string Clean(string value)
    {
        return value
            .Replace("*", "")
            .Replace("^", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim();
    }
}
