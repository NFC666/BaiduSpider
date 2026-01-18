// See https://aka.ms/new-console-template for more information

using Spider.Common.Models.WorkerCn;
using Spider.Common.Services.WorkerCn;

namespace WorkerCnSpider
{
    internal class WorkerCnSpider
    {
        private static readonly WorkCnService _workCnService = new();

        public static async Task Main(string[] args)
        {
            await _workCnService.InitializeAsync();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== 新闻类型选择 ====");
                foreach (var value in Enum.GetValues<NewsType>())
                {
                    Console.WriteLine($"{(int)value} - {value}");
                }
                Console.WriteLine("q - 退出程序");
                Console.Write("请选择新闻类型：");

                var input = Console.ReadLine();

                if (input?.Trim().ToLower() == "q")
                    break;

                if (int.TryParse(input, out int choice) &&
                    Enum.IsDefined(typeof(NewsType), choice))
                {
                    var newsType = (NewsType)choice;
                    Console.WriteLine($"你选择了: {newsType}");
                
                    try
                    {
                        // 调用你的服务处理逻辑
                        await _workCnService.GetNewsByNewsType(newsType);
                        Console.WriteLine("处理完成！按任意键继续...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"处理失败: {ex.Message}");
                    }
                
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("输入无效，请重新选择。按任意键继续...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("程序已退出。");
        }
    }
}
