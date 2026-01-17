using System.Text.Json;
using Spider.Common.Models;
using Spider.Common.Models.Baidu;

namespace Spider.Common.Services.Baidu;

public class FileService
{
    private readonly string _directory;
    public FileService(string saveDirectory)
    {

        _directory = saveDirectory;
        Directory.CreateDirectory(_directory);

    }
    
    public async Task SaveAllContentToJson<T>(T newsContents, SpiderSource spiderSource)
    {
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // 美观的格式
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        
        var text = JsonSerializer.Serialize(newsContents, options);
        var time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        await File.WriteAllTextAsync(Path
            .Combine(_directory, $"{spiderSource}_{time}.json")
            , text);

    }


}