using Newtonsoft.Json;

namespace Spider.Common.Models.ThePaper;

public class NewsCover
{
    [JsonProperty("contId")]
    public string? ConId { get; set; }
    // https://www.thepaper.cn/newsDetail_forward_32406454
    [JsonIgnore]
    public string Link =>  $"https://www.thepaper.cn/newsDetail_forward_{ConId}";
    [JsonProperty("pic")]
    public string? Picture { get; set; }
    [JsonProperty("name")]
    public string? Name { get; set; }
}

public class NodeInfo
{
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("desc")]
    public string? Description { get; set; }
    
    [JsonProperty("summarize")]
    public string? Summarize { get; set; }
}