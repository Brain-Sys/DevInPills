using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.DomainModels
{
    public class VideosResponse
    {
        [JsonProperty(PropertyName = "data")]
        public List<Video> Items { get; set; }

        public VideosResponse()
        {
            this.Items = new List<Video>();
        }
    }
}