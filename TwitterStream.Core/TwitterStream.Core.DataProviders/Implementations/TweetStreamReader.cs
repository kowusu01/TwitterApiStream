
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

using TwitterStream.Core.Models;
using TwitterStream.Core.Utils;
using TwitterStream.Core.Interfaces;

namespace TwitterStream.Core.Implementations
{
    /// <summary>
    /// Implemenatation for IStreamObjectReader.
    /// This class watches for changes to an observable list 
    /// and publishes the items to a Kafka topic for consumption.
    /// </summary>
    public class TweetStreamReader : ITweetStreamReader
    {
        private readonly ILogger<TweetStreamReader> _logger;
        
        // for now, we have only one observer to process tweets as they came in
        private IObserver<RawTweet>? _tweetsObserver;
        
        private HttpClient _httpClient = new HttpClient();

        public TweetStreamReader(ILogger<TweetStreamReader> logger)
        {
            _logger = logger;          
        }

        // The ConfigureHttpClient is a regular setter method
        // But unlike regular other setter methods, it does not take required data,
        // instead it takes in a method that knows where to get the required data
        public void ConfigureHttpClient(HttpClientPropertiesDelegate fnSetDbData)
        {
            fnSetDbData(_httpClient);
        }

        public async Task<int> ReadStream()
        {
            string twitterApiPath =
                $"?{TwitterAPIConstants.API_PARAMETERS_TWEET_FIELDS}&" +
                $"{TwitterAPIConstants.API_PARAMETERS_USER_FIELDS}&" +
                $"{TwitterAPIConstants.API_PARAMETERS_EXPANSIONS}";

            var responseStream = await _httpClient.GetStreamAsync(twitterApiPath);

            string data = string.Empty;
            int count = 1;

            using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
            {
                while (!readStream.EndOfStream)
                //while (count < 100)
                {
                    data = await readStream.ReadLineAsync();
                    
                    // notify our one and only one observer
                    _tweetsObserver.OnNext(new RawTweet() { TweetJsonString = data});

                    if (string.IsNullOrEmpty(data))
                    {
                        _tweetsObserver.OnCompleted();
                        break;
                    }                
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// subscribe to data source and watch for changes
        /// </summary>
        /// <param name="dataSource">data source to subscribe to</param>
        public IDisposable Subscribe(IObserver<RawTweet> observer)
        {
            _tweetsObserver = observer;
            return new TweetUnsubscriber();
        }
        
        /// <summary>
        /// unsubscribe from the data source, i.e. stop watching for data
        /// / right now, you can't unsubscribe
        /// </summary>
        public void UnSubscribeFrom()
        {
        }

    }


    // does not do much right now
    // following the pattern from: https://learn.microsoft.com/en-us/dotnet/standard/events/how-to-implement-a-provider
    public class TweetUnsubscriber : IDisposable
    {
        public TweetUnsubscriber()
        {
        }

        public void Dispose()
        {    
        }
    }

}
