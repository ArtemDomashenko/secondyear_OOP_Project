using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Project_WPF.Models;

namespace Project_WPF.Services
{
    public class RapidApiYoutube138Api : IVideoApi
    {
        private readonly AppSettings _settings;
        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        public RapidApiYoutube138Api(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<VideoInfo> GetVideoInfoAsync(string videoId)
        {
            if (string.IsNullOrWhiteSpace(_settings.RapidApiKey))
                throw new Exception("RapidAPI key is empty (go to Settings).");

            var url = BuildUrl(videoId);

            using (var req = new HttpRequestMessage(HttpMethod.Get, url))
            {
                req.Headers.Add("X-RapidAPI-Key", _settings.RapidApiKey);
                req.Headers.Add("X-RapidAPI-Host", _settings.RapidApiHost);

                using (var resp = await Http.SendAsync(req))
                {
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                        throw new Exception((int)resp.StatusCode + " " + resp.ReasonPhrase + ". " + TryMessage(body));

                    return Parse(body);
                }
            }
        }

        private string BuildUrl(string videoId)
        {
            var baseUrl = (_settings.BaseUrl ?? "").Trim().TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl))
                baseUrl = "https://youtube138.p.rapidapi.com";

            var hl = string.IsNullOrWhiteSpace(_settings.Hl) ? "en" : _settings.Hl.Trim();
            var gl = string.IsNullOrWhiteSpace(_settings.Gl) ? "US" : _settings.Gl.Trim();

            return baseUrl + "/video/details/?id=" + Uri.EscapeDataString(videoId)
                           + "&hl=" + Uri.EscapeDataString(hl)
                           + "&gl=" + Uri.EscapeDataString(gl);
        }

        private VideoInfo Parse(string json)
        {
            using (var doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                var info = new VideoInfo();

                info.Title = root.TryGetProperty("title", out var t)
                    ? (t.GetString() ?? "(no title)")
                    : "(no title)";

                if (root.TryGetProperty("stats", out var stats) &&
                    stats.TryGetProperty("views", out var v))
                {
                    if (v.ValueKind == JsonValueKind.Number)
                        info.Views = v.GetInt64();
                    else { long tmp; long.TryParse(v.GetString(), out tmp); info.Views = tmp; }
                }

                info.PublishedUtc = DateTime.UtcNow;
                if (root.TryGetProperty("publishedDate", out var pd))
                {
                    var s = pd.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                        info.PublishedUtc = DateTime.Parse(s, null,
                            System.Globalization.DateTimeStyles.AdjustToUniversal);
                }

                info.ThumbnailUrl = "";
                if (root.TryGetProperty("thumbnails", out var thumbs) &&
                    thumbs.ValueKind == JsonValueKind.Array &&
                    thumbs.GetArrayLength() > 0)
                {
                    var last = thumbs[thumbs.GetArrayLength() - 1];
                    if (last.TryGetProperty("url", out var u))
                    {
                        var url = u.GetString();
                        if (!string.IsNullOrWhiteSpace(url))
                            info.ThumbnailUrl = url;
                    }
                }

                return info;
            }
        }

        private string TryMessage(string body)
        {
            try
            {
                using (var doc = JsonDocument.Parse(body))
                {
                    if (doc.RootElement.TryGetProperty("message", out var msg))
                        return msg.GetString() ?? "";
                }
                return "";
            }
            catch
            {
                if (string.IsNullOrWhiteSpace(body)) return "";
                body = body.Trim();
                return body.Length > 140 ? body.Substring(0, 140) + "..." : body;
            }
        }
    }
}