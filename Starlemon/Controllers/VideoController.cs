using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Starlemon.Models;

namespace Starlemon.Controllers
{
    [Route("/video")]
    public class VideoController : Controller
    {
        private readonly StarlemonContext _starlemonContext;

        public VideoController(StarlemonContext starlemonContext)
        {
            _starlemonContext = starlemonContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id, int pageNumber = 1)
        {
            //if (!Videos.TryGetValue(id, out var infos))
            //{
            //    return NotFound();
            //}

            //ViewBag.Url = infos[0].url;
            //var videoQualities = infos.Select(t => new VideoQuality(t.name, t.url));

            // Check page number.
            if (pageNumber <= 0)
                return BadRequest();
            // Get video and pages, as well as verify if page exists.
            var video = await _starlemonContext.Videos.Where(v => v.Id == id).Include(v => v.Pages).FirstOrDefaultAsync();
            if (video == null || video.Pages.Count < pageNumber)
            {
                return NotFound();
            }

            var page = video.Pages[pageNumber - 1];
            // Set pages.
            ViewBag.Pages = video.Pages;

            // Set title.
            ViewBag.VideoTitle = string.IsNullOrEmpty(page.Remark) ? video.Title : page.Remark;

            // Get nodes.
            var nodes = await _starlemonContext.VideoPages.Where(p => p.Id == page.Id).SelectMany(p => p.Nodes).Include(p => p.Node).ToListAsync();

            var videoQualities = nodes.Select(n => new VideoQuality(n.Node.Name, new Uri(n.Node.Uri, page.VideoPath).AbsoluteUri));

            // Convert qualities to json and return.
            ViewBag.Quality = JsonConvert.SerializeObject(videoQualities);
            return View();
        }

        public const string VideoInfo = @"
2;fr,https://media.bleatingsheep.org/lemon/manifest.mpd;cn,https://pi.bleatingsheep.org:4338/lemon/manifest.mpd
1;la,https://media2.bleatingsheep.org/Starry%20Desert/manifest.mpd;fr,https://media.bleatingsheep.org/Starry%20Desert/manifest.mpd
3;fr,https://media.bleatingsheep.org/summer/index.mpd;cn,https://pi.bleatingsheep.org:4338/summer/index.mpd
4;fr,https://media.bleatingsheep.org/tari2/manifest.mpd";

        public static readonly Dictionary<int, (string name, string url)[]> Videos
            = new Dictionary<int, (string, string)[]>();

        static VideoController()
        {
            var sr = new StringReader(VideoInfo);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var info = line.Split(';', StringSplitOptions.None);
                var id = int.Parse(info[0]);
                var contents = new List<(string name, string url)>();
                foreach (var item in info.Skip(1))
                {
                    var array = item.Split(',', StringSplitOptions.None);
                    contents.Add((array[0], array[1]));
                }
                Videos.Add(id, contents.ToArray());
            }
        }

        public class VideoQuality
        {
            public VideoQuality([JsonProperty]string name, [JsonProperty]string url)
            {
                Name = name;
                Url = url;
            }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; } = "splr";
        }
    }
}