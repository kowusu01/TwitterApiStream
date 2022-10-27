using TwitterStream.Core.Models;

namespace TwitterStream.Core.Interfaces
{
    /// <summary>
    /// This is an observer class. It's job is to watch for for 
    /// new raw tweets been sent, receive it and process it.
    /// It implements the IObserver.
    /// </summary>
    public interface ITweetStreamConsumer: IObserver<RawTweet>
    {
        void Subscribe(ITweetStreamProducer producer);
        //void ConfigureDestination(object setTweetDestinationProperties);      
    }


    // create a delegate for setting destination properties,
    // it can be db connection string to store the data,
    // Kafka server properties, etc
    public delegate void TweetDestinationPropertiesDelegate(object destinationProperties);
}

