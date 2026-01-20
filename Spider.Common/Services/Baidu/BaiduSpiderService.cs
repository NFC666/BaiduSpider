using System.Text;
using Microsoft.Playwright;
using Spider.Common.Models;
using Spider.Common.Models.Baidu;
using News = Spider.Common.Models.Baidu.News;
using NewsCover = Spider.Common.Models.Baidu.NewsCover;

namespace Spider.Common.Services.Baidu;

public class BaiduSpiderService : PlaywrightService
{

    public string Directory { get; set; } = "./Baidu";
    private readonly FileService _fileService = new("Directory");

    public async Task GotoBaiduHomePage()
    {
        await Page.GotoAsync("https://www.baidu.com");
    }
    
    public async Task<List<NewsCover>> GetNewsItems()
    {
        var result = new List<NewsCover>();

        // 选中所有 hotsearch-item
        var items = await 
            Page.QuerySelectorAllAsync("li.hotsearch-item");

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
                result.Add(new NewsCover
                {
                    Title = title,
                    Url = url
                });
            }
        }

        return result;
    }

    public async Task<List<News>> GetNewsContent(List<NewsCover> newsItems)
    {
        var newsContents = new List<News>();

        

        foreach (var news in newsItems)
        {
            var newsContent = new News
            {
                Item = news,
                Contents = new()
            };

            await Page.GotoAsync(news.Url, new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            var titles = Page.Locator("div[class^='title-wrapper_']");
            var count = Math.Min((int)await titles.CountAsync(), 10);
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
                await Page.WaitForTimeoutAsync(800);
            }
            
            foreach (var url in urls)
            {
                var sb = new StringBuilder();
                try
                {
                    await Page.GotoAsync(url, new()
                    {
                        WaitUntil = WaitUntilState.NetworkIdle
                    });
                    
                    await Page.Locator("body")
                        .WaitForAsync(new() { Timeout = 5000 });
                    // var text = await _page.EvaluateAsync<string>("() => document.body.innerText");
                    var textContent = await Page.Locator("p").AllTextContentsAsync();
                    sb.Append(string.Join((string?)"\n", (IEnumerable<string?>)textContent));
                    // newsContent.Content.Add(sb.ToString());
                    newsContent.Contents.Add(new NewsContent
                    {
                        Url = url,
                        Content = sb.ToString()
                    });
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            newsContents.Add(newsContent);
            await _fileService.SaveAllContentToJson(newsContents, SpiderSource.Baidu);
        }

        return newsContents;
    }


    public async Task<List<NewsContent>> GetNewsContentByHotWordAsync(NewsCover hotWord)
    {
        var contents = new List<NewsContent>();
        await Page.GotoAsync(hotWord.Url, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
        var titles = Page.Locator("div[class^='title-wrapper_']");
        var count = Math.Min((int)await titles.CountAsync(), 10);
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
            await Page.WaitForTimeoutAsync(200);
        }
        foreach (var url in urls)
        {
            var sb = new StringBuilder();
            try
            {
                await Page.GotoAsync(url, new()
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });

                await Page.Locator("body")
                    .WaitForAsync(new() { Timeout = 5000 });
                // var text = await _page.EvaluateAsync<string>("() => document.body.innerText");
                var textContent = await Page.Locator("p").AllTextContentsAsync();
                sb.Append(string.Join((string?)"\n", (IEnumerable<string?>)textContent));
                // newsContent.Content.Add(sb.ToString());
                contents.Add(new NewsContent
                {
                    Url = url,
                    Content = sb.ToString()
                });
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        return contents;

    }
}