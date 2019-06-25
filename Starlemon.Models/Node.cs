using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Starlemon.Models
{
    public class Node
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public Uri Uri => new Uri(Url);

        public List<VideoPageNode> StoredPages { get; set; }
    }
}
