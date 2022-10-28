/////////////////////////////////////////////////////////////////////
// 
// a console application that drives the twitter streaming
// - it assembles the pieces that:
//  1) connect to Twitter,
//  2) stream data and
//  3) in the future: make the data ready to other processes for further processing and analyis
// 
// https://goessner.net/articles/JsonPath/
// https://www.newtonsoft.com/json/help/html/SelectToken.htm
//
// Resources
// - observer pattern - https://learn.microsoft.com/en-us/dotnet/standard/events/how-to-implement-an-observer
// 
//
/////////////////////////////////////////////////////////////////////////


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TwitterStream.Core.Implementations;
using TwitterStream.Core.Interfaces;
using TwitterStream.Core.Utils;

Console.WriteLine("Welcome to Twitter streaming!");


try
{
    // create host and setup services
    IHost host = (IHost)Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (services) =>
        {
            services
            .AddSingleton<ITweetStreamReader, TweetStreamReader>()
            .AddSingleton<ITweetStreamProcessor, TweetStreamProcessor>();
        })
    .Build();

    IConfiguration _config = host.Services.GetService<IConfiguration>();

    // this should be available in the appsettings.congig
    string API_BASE_ADDRESS = _config[TwitterAPIConstants.API_BASE_URL];
    
    // this value must be add to the config
    string API_AUTH_BEARER_TOKEN = _config[TwitterAPIConstants.API_AUTH_BEARER_TOKEN];

     // get the producer instance
    ITweetStreamReader tweetReader = host.Services.GetService<ITweetStreamReader>();

    ITweetStreamProcessor rawTweetProcessor = host.Services.GetService<ITweetStreamProcessor>();

    // subscribe to the producer data
    tweetReader.Subscribe(rawTweetProcessor);

    // configure the producer
    Console.WriteLine("configuring http client twitter for stream producer...");
    tweetReader.ConfigureHttpClient(
            httpClient =>
            {
                httpClient.BaseAddress = new Uri(API_BASE_ADDRESS);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_AUTH_BEARER_TOKEN);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        );
    Console.WriteLine("done configuring http client for twitter stream.");

    try
    {
        // currently we just connect and read one stream
        // in the future, we may loop with a timer and run forever
        Console.WriteLine("beginning new streaming...");
        int totalTweets = await tweetReader.ReadStream();
        Console.WriteLine("Done with current streaming.");

        // print report
        Console.WriteLine("==============================================");
        Console.WriteLine($"Number of tweets: {totalTweets}");
        Console.WriteLine($"Top 10 hashtags");
        Console.WriteLine($"Rank\tFrequency\tHashtag");
        Console.WriteLine("-----------------------------------------------");
        var results = rawTweetProcessor.GetTopNHashtags(10);
        int count = 0;
        foreach(KeyValuePair<string, int> k in results)
        {
            Console.WriteLine($"{++count}\t{k.Value}\t\t{k.Key}");
        }
        Console.WriteLine("==============================================");
    }
    catch(Exception e)
    {
        Console.WriteLine($"Error processing tweets. \n {e.ToString()}");
    }
}
catch(Exception e)
{
    Console.WriteLine($"Something bad happend: \n {e.ToString()}" );
}
