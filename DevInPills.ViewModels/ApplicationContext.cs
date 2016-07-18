using DevInPills.DomainModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.ViewModels
{
    public class ApplicationContext
    {
        string appId = "";
        string secretKey = "";
        string pageName = "BrainSys";
        string version = "2.6";
        string access_token = "";
        string request;
        int limit = 100;
        private HttpClient client;

        static ApplicationContext instance;

        public static ApplicationContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApplicationContext();
                }

                return instance;
            }
        }


        public string CacheFilename { get; private set; }
        public CacheManager CacheManager { get; private set; }

        private ApplicationContext()
        {
            this.CacheManager = new CacheManager(TimeSpan.FromSeconds(10));
            client = new HttpClient();
            this.CacheFilename = "feed.facebook.cache";
        }

        public async Task<VideosResponse> GetVideos()
        {
            VideosResponse result = new VideosResponse();

            try
            {
                access_token = getAppAccessToken();
                string fields = "id,description,updated_time,length,permalink_url,picture,source,likes{name}";
                var request = string.Format("https://graph.facebook.com/v{0}/{1}/videos?access_token={2}&fields={3}&limit={4}",
                    version, pageName, access_token, fields, limit);

                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, request);
                var response = await client.SendAsync(msg);
                var json = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<VideosResponse>(json);
            }
            catch(Exception)
            {

            }

            return result;
        }

        private string getAppAccessToken()
        {
            request = "https://graph.facebook.com/v2.6/oauth/access_token?client_id=" + appId + "&client_secret=" + secretKey + "&grant_type=client_credentials";
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, request);
            var response = client.SendAsync(msg).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            JObject r = JObject.Parse(json);

            return (string)r.GetValue("access_token");
        }
    }
}