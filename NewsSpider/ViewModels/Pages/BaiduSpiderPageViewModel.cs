using Spider.Common.Models.Baidu;
using Spider.Common.Services.Baidu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Documents;

namespace NewsSpider.ViewModels.Pages
{
    public partial class BaiduSpiderPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _saveDirectory = "./Baidu";
        [ObservableProperty]
        private ObservableCollection<News> _newsContents = new();

        private readonly BaiduSpiderService _baiduSpiderService;

        public BaiduSpiderPageViewModel(BaiduSpiderService baiduSpiderService)
        {
            _baiduSpiderService = baiduSpiderService;
        }

        [RelayCommand]
        public async Task StartSpider()
        {
            try
            {
                await InitialSpider();
                var newsItems = await _baiduSpiderService.GetNewsItems();
                await GetHotWordAsync(newsItems);
                await GetContentByHotWord();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"发生错误");
                return;
            }
        }

        private async Task InitialSpider()
        {
            try
            {
                await _baiduSpiderService.InitializeAsync();
                await _baiduSpiderService.GotoBaiduHomePage();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "初始化出错");
                return;
            }
        }

        private async Task GetHotWordAsync(List<NewsCover> hotWords)
        {
            foreach (var news in hotWords)
            {
                NewsContents.Add(new News
                {
                    Item = news,
                    Contents = new()
                });
            }
        }

        private async Task GetContentByHotWord()
        {

            for(int i = 0; i < NewsContents.Count; i++)
            {
                var news = NewsContents[i];
                var newsContents = await _baiduSpiderService
                    .GetNewsContentByHotWordAsync(news.Item);
                NewsContents[i].Contents = [..newsContents];
            }

            
        }


    }
}
