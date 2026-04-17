using System;
using System.Text.RegularExpressions;

namespace Project_WPF.Services
{
    public static class YouTubeUrlParser
    {
        private static readonly Regex Watch = new Regex(@"[?&]v=([a-zA-Z0-9_-]{6,})", RegexOptions.Compiled);
        private static readonly Regex Short = new Regex(@"youtu\.be/([a-zA-Z0-9_-]{6,})", RegexOptions.Compiled);
        private static readonly Regex Shorts = new Regex(@"youtube\.com/shorts/([a-zA-Z0-9_-]{6,})", RegexOptions.Compiled);

        public static string GetVideoId(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("URL is empty.");

            input = input.Trim();

            if (Regex.IsMatch(input, @"^[a-zA-Z0-9_-]{6,}$"))
                return input;

            var m1 = Watch.Match(input);
            if (m1.Success) return m1.Groups[1].Value;

            var m2 = Short.Match(input);
            if (m2.Success) return m2.Groups[1].Value;

            var m3 = Shorts.Match(input);
            if (m3.Success) return m3.Groups[1].Value;

            throw new ArgumentException("Invalid YouTube URL.");
        }
    }
}