using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.DomainModels
{
    public class Video
    {
        private int size = 40;

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "updated_time")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "length")]
        public double Seconds { get; set; }

        [JsonProperty(PropertyName = "permalink_url")]
        public string VideoUrl { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string PictureUrl { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "likes")]
        public Like LikeStatus { get; set; }

        public string Duration
        {
            get
            {
                var ts = TimeSpan.FromSeconds(this.Seconds);
                string formatted = (DateTime.Today + ts).ToString("m:ss");
                return formatted;
            }
        }

        public string FirstPartDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Description) && this.Description.Length > 40)
                {
                    int index = this.Description.IndexOf(' ', size);
                    return this.Description.Substring(0, index);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string SecondPartDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Description) && this.Description.Length > 40)
                {
                    int index = this.Description.IndexOf(' ', size);
                    return this.Description.Substring(index + 1);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string FormattedDate
        {
            get
            {
                return this.Date.ToString("dd MMMM");
            }
        }

        public string DaysAgo
        {
            get
            {
                int days = (int)DateTime.Today.Subtract(this.Date).TotalDays;

                if (days > 0)
                {
                    return string.Format("{0} giorni fa", days);
                }
                else
                {
                    return "oggi";
                }
            }
        }

        public DateTime Month
        {
            get
            {
                return new DateTime(this.Date.Year, this.Date.Month, 01);
            }
        }

        public int? LikesCount
        {
            get
            {
                return this.LikeStatus?.Likes.Count();
            }
        }

        public override string ToString()
        {
            return this.VideoUrl;
        }
    }
}