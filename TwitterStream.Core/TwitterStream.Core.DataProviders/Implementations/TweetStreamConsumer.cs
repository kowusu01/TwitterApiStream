using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStream.Core.Interfaces;
using TwitterStream.Core.Models;
using TwitterStream.Core.Utils;

/// reference: https://learn.microsoft.com/en-us/dotnet/standard/events/how-to-implement-an-observer
namespace TwitterStream.Core.Implementations
{
    public   class TweetStreamConsumer : ITweetStreamConsumer
    {
        ILogger<TweetStreamConsumer> _logger;
        IDisposable? _unsubscriber;
        object? _destinationProperties;

        public TweetStreamConsumer(ILogger<TweetStreamConsumer> logger)
        {
            _logger = logger;
        }
        public virtual void Subscribe(ITweetStreamProducer producer)
        {
            // subscribe myself to the producer
            _unsubscriber = producer.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("Tweets from current stream have been consumed.");
        }

        public virtual void OnError(Exception error)
        {
            // Do nothing.
        }

        //public void ConfigureDestination(TweetDestinationPropertiesDelegate destinationPropertiesFunction)
        //{
        //    destinationPropertiesFunction(_destinationProperties);
        //}

        public virtual void OnNext(RawTweet tweetObj)
        {
            if (string.IsNullOrEmpty(tweetObj.TweetJsonString))
                return;

            _logger.LogInformation("Tweet received, processing...");

            JObject twitterData = JObject.Parse(tweetObj.TweetJsonString);
            _logger.LogInformation(twitterData["data"]["author_id"].ToString());

            // find all hastags nodes
            // this searches for all hashtags directly related to the tweet - data.entities.hashtag.
            // Note: there could be other hashtags in the other tweets included, but for now those are ignored
            string jsonPathHashTags =
                $"$.{TwitterJsonElements.TWITTER_JSON_ELEMENT_DATA}." +
                $"{TwitterJsonElements.TWITTER_JSON_ELEMENT_ENTITIES}." +
                $"{TwitterJsonElements.TWITTER_JSON_ELEMENT_HASHTAGS}[*]." +
                $"{TwitterJsonElements.TWITTER_JSON_ELEMENT_TAG}";

            var tags = twitterData.SelectTokens(jsonPathHashTags).ToList();
            string tag = string.Empty;
            if (tags != null && tags.Count() > 0)
            {
                _logger.LogInformation($"Num Hashtags: {tags.Count()}");
                foreach(JToken t in tags)
                {
                    tag = t.ToString().Replace("?","").Replace("!", "");
                    if (!string.IsNullOrEmpty(tag))
                    {
                        _logger.LogInformation(t.ToString());
                    }
                }
            }
            _logger.LogInformation("Done processing tweet.");
        }
    }
}

