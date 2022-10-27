using TwitterStream.Core.Models;

namespace TwitterStream.Core.Interfaces
{
    /// <summary>
    /// This is an observable class. It's job is to read stream data, and notify observers to act on it
    /// This clas does not store the stream data.
    /// 
    /// It implements the IObservable subscribe/unsubscribe for
    /// observers to get notified when data is ready.
    /// </summary>
    public interface ITweetStreamProducer : IObservable<RawTweet>
    {
        void ConfigureHttpClient(HttpClientPropertiesDelegate fnSetDbData);
        Task<int> ReadStream();
    }


    // create a delegate for setting HttpClient Uri settings
    public delegate void HttpClientPropertiesDelegate(HttpClient httpClient);
}

