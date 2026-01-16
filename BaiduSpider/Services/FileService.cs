using System.Text.Json;
using System.Text.RegularExpressions;
using BaiduSpider.Models;

namespace BaiduSpider.Services;

public class FileService
{
    public FileService()
    {
        Directory.CreateDirectory("./Content");
    }
    public async Task SaveAllContentToJson(List<NewsContent> newsContents)
    {
        var text = JsonSerializer.Serialize(newsContents);
        text = Regex.Unescape(text);

        var time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Content",
            $"newsContents_{time}.json"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await File.WriteAllTextAsync(path, text);
    }

    public async Task SaveNewsItem(List<NewsItem> newsItems)
    {
        var text = JsonSerializer.Serialize(newsItems);
        text = Regex.Unescape(text);
        await File.WriteAllTextAsync("./newsItems.json"
            , text);
    }
}