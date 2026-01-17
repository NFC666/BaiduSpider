// See https://aka.ms/new-console-template for more information

using Spider.Common.Models.ThePaper;
using Spider.Common.Services.ThePaper;

namespace ThePaperSpider;

public class ThePaperSpider
{
    private static PaperSpiderService _paperSpiderService = new();
    public static async Task Main(string[] args)
    {
        await _paperSpiderService.InitializeAsync();
        // var res = await _paperSpiderService.GetNewsByPageNumAsync(1);
        var res = await _paperSpiderService
            .GetNewsByPageNumAsync(NewsType.智库);
        foreach (var r in res)
        {
            Console.WriteLine(r.NewsContent);
        }
    }
}