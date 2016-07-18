using DevInPills.DomainModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.ViewModels
{
    public class MonthlyVideo : ObservableObject
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value != date)
                {
                    date = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<Video> items;
        public ObservableCollection<Video> Items
        {
            get { return items; }
            set
            {
                if (value != items)
                {
                    items = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public string FormattedMonth
        {
            get
            {
                return this.Date.ToString("MMMM yyyy");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, ({1})", this.Date.ToString("MMMM"), this.Items.Count);
        }
    }
}