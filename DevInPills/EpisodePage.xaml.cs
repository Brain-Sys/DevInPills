using DevInPills.DomainModels;
using DevInPills.UWP_ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DevInPills
{
    public sealed partial class EpisodePage : Page
    {
        string family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
        public EpisodeViewModel ViewModel { get; set; }

        public EpisodePage()
        {
            this.InitializeComponent();
            this.ViewModel = this.Resources["vm"] as EpisodeViewModel;
            Window.Current.CoreWindow.CharacterReceived += CoreWindow_CharacterReceived;
        }

        private void CoreWindow_CharacterReceived(CoreWindow sender, CharacterReceivedEventArgs args)
        {
            if (args.KeyCode == 8)
            {
                args.Handled = true;

                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ItemClickEventArgs))
            {
                this.ViewModel.Video = ((ItemClickEventArgs)e.Parameter).ClickedItem as Video;
            }

            if (e.Parameter.GetType() == typeof(Video))
            {
                this.ViewModel.Video = e.Parameter as Video;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }

            e.Handled = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.CharacterReceived -= CoreWindow_CharacterReceived;
            SystemNavigationManager.GetForCurrentView().BackRequested -= MainPage_BackRequested;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.MainPage_BackRequested(this, null);
        }
    }
}