using TwitterStream.Core.Models;

namespace TwitterStream.Core.Interfaces
{
    /// <summary>
    /// This is an observer class. It's job is to watch for for 
    /// new raw tweets been sent, receive it and process it.
    /// It implements the IObserver.
    /// </summary>
    public interface ITweetStreamProcessor: IObserver<RawTweet>
    {
        void Subscribe(ITweetStreamReader producer);

        // for future implementation:
        //void ConfigureDestination(object setTweetDestinationProperties);

        void AddTweetHashTag(string tag);
        IEnumerable<KeyValuePair<string, int>> GetTopNHashtags(int n);        
    }


    // for future implementation:
    // create a delegate for setting destination properties,
    // it can be db connection string to store the data,
    // Kafka server properties, etc
    public delegate void TweetDestinationPropertiesDelegate(object destinationProperties);
}

