namespace TwitterStream.Core.Utils
{
    
    public class TwitterAPIConstants
    {
        public static string API_AUTH_BEARER_TOKEN = "ApiAuthBearer";

        public static string API_BASE_URL = "twitterApiBaseUrl";
        public static string API_PARAMETERS_TWEET_FIELDS = "tweet.fields=author_id,context_annotations,created_at,entities,id,source,text";
        public static string API_PARAMETERS_USER_FIELDS = "user.fields=created_at,entities,id,name,username";
        public static string API_PARAMETERS_EXPANSIONS = "expansions=author_id";
    }

    public class TwitterJsonElements
    {
        public static string TWITTER_JSON_ELEMENT_DATA = "data";
        public static string TWITTER_JSON_ELEMENT_ENTITIES = "entities";
        public static string TWITTER_JSON_ELEMENT_HASHTAGS = "hashtags";
        public static string TWITTER_JSON_ELEMENT_TAG = "tag";

    }

    public class DBConstants
    {
        /// <summary>
        /// the connection string template has the form:
        /// Host={0};Database={1};Username={2};Password={3}"
        /// </summary>
        public static readonly string ConnectionStringTemplate = "Host={0};Database={1};Username={2};Password={3}";
        
        public static readonly string DBServer = "DBServer";
        public static readonly string DBInstance = "DBInstance";
        public static readonly string DBUser = "DBUsername";
        public static readonly string DBPassword = "DBPassword";
    }

}