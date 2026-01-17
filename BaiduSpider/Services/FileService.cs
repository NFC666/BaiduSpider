using System.Text.Json;
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
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // 美观的格式
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var text = JsonSerializer.Serialize(newsContents, options);

        var time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Content",
            $"newsContents_{time}.json"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await File.WriteAllTextAsync(path, text);
    }

    public async Task SaveNewsItem(List<HotWord> newsItems)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var text = JsonSerializer.Serialize(newsItems, options);
        await File.WriteAllTextAsync("./newsItems.json", text);
    }
}