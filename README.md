# Twitter Api Streaming
 Simple app to demo how Twitter API can be streamed and analysed.

## intoduction
The application consumes tweets from Twitter api. It connects to the V2 of the twitter streaming api that serves sample tweets - https://api.twitter.com/2/tweets/sample/stream.

When a streaming channel is opned between the app and Twitter, the app can receive about 1000 tweets per second, based on sample runs. There is a limnit on how mnay times the appi can be called withing a specific time frame.

## how the app is constructed
The app is desaigned to be able to handle large amount of data and process without delay. The high level architecture contains two main logical components:
- ### Data Produces
- ### Data Consumers

### data providers
Data Providers interact with Twitter api using the Observer Pattern subscription to read data and make it available to be processed.
Within the Data Providers logical component, there is a component that reads the stream; the data is not stored, rather, it passes it to another component to parse it and extract relavant information. The long term goal is to make the entire Data Producer logical component a Kafka producer by allowing it to publidh each tweet as a kafka topic so other independet app/services can consume it

### data consumers
The Data Consumers is an example pf an independent process that consumes data as Kafka topics published by the Data Providers. These data consumers can be written any lanuguage; the intent is to allow consumers to determine how to process tweet data and where or how to store it. 

The application architecure emphasizes on this separation of concerns to achive scalability and performance. For instance, the task of processing the tweets can be cpu and memory intensive, by making the cosumers independent, several instances can be deployed each performing different type of processing and analysis.

## high level architecture
 ![app structure](https://github.com/kowusu01/TwitterApiStream/blob/main/images/high-level-arch.jpg?raw=true)

## executing the app
- pull the repo to local file system
- on a command line, navigate to the root folder  - the TwitterStream folder
- navigate to one of the the drivers' folder.   
  e.g.: .\TwitterStream\TwitterStream.Drivers\TwitterStream.Drivers.DataProvider

- the app requires api key.
  - in the folder, you will find a config file: *appsettings.config.Production*  
  - open the file and edit it; replace the entire ```"{API_TOKEN_HERE}"``` with your api key
  - save the file and exit.

- on a command line, in the same folder, issue the following command:

``` dotnet run --environment Production  ```

