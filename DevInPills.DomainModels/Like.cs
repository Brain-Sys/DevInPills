using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.DomainModels
{
    public class Like
    {
        [JsonProperty(PropertyName = "data")]
        public List<LikeElement> Likes { get; set; }

        public Like()
        {
            this.Likes = new List<LikeElement>();
        }
    }
}