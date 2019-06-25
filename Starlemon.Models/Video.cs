using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Starlemon.Models
{
    public class Video
    {
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public List<VideoPage> Pages { get; set; }
    }
}
