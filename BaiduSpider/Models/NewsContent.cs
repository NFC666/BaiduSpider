namespace BaiduSpider.Models;

public class NewsContent
{
    public required NewsItem Item { get; set; }
    public required List<string> Content { get; set; }
}