using System.Threading.Tasks;
using Project_WPF.Models;

namespace Project_WPF.Services
{
    public interface IVideoApi
    {
        Task<VideoInfo> GetVideoInfoAsync(string videoId);
    }
}