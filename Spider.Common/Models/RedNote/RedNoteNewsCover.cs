using System;
using System.Collections.Generic;
using System.Text;

namespace Spider.Common.Models.RedNote
{
    public class RedNoteNewsCover : NewsCover
    {
        public string Url => $"https://www.xiaohongshu.com/explore{this.Link}pc_search";
    }
}
