using Microsoft.Playwright;
using Spider.Common.Models.RedNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spider.Common.Services.RedNote
{
    public class RedNoteService : PlaywrightService
    {
        public override string StorageStatePath => "./xiaohongshu.json";
        private string loginUrl = "https://www.xiaohongshu.com/login";
        private string? searchId;
        public async Task LoginAsync()
        {
            if (File.Exists(StorageStatePath))
            {
                await Context.StorageStateAsync(new()
                {
                    Path = StorageStatePath
                });
                return;
            }
            await Page.GotoAsync(loginUrl);
            await WaitForLoginSuccess();
        }


        public async Task<List<RedNoteNews>> GetRedNotesAsync(string keyWord
            , int batch = 5
            )
        {
            var news = new List<RedNoteNews>();
            var url = $"https://www.xiaohongshu.com/search_result/?keyword={keyWord}";
            await Page.GotoAsync(url, new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
            for (int i = 0; i < batch; i++)
            {
                var selectors = await Page.QuerySelectorAllAsync("section[class='note-item']");

                int j = i % selectors.Count;
                var selector = selectors[j];
                {
                    var trigger = await selector.InnerTextAsync();
                    if (trigger.Contains("大家都在搜"))
                    {
                        continue;
                    }
                }
                var cover = await GetCoverFromSelector(selector);
                //cover mask ld
                var a = await selector.QuerySelectorAsync("a[class='cover mask ld']");
                if (a == null)
                {
                    continue;
                }
                var href = await a.GetAttributeAsync("href");

                if (href == null)
                {
                    continue;
                }

                await Page.GotoAsync("https://www.xiaohongshu.com" + href, new()
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });


                var content = await RedNoteNewsContentFromCover();
                news.Add(new RedNoteNews
                {
                    Cover = cover,
                    Content = content
                });


                await Page.WaitForTimeoutAsync(1_000);
                await Page.GoBackAsync();
                if (i + 1 % 4 == 0)
                {
                    var container = await Page.QuerySelectorAsync("div[class='feeds-container']");
                    if (container == null)
                    {
                        continue;
                    }
                    await container.ScrollIntoViewIfNeededAsync();
                }
            }
            return news;
        }

        public async Task GetNotesJson(string keyWord)
        {

            var url = $"https://www.xiaohongshu.com/search_result/?keyword={keyWord}";
            await Page.GotoAsync(url, new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
            Page.Response += async (_, response) =>
            {
                try
                {
                    var request = response.Request;

                    // 只关心 POST
                    if (request.Method != "POST")
                        return;


                    //https://edith.xiaohongshu.com/api/sns/web/v1/search/notes

                    // 只拦截目标接口
                    if (!request.Url.Contains("/api/sns/web/v1/search/notes"))
                        return;

                    Console.WriteLine("命中接口：" + request.Url);

                    // 获取返回 JSON
                    var json = await response.JsonAsync();

                    Console.WriteLine(json.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

        }

        private async Task WaitForLoginSuccess()
        {
            await Page.WaitForURLAsync(
                url => !url.Contains("login"),
                new()
                {
                    Timeout = 1_000 * 60
                }
            );
            await Page.Context.StorageStateAsync(new()
            {
                Path = StorageStatePath
            });
        }

        //public async Task<List<RedNoteNews>>
        //    GetRedNoteNewsByClickAsync(string keyWord, int item = 50)
        //{
        //    var news = new List<RedNoteNews>();
        //    var notes = new List<RedNoteNewsCover>();
        //    var url = $"https://www.xiaohongshu.com/search_result/?keyword={keyWord}";
        //    await Page.GotoAsync(url, new()
        //    {
        //        WaitUntil = WaitUntilState.NetworkIdle
        //    });
        //    for (int i = 0; i < item; i++)
        //    {
        //        var selectors = await Page.QuerySelectorAllAsync("section[class='note-item']");

        //        for (int i = 0; i < selectors.Count; i++)
        //        {
        //            var cover = await GetCoverFromSelector(selectors[i]);
        //        }
        //    }

        //    return notes;
        //}




        private async Task<RedNoteNewsCover>
            GetCoverFromSelector(IElementHandle selector)
        {
            var cover = new RedNoteNewsCover()
            {
                CoverImg = "",
                Title = "",
                Time = "",
                Link = "",
            };

            try
            {

                var img = await selector.QuerySelectorAsync("img[loading='lazy']");

                if (img != null)
                {
                    cover.CoverImg = await img.GetAttributeAsync("src");
                }

                var title = await selector.QuerySelectorAllAsync("span");
                if (title.Count > 0)
                {
                    cover.Title = await title.First().InnerTextAsync();
                }

                var time = await selector.QuerySelectorAsync("div[class='time']");

                if (time != null)
                {
                    cover.Time = await time.InnerTextAsync();
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

            return cover;

        }

        public async Task<string> RedNoteNewsContentFromCover()
        {

            var content = await Page.QuerySelectorAsync("div[class='note-content']");
            if (content == null)
            {
                return string.Empty;
            }
            var text = await content.InnerTextAsync();
            return text;

        }

    }
}
