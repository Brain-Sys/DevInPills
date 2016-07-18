using DevInPills.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.ViewModels
{
    public class EpisodeViewModel : ApplicationViewModelBase
    {
        private Video video;
        public Video Video
        {
            get { return video; }
            set
            {
                if (value != video)
                {
                    video = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public string Date
        {
            get
            {
                return this.Video.Date.ToString();
            }
        }
    }
}