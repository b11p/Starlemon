namespace Starlemon.Models
{
    public class VideoPageNode
    {
        public int NodeId { get; set; }
        public Node Node { get; set; }

        public long VideoPageId { get; set; }
        public VideoPage VideoPage { get; set; }
    }
}
