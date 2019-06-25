using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Starlemon.Models
{
    public class VideoPage
    {
        public long Id { get; set; }

        public int PageId { get; set; }

        public string Remark { get; set; }

        [Required]
        public string VideoPath { get; set; }

        public long VideoId { get; set; }
        public Video Video { get; set; }

        public List<VideoPageNode> Nodes { get; set; }
    }
}
