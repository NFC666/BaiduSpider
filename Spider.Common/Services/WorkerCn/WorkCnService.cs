using System.Text;
using Microsoft.Playwright;
using Spider.Common.Models;
using Spider.Common.Models.WorkerCn;
using Spider.Common.Services.Baidu;
using News = Spider.Common.Models.WorkerCn.News;
using NewsCover = Spider.Common.Models.WorkerCn.NewsCover;

namespace Spider.Common.Services.WorkerCn;

public class WorkCnService : PlaywrightService
{
    private readonly string baseUrl = "https://www.workercn.cn/";

    private static string saveDic = "./WorkCn";
    private readonly FileService _fileService = new(saveDic);

    public async Task<List<News>> GetNewsByNewsType(NewsType newsType)
    {
        var news = new List<News>();
        string url = GetUrlFromType(newsType);
        await Page.GotoAsync(Path.Combine(baseUrl, url));
        var selectors = await
            Page.QuerySelectorAllAsync("li[id^='li_']");
        foreach (var selector in selectors)
        {
            try
            {
                var cover = await GetNewsCoverFromSelector(selector);
                news.Add(new News
                {
                    Cover = cover
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("获取新闻封面失败");
                Console.WriteLine(e);

            }

        }

        foreach (var n in news)
        {
            try
            {
                n.Content = await GetContentFromUrl(n.Cover.Url);
            }catch(Exception e)
            {
                Console.WriteLine("获取新闻内容失败");
                Console.WriteLine(e);
            }
            await _fileService.SaveAllContentToJson(news, SpiderSource.WorkerCn);
            
            Console.WriteLine($"文章已保存在{saveDic}目录下");
        }
        
        return news;
    }
    
    private async Task<NewsCover> GetNewsCoverFromSelector(IElementHandle selector)
    {
        var cover = new NewsCover();

        var title = await selector.QuerySelectorAsync("h3");
        cover.Title = await title.InnerTextAsync();

        var link = await selector.QuerySelectorAsync("a");
        cover.Link = Path.Combine(baseUrl, await link.GetAttributeAsync("href"));

        var source = await selector
            .QuerySelectorAsync("span[style='float:right']");

        cover.Source = await source.InnerTextAsync();

        var time = await selector
            .QuerySelectorAsync("div[class='info2']");
        cover.Time = await time.InnerTextAsync();

        var coverImg = await selector
            .QuerySelectorAsync("img");
        cover.CoverImg = await coverImg.GetAttributeAsync("src");
        
        
        return cover;
    }


    private async Task<string> GetContentFromUrl(string url)
    {
        
        await Page.GotoAsync(url);
        
        var paragraphs = await Page
            .QuerySelectorAsync("div[class='ccontent']");
        if (paragraphs == null)
        {
            return string.Empty;
        }
        var content = await paragraphs.InnerTextAsync();
        return content;


    }
    private string GetUrlFromType(NewsType type)
    {
        return type switch
        {
            NewsType.全总 => "quanzong",
            NewsType.工会 => "acftu",
            NewsType.评论 => "pl/pl",
            NewsType.权益 => "right",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}