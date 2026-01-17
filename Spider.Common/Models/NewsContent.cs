namespace Spider.Common.Models;

public class NewsContent
{
    public required HotWord Item { get; set; }
    public required List<NewsItem>? Content { get; set; }
}