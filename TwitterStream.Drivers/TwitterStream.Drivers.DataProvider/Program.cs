/////////////////////////////////////////////////////////////////////
// 
// a console application that drives the twitter streaming
// - it assembles the pieces that:
//  1) connect to Twitter,
//  2) stream data and
//  3) make the data ready to other processes for further processing and analyis
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
using TwitterStream.Core.Implementations;
using TwitterStream.Core.Interfaces;
using TwitterStream.Core.Utils;

Console.WriteLine("Welcome to Twitter streaming!");

try
{
    IHost host = (IHost)Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (services) =>
        {
            services
            .AddSingleton<ITweetStreamProducer, TweetStreamProducer>()
            .AddSingleton<ITweetStreamConsumer, TweetStreamConsumer>();
        })
    .Build();

    IConfiguration _config = host.Services.GetService<IConfiguration>();
    
    string API_AUTH_BEARER_TOKEN = _config[TwitterAPIConstants.API_AUTH_BEARER_TOKEN];
    string API_BASE_ADDRESS = _config[TwitterAPIConstants.API_BASE_URL];

     // get the producer instance
    ITweetStreamProducer tweetProducer = host.Services.GetService<ITweetStreamProducer>();

    ITweetStreamConsumer rawTweetConsumer = host.Services.GetService<ITweetStreamConsumer>();

    // subscrib to the producer data
    rawTweetConsumer.Subscribe(tweetProducer);

    // configure the producer
    Console.WriteLine("configuring http client twitter for stream producer...");
    tweetProducer.ConfigureHttpClient(
            httpClient =>
            {
                httpClient.BaseAddress = new Uri(API_BASE_ADDRESS);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_AUTH_BEARER_TOKEN);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        );
    Console.WriteLine("done configuring http client for twitter stream.");

    while (true)
    {
        Console.WriteLine("beginning new streaming...");
        int totalTweets = await tweetProducer.ReadStream();
        Console.WriteLine("Done with current streaming.");
    }
}
catch(Exception e)
{
    Console.WriteLine($"Something bad happend: \n {e.ToString()}" );
}

//_logger.LogInformation("all streaming done.");
//Console.ReadKey();
