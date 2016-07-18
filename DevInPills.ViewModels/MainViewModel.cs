using DevInPills.DomainModels;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.ViewModels
{
    public class MainViewModel : ApplicationViewModelBase
    {
        private bool isListeningAvailable;
        public bool IsListeningAvailable
        {
            get { return isListeningAvailable; }
            set { isListeningAvailable = value; base.RaisePropertyChanged(); }
        }

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value; base.RaisePropertyChanged();
                base.RaisePropertyChanged(nameof(IsNotConnected));
                this.RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                base.RaisePropertyChanged();
                base.RaisePropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy
        {
            get
            {
                return !this.IsBusy;
            }
        }

        public bool IsNotConnected
        {
            get
            {
                return !this.IsConnected;
            }
        }

        private Video item;
        public Video Item
        {
            get { return item; }
            set
            {
                if (value != item)
                {
                    item = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        private Video lastEpisode;
        public Video LastEpisode
        {
            get { return lastEpisode; }
            set
            {
                lastEpisode = value;
                base.RaisePropertyChanged();
            }
        }

        public ObservableCollection<Video> Items { get; set; }
        public ObservableCollection<MonthlyVideo> MonthlyVideo { get; set; }

        public event EventHandler LoadVideoCompleted;
        public RelayCommand<string> RefreshCommand { get; set; }

        public MainViewModel()
        {
            this.RefreshCommand = new RelayCommand<string>(LoadVideo, LoadVideoCanExecute);
        }

        private bool LoadVideoCanExecute(string parameter)
        {
            return this.IsConnected == true;
        }

        public async virtual void LoadVideo(string parameter)
        {
            this.IsBusy = true;

            bool ignoreCache = string.IsNullOrEmpty(parameter);
            VideosResponse response;

            if (ignoreCache == false && await ApplicationContext.Instance.CacheManager.IsCacheAvailableAsync(ApplicationContext.Instance.CacheFilename))
            {
                response = await ApplicationContext.Instance.CacheManager.LoadAsync<VideosResponse>(ApplicationContext.Instance.CacheFilename);
            }
            else
            {
                response = await ApplicationContext.Instance.GetVideos();
                await ApplicationContext.Instance.CacheManager.SaveAsync<VideosResponse>(response,
                    ApplicationContext.Instance.CacheFilename);
            }

            this.Items = new ObservableCollection<Video>(response.Items);
            this.OrganizeItems();
            this.IsBusy = false;

            this.LoadVideoCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void OrganizeItems()
        {
            if (this.Items != null && this.Items.Any())
            {
                // Prepare the object for the last episode
                this.LastEpisode = this.Items[0];
                this.Items.RemoveAt(0);

                // Build the structure for grouping all the videos considering the month
                IEnumerable<MonthlyVideo> grouped = this.Items.GroupBy(
                    i => i.Month,
                    i => i,
                    (month, videoList) => new MonthlyVideo()
                    {
                        Date = month,
                        Items = new ObservableCollection<Video>(videoList)
                    });

                this.MonthlyVideo = new ObservableCollection<ViewModels.MonthlyVideo>(grouped);

                base.RaisePropertyChanged("Items");
                base.RaisePropertyChanged("MonthlyVideo");
            }
        }
    }
}