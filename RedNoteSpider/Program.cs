using Spider.Common.Services.RedNote;
using System.Runtime.CompilerServices;

namespace RedNoteSpider
{
    internal class Program
    {
        private static readonly RedNoteService _redNoteService 
            = new RedNoteService();
        static async Task  Main(string[] args)
        {

            await InitializeAndLoginAsync();
        }

        private static async Task InitializeAndLoginAsync()
        {
            var islogin = await _redNoteService
                .InitializeAsync(useStorageState: true);
            await _redNoteService.LoginAsync();

            Console.WriteLine("开始爬取");
            var news = await _redNoteService
                .GetRedNotesAsync("神人");
            //await _redNoteService.GetNotesJson("神人");
            //Console.ReadKey();

            Console.WriteLine($"共爬取到{news.Count}条新闻");
            foreach (var n in news)
            {
                if (n.Content is null)
                {
                    Console.WriteLine("封面为空");
                    continue;
                }
                Console.WriteLine(n.Content);
            }


            //Console.WriteLine("开始爬取");
            //var covers = await _redNoteService
            //    .GetNoteCoversAsyncByKeyWordAsync("神人");


            //var news = await _redNoteService
            //    .GetRedNoteNewsByNoteCoversAsync(covers);

            //Console.WriteLine($"共爬取到{news.Count}条新闻");
            //foreach (var n in news)
            //{
            //    if(n.Cover is null)
            //    {
            //        Console.WriteLine("封面为空");
            //        continue;
            //    }
            //    Console.WriteLine(n.Content);
            //}



        }
    }
}
