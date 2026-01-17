using System.Text;
using Newtonsoft.Json;
using Spider.Common.Helpers;
using Spider.Common.Models;
using Spider.Common.Models.ThePaper;
using Spider.Common.Services.Baidu;

namespace Spider.Common.Services.ThePaper;

public class PaperSpiderService : PlaywrightService
{
    private HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.thepaper.cn/")
    };

    private static readonly string Directory = "./ThePaper";
    private FileService _fileService = new(Directory);
    
    public async Task<List<News>> GetNewsByPageNumAsync(int pageNum, int pageSize = 20)
    {
        var news = new List<News>();
        var json = await GetNewsJsonAsync(pageNum, pageSize);
        var newsCovers = JsonConverterHelper
            .FromJsonListToList<NewsCover>(json);
        
        foreach (var newsCover in newsCovers)
        {
            var newsItem = new News
            {
                Cover = newsCover
            };
            newsItem.NewsContent = await GetNewsContentAsync(newsItem.Cover);
            news.Add(newsItem);
            await _fileService.SaveAllContentToJson(news,SpiderSource.ThePaper);
            Console.WriteLine($"新闻信息已经保存在当前目录下的{Directory}内");
        }
        return news;
    }
    
    private async Task<string> GetNewsJsonAsync(int pageNum,int pageSize = 20)
    {
        var url = "contentapi/nodeCont/getByChannelId";
        var requestBody = new
        {
            channelId = "25950",
            excludeContIds = new long[] { 32405966, 32406446, 32405967, 32406027, 32406021, 32406050, 32405953, 32405919, 32406429, 32407254 },
            // listRecommendIds = new long[] { 32406454, 32406429, 32406050, 32405919 },
            listRecommendIds = new long[] { 32406454, 32406429, 32406050, 32405919 },
            pageNum,
            pageSize,
            province = (string?)null,
        };
        var json = JsonConvert.SerializeObject(requestBody);
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        // return await response.Content.ReadAsStringAsync();
        var res = await response.Content.ReadAsStringAsync();
        // 格式化打印
        var formattedJson = JsonConvert.SerializeObject(
            JsonConvert.DeserializeObject(res),
            Formatting.Indented
        );
        return formattedJson;
        // return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> GetNewsContentAsync(NewsCover newsCover)
    {
        var url = newsCover.Link;
        await _page.GotoAsync(url);

        // 模糊选择 class 名含 "cententWrap"
        var contentDiv = await _page
            .QuerySelectorAsync("div[class*='cententWrap']");
        
        if (contentDiv == null)
        {
            return string.Empty;
        }
        // 获取 div 内的全部文字
        var textContent = await contentDiv.InnerTextAsync();
        return textContent;


    }
}