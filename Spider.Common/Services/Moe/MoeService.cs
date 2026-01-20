using Spider.Common.Models;
using Spider.Common.Models.Moe;
using News = Spider.Common.Models.Moe.News;
using NewsCover = Spider.Common.Models.Moe.NewsCover;

namespace Spider.Common.Services.Moe;

public class MoeService : PlaywrightService
{
    private static readonly string Directory = "./Moe";

    private readonly FileService _fileService = new(Directory);
    private readonly string? _baseUrl = "http://www.moe.gov.cn";


    public async Task<List<News>> GetNewsByNewsType(NewsType newsType)
    {
        var news = new List<News>();
        var newsCovers = await GetNewsCoverByNewsTypeAsync(newsType);
        foreach (var newsCover in newsCovers)
        {
            var newsItem = new News
            {
                Cover = newsCover
            };
            var content = await GetNewsContentAsync(newsCover);
            newsItem.Content = content;

            news.Add(newsItem);
            await _fileService.SaveAllContentToJson(news, SpiderSource.Moe);
            Console.WriteLine($"已保存到目录：{Directory}");
        }

        return news;
    }


    private async Task<List<NewsCover>> GetNewsCoverByNewsTypeAsync(NewsType newsType)
    {
        var news = new List<NewsCover>();
        var url = GetUrlFromType(newsType);
        await Page.GotoAsync(Path.Combine(_baseUrl, url));
        var ul = await Page.QuerySelectorAsync("ul[id='list']");
        if (ul == null)
        {
            return news;
        }

        var selectors = await ul
            .QuerySelectorAllAsync("li");
        foreach (var selector in selectors)
        {
            var cover = new NewsCover();

            var titleSelector = await selector
                .QuerySelectorAsync("a");
            if (titleSelector == null)
            {
                continue;
            }

            cover.Title = await titleSelector.InnerTextAsync();
            cover.Link = await titleSelector.GetAttributeAsync("href");

            var timeSelector = await selector
                .QuerySelectorAsync("span");
            if (timeSelector == null)
            {
                continue;
            }

            cover.Time = await timeSelector.InnerTextAsync();
            news.Add(cover);
        }

        return news;
    }

    private async Task<string> GetNewsContentAsync(NewsCover newsCover)
    {
        if (newsCover.Link == null)
        {
            return string.Empty;
        }
        var midUrl = GetUrlFromType(NewsType.工作动态);
        await Page.GotoAsync(Path.Combine(_baseUrl, midUrl, newsCover.Link));
        var content = await Page
            .QuerySelectorAsync("div[class='TRS_Editor']");
        if (content == null)
        {
            return string.Empty;
        }

        return await content.InnerTextAsync();
    }

    private string GetUrlFromType(NewsType type)
    {
        return type switch
        {
            // NewsType.发布会 => "jyb_xwfb/xw_fbh/moe_2069/xwfbh/",
            NewsType.政策解读 => "jyb_xwfb/s271/",
            NewsType.工作动态 => "jyb_xwfb/gzdt_gzdt/",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}