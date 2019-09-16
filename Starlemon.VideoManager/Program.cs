using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Starlemon.Models;

namespace Starlemon.VideoManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await AddFullSeasonAsync("title", 24, "title/{0:D2}/manifest.mpd", new[] { "unicom" });
            //await AddNodesForVideo(14, new[] { "cn" });
            IEnumerable<string> GetTitles()
            {
                for (int i = 0; i < 24; i++)
                {
                    yield return Console.ReadLine();
                }
            }
            await AddPageRemarkAsync(14, GetTitles());
        }

        static async Task AddFullSeasonAsync(string title, int episodes, string path, string[] nodes)
        {
            using (var starlemonContext = new StarlemonContext())
            using (var transcation = await starlemonContext.Database.BeginTransactionAsync())
            {
                var node = starlemonContext.Set<Node>().Where(n => nodes.Contains(n.Name));
                var nodeIds = await node.Select(n => n.Id).ToListAsync();
                var video = new Video
                {
                    Title = title,
                    Pages = Enumerable.Range(0, episodes).Select(
                        i =>
                            new VideoPage
                            {
                                Nodes = nodeIds.Select(ni => new VideoPageNode { NodeId = ni }).ToList(),
                                PageId = i + 1,
                                VideoPath = string.Format(path, i + 1),
                            }
                        ).ToList()
                };

                starlemonContext.Videos.Add(video);
                await starlemonContext.SaveChangesAsync();
                transcation.Commit();
            }
        }

        static async Task AddNodesForVideo(int videoId, string[] nodes)
        {
            using (var starlemonContext = new StarlemonContext())
            using (var transcation = await starlemonContext.Database.BeginTransactionAsync())
            {
                var node = starlemonContext.Set<Node>().Where(n => nodes.Contains(n.Name));
                var nodeIds = await node.Select(n => n.Id).ToListAsync();
                IEnumerable<VideoPageNode> pageNodesLazy = nodeIds.Select(nodeId => new VideoPageNode { NodeId = nodeId });
                var pages = await starlemonContext.VideoPages.Include(p => p.Nodes).Where(p => p.VideoId == videoId).ToListAsync();
                foreach (var page in pages)
                {
                    page.Nodes.AddRange(pageNodesLazy);
                }
                await starlemonContext.SaveChangesAsync();
                transcation.Commit();
            }
        }

        private static async Task AddPageRemarkAsync(int videoId, IEnumerable<string> remarks)
        {
            using (var starlemonContext = new StarlemonContext())
            using (var transcation = await starlemonContext.Database.BeginTransactionAsync())
            {
                var pages = await starlemonContext.VideoPages.Where(p => p.VideoId == videoId).OrderBy(p => p.PageId).ToListAsync();
                var updatedPages = pages.Zip(remarks, (p, r) =>
                {
                    p.Remark = r;
                    return p;
                });
                starlemonContext.VideoPages.UpdateRange(updatedPages);
                await starlemonContext.SaveChangesAsync();
                transcation.Commit();
            }
        }
    }
}
