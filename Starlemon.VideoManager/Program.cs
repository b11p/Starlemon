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
            await AddFullSeasonAsync("title", 24, "title/{0:D2}/manifest.mpd", new[] { "unicom" });
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
    }
}
