using System;

public class Class1
{
    public interface IVideoApi
    {
        Task<VideoInfo> GetVideoInfoAsync(string videoId);
    }
}
