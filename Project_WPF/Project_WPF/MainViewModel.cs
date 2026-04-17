using Project_WPF.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_WPF
{
    public class MainViewModel : NotifyBase
    {
        private readonly AppSettings _settings;
        private readonly IVideoApi _api;

        
        private string _apiKey;
        public string ApiKey { get { return _apiKey; } set { Set(ref _apiKey, value); } }

        private string _apiHost;
        public string ApiHost { get { return _apiHost; } set { Set(ref _apiHost, value); } }

        private string _baseUrl;
        public string BaseUrl { get { return _baseUrl; } set { Set(ref _baseUrl, value); } }

        private string _hl;
        public string Hl { get { return _hl; } set { Set(ref _hl, value); } }

        private string _gl;
        public string Gl { get { return _gl; } set { Set(ref _gl, value); } }

        public RelayCommand SaveSettingsCommand { get; private set; }

        private void SaveSettings()
        {
            _settings.RapidApiKey = (ApiKey ?? "").Trim();
            _settings.RapidApiHost = (ApiHost ?? "").Trim();
            _settings.BaseUrl = (BaseUrl ?? "").Trim();
            _settings.Hl = (Hl ?? "").Trim();
            _settings.Gl = (Gl ?? "").Trim();
            SettingsService.Save(_settings);
        }

        // Analyze tab
        private string _url = "";
        public string Url { get { return _url; } set { Set(ref _url, value); } }

        private string _status = "Deploy 2: API works (no DB yet).";
        public string Status { get { return _status; } set { Set(ref _status, value); } }

        private VideoInfo _lastResult;
        public VideoInfo LastResult { get { return _lastResult; } set { Set(ref _lastResult, value); } }

        public RelayCommand AnalyzeCommand { get; private set; }

        // History (in memory)
        public ObservableCollection<VideoInfo> Items { get; private set; }
        private VideoInfo _selected;
        public VideoInfo Selected { get { return _selected; } set { Set(ref _selected, value); } }

        public MainViewModel()
        {
            _settings = SettingsService.LoadOrCreate();

            ApiKey = _settings.RapidApiKey;
            ApiHost = _settings.RapidApiHost;
            BaseUrl = _settings.BaseUrl;
            Hl = _settings.Hl;
            Gl = _settings.Gl;

            _api = new CachedVideoApi(new RapidApiYoutube138Api(_settings));

            Items = new ObservableCollection<VideoInfo>();

            SaveSettingsCommand = new RelayCommand(() =>
            {
                SaveSettings();
                Status = "Settings saved ✅";
            });

            AnalyzeCommand = new RelayCommand(AnalyzeAsync);
        }

        private async Task AnalyzeAsync()
        {
            try
            {
                Status = "Calling RapidAPI...";
                SaveSettings();

                var videoId = YouTubeUrlParser.GetVideoId(Url);

                var info = await _api.GetVideoInfoAsync(videoId);
                info.Url = Url;
                info.VideoId = videoId;

                LastResult = info;
                Items.Insert(0, info);
                Selected = info;

                Status = "Loaded from API ✅ (DB in Deploy 3)";
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
        }
    }
}
