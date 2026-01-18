namespace Spider.Common.Models.WorkerCn;

public class NewsCover
{
    public string? Title { get; set; }
    public string? Link { get; set; }

    public string Url
    {
        get
        {
            if (Link != null) return "https://www.workercn.cn" + Link;
            return string.Empty;
        }
    }

    public string? Source { get; set; }
    public string? Time { get; set; }
    public string? CoverImg { get; set; }
}