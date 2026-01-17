using Spider.Common.Models;
using Spider.Common.Services;

namespace BaiduSpider
{
    class Program
    {
        private static readonly BaiduSpiderService BaiduSpiderService = new();
        private static readonly FileService _fileService = new();
        
        static async Task Main(string[] args)
        {
            var newsItems = new List<HotWord>();
            try
            {
                await BaiduSpiderService.InitializeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化Playwright出错，详情：" + ex.Message);
                return;
            }
            try{
                await BaiduSpiderService.GotoBaiduHomePage();
            }catch(Exception ex)
            {
                Console.WriteLine("进入百度首页出错，详情："+ex.Message);
                return;
            }
            try
            {
                newsItems = await BaiduSpiderService.GetNewsItems();
                foreach (var news in newsItems)
                {
                    Console.WriteLine(news.Title);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取新闻出错，详情：" + ex.Message);
                return;
            }
            try
            {
                var newsContents = await BaiduSpiderService
                    .GetNewsContent(newsItems);

                await _fileService.SaveAllContentToJson(newsContents);
                Console.WriteLine("获取成功，文件保存在当前目录下newsContent.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取新闻内容出错，详情：" + ex.Message);
                return;
            }

            Console.ReadLine();
        }
        

    }
}