// See https://aka.ms/new-console-template for more information

using Spider.Common.Models.ThePaper;
using Spider.Common.Services.ThePaper;

namespace ThePaperSpider;

public class ThePaperSpider
{
    private static PaperSpiderService _paperSpiderService = new();
    public static async Task Main(string[] args)
    {
        await _paperSpiderService.InitializeAsync();
        while (true)
        {

            var choice = ConsoleShowMenu();
            if (choice == "1")
            {
                Console.Write("请输入关键词：");
                var word = Console.ReadLine();
                Console.Write("请输入页码：");
                string input = Console.ReadLine();
                bool success = int.TryParse(input, out int page);
                if (!success)
                {
                    Console.WriteLine("输入页码错误，请重新输入");
                    continue;
                }
                Console.Write("请输入每页数量：");
                string inputPageSize = Console.ReadLine();
                bool successPageSize = int.TryParse(inputPageSize, out int pageSize);
                if (!successPageSize)
                {
                    Console.WriteLine("输入每页数量错误，请重新输入");
                    continue;
                }

                if(word == null)
                    continue;
                var res = await _paperSpiderService
                    .GetNewsByKeyWordsAsync(word, page, pageSize);
                
            
            }
            else if (choice == "2")
            {

                Console.WriteLine("请选择栏目：");
                var newsType = ConsoleShowSelectColumn();
                Console.Write("请输入页码：");
                string input = Console.ReadLine();
                bool success = int.TryParse(input, out int page);
                if (!success)
                {
                    Console.WriteLine("输入页码错误，请重新输入");
                    continue;
                }
                Console.Write("请输入每页数量：");
                string inputPageSize = Console.ReadLine();
                bool successPageSize = int.TryParse(inputPageSize, out int pageSize);
                if (!successPageSize)
                {
                    Console.WriteLine("输入每页数量错误，请重新输入");
                    continue;
                }
                var res = await _paperSpiderService
                    .GetNewsByPageNumAsync(newsType, page, pageSize);
                foreach (var r in res)
                {
                    Console.WriteLine(r.NewsContent);
                }
            }
            else if (choice == "3")
            {
                break;
            }
        }

    }

    private static string? ConsoleShowMenu()
    {
        Console.WriteLine("选择搜索，或直接采集？");
        Console.WriteLine("1. 搜索");
        Console.WriteLine("2. 采集");
        Console.WriteLine("3. 退出");
        Console.Write("请输入您的选择：");
        string input = Console.ReadLine();

        return input;
    }

    private static NewsType ConsoleShowSelectColumn()
    {
        Console.WriteLine("请选择栏目：");
        Console.WriteLine("1. 时事");
        Console.WriteLine("2. 国际");
        Console.WriteLine("3. 财经");
        Console.WriteLine("4. 科技");
        Console.WriteLine("5. 暖文");
        Console.WriteLine("6. 智库");
        Console.Write("请输入数字选择 (1-6): ");

        while (true)
        {
            string input = Console.ReadLine();
        
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 6)
            {
                return choice switch
                {
                    1 => NewsType.时事,
                    2 => NewsType.国际,
                    3 => NewsType.财经,
                    4 => NewsType.科技,
                    5 => NewsType.暖文,
                    6 => NewsType.智库,
                    _ => NewsType.时事 // 默认值
                };
            }
        
            Console.Write("输入无效，请重新输入数字 (1-6): ");
        }
    }
}