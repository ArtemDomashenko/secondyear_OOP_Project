using System;

namespace Project_WPF.Models
{
    public class VideoInfo
    {
        public string Url { get; set; } = "";
        public string VideoId { get; set; } = "";
        public string Title { get; set; } = "";
        public long Views { get; set; }
        public DateTime PublishedUtc { get; set; }
        public string ThumbnailUrl { get; set; } = "";
    }
}