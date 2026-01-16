using BaiduSpider.Models;

namespace BaiduSpider.Services;
using Microsoft.Playwright;
public class PlaywrightService
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;
    
    private readonly FileService _fileService = new();

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            Timeout = 10000,
            ExecutablePath = edgePath // 使用系统浏览器
        });
        _page = await _browser.NewPageAsync();
    }

    public async Task GotoBaiduHomePage()
    {
        await _page.GotoAsync("https://www.baidu.com");
    }
    
    public async Task<List<NewsItem>> GetNewsItems()
    {
        var result = new List<NewsItem>();

        // 选中所有 hotsearch-item
        var items = await 
            _page.QuerySelectorAllAsync("li.hotsearch-item");

        foreach (var item in items)
        {
            // a 标签
            var link = await item.QuerySelectorAsync("a.title-content");
            if (link == null)
                continue;

            var url = await link.GetAttributeAsync("href");

            // 标题
            var titleElement = await link.QuerySelectorAsync(".title-content-title");
            var title = titleElement == null
                ? string.Empty
                : (await titleElement.InnerTextAsync()).Trim();

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(url))
            {
                result.Add(new NewsItem
                {
                    Title = title,
                    Url = url
                });
            }
        }

        return result;
    }

    public async Task<List<NewsContent>> GetNewsContent(List<NewsItem> newsItems)
    {
        var newsContents = new List<NewsContent>();
        // //newsContents截取，不要第一个元素
        // newsItems = newsItems.Skip(1).ToList();
        

        foreach (var news in newsItems)
        {
            var newsContent = new NewsContent
            {
                Item = news,
                Content = new()
            };

            await _page.GotoAsync(news.Url, new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            var titles = _page.Locator("div[class^='title-wrapper_']");
            var count = Math.Min(await titles.CountAsync(), 10);
            string[] urls = new string[count];
            for (int i = 0; i < count; i++)
            {
                var title = titles.Nth(i);

                // await title.ScrollIntoViewIfNeededAsync();
                // await title.ClickAsync();
                var linkLocator = title.Locator("a").First;
                var href = await linkLocator.GetAttributeAsync("href");
                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }
                urls[i] = href;

                // 防反爬（很重要）
                await _page.WaitForTimeoutAsync(800);
            }



            foreach (var url in urls)
            {
                try
                {
                    await _page.GotoAsync(url, new()
                    {
                        WaitUntil = WaitUntilState.NetworkIdle
                    });
                    
                    await _page.Locator("body")
                        .WaitForAsync(new() { Timeout = 5000 });
                    var text = await _page.EvaluateAsync<string>("() => document.body.innerText");
                    newsContent.Content.Add(text);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            newsContents.Add(newsContent);
            await _fileService.SaveAllContentToJson(newsContents);
        }

        return newsContents;
    }

}