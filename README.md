# Twitter Api Streaming
 Simple app to demo how Twitter API can be streamed and analysed

Executing the app
- pull the repo to local file system
- on a command line, navigate to the root folder  - the TwitterStream folder
- navigate to one of the the drivers' folder.   
  e.g.: .\TwitterStream\TwitterStream.Drivers\TwitterStream.Drivers.DataProvider

- the app requires api key.
  - in the folder, you will find a config file: *appsettings.config.Production*  
  - open the file and edit it; replace the entire ```"{API_TOKEN_HERE}"``` with your api key
  - after edit, the config should look like:  ```  "apiAuthBearer": "AAAAAAAAAsomethingthingsomethingandotherthings" ```
  - save the file and exit.

- on a command line, in the same folder, issue the following command:

``` dotnet run --environment Production  ```




