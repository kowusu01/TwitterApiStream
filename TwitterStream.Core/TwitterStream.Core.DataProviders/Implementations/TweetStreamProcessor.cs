using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitterStream.Core.Interfaces;
using TwitterStream.Core.Models;
using TwitterStream.Core.Utils;

/// reference: https://learn.microsoft.com/en-us/dotnet/standard/events/how-to-implement-an-observer
namespace TwitterStream.Core.Implementations
{
    public   class TweetStreamProcessor : ITweetStreamProcessor
    {
        ILogger<TweetStreamProcessor> _logger;
        IDisposable? _unsubscriber;
        object? _destinationProperties;


        // use internal hashtable to keep track of tweet tag count
        Dictionary<string,int> _dictionaryHashTags = new System.Collections.Generic.Dictionary<string, int>();


        public TweetStreamProcessor(ILogger<TweetStreamProcessor> logger)
        {
            _logger = logger;
        }
        public virtual void Subscribe(ITweetStreamReader producer)
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
            try
            {
                if (string.IsNullOrEmpty(tweetObj.TweetJsonString))
                    return;

                _logger.LogInformation("Tweet received, processing...");

                JObject twitterData = JObject.Parse(tweetObj.TweetJsonString);

                var authorId = twitterData["data"]["author_id"].ToString();
                _logger.LogInformation($"author_id:{authorId}");

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
                    string tagsList = string.Join("", tags);
                    Console.WriteLine(tagsList);
                    _logger.LogInformation($"Num Hashtags: {tags.Count()}");
                    foreach (JToken t in tags)
                    {                    
                        // a bit of cleaning
                        // 1. trim trailing spaces if any
                        tag = t.ToString().Trim();

                        // 2. remove words that are just repeated chars like '!!!!!!!', '???????'", etc
                        tag = Regex.Replace(tag, @"[\?]*", "");
                        tag = Regex.Replace(tag, "[//!]*", "");
                        tag = Regex.Replace(tag, "[//#]*", "");
                        if (!string.IsNullOrEmpty(tag))
                        {
                            //_logger.LogInformation(tag);
                            AddTweetHashTag(tag);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"Something bad happened while processing a new tweet. \n {e.ToString()}");
            }
            _logger.LogInformation("Done processing tweet.");
        }

        public void AddTweetHashTag(string tag)
        {
            try
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    int someValue = 0;
                    if (_dictionaryHashTags.TryGetValue(tag, out someValue))
                    {
                        _dictionaryHashTags[tag] = (int)_dictionaryHashTags[tag] + 1;
                    }
                    else
                    {
                        _dictionaryHashTags[tag] = 1;
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"Error adding new hashtag [{tag}] \n {e.ToString()}");
            }
        }
        public IEnumerable<KeyValuePair<string, int>> GetTopNHashtags(int n)
        {
            if (n<1 || n > _dictionaryHashTags.Count())
            {
                throw new Exception($"Number of items to return [{n}] is not valid. Number must be greater than 0 and less than/equal to list size.");
            }
            // sort by the total count
            // grab top 10
           var result = _dictionaryHashTags.OrderByDescending(i => i.Value);
           return (result.Count() > n) ? result.Take(n) : result;
        }
    }

}

