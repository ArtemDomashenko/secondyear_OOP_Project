using Project_WPF.Mvvm;
using Project_WPF.Models;
using Project_WPF.Services;
using System;
using System.Collections.ObjectModel;
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

        private string _url = "";
        public string Url { get { return _url; } set { Set(ref _url, value); } }

        private string _status = "Enter a YouTube URL and click Analyze.";
        public string Status { get { return _status; } set { Set(ref _status, value); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); AnalyzeCommand.RaiseCanExecuteChanged(); }
        }

        private VideoInfo _lastResult;
        public VideoInfo LastResult { get { return _lastResult; } set { Set(ref _lastResult, value); } }

        public RelayCommand AnalyzeCommand { get; private set; }

        public ObservableCollection<VideoInfo> Items { get; private set; }

        private VideoInfo _selected;
        public VideoInfo Selected { get { return _selected; } set { Set(ref _selected, value); } }

       
        private string _searchText = "";
        public string SearchText
        {
            get { return _searchText; }
            set { Set(ref _searchText, value); RefreshFilter(); }
        }

        public ObservableCollection<VideoInfo> FilteredItems { get; private set; }

        private void RefreshFilter()
        {
            FilteredItems.Clear();
            foreach (var v in Items)
            {
                if (string.IsNullOrWhiteSpace(SearchText) ||
                    v.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    FilteredItems.Add(v);
            }
        }

        public void DeleteItem(VideoInfo video)
        {
            Items.Remove(video);
            FilteredItems.Remove(video);
            if (Selected == video) Selected = null;
        }

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
            FilteredItems = new ObservableCollection<VideoInfo>();  

            SaveSettingsCommand = new RelayCommand(() =>
            {
                SaveSettings();
                Status = "Settings saved ";
            });

            AnalyzeCommand = new RelayCommand(AnalyzeAsync, () => !IsBusy);
        }

        private async Task AnalyzeAsync()
        {
            try
            {
                IsBusy = true;
                Status = "Calling RapidAPI...";
                SaveSettings();

                var videoId = YouTubeUrlParser.GetVideoId(Url);
                var info = await _api.GetVideoInfoAsync(videoId);
                info.Url = Url;
                info.VideoId = videoId;

                LastResult = info;

                bool exists = false;
                foreach (var v in Items)
                    if (v.VideoId == videoId) { exists = true; break; }

                if (!exists) Items.Insert(0, info);
                RefreshFilter();  

                Selected = info;
                Status = "Loaded ";
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}