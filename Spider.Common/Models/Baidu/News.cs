using System.Collections.ObjectModel;

namespace Spider.Common.Models.Baidu;

public class News
{
    public required NewsCover Item { get; set; }
    public required ObservableCollection<NewsContent>? Contents { get; set; }
}