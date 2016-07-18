using System;
using DevInPills.Messages;
using DevInPills.UWP_ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.System;
using Windows.UI.Core;

namespace DevInPills
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = this.Resources["vm"] as MainViewModel;
            this.ViewModel.Media = this.SpeechPlayer;

            this.Loaded += (s, e) =>
            {
                this.ViewModel.LoadVideo("use_cache");
            };

            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.CharacterReceived += CoreWindow_CharacterReceived;

            this.SpeechPlayer.MediaEnded += (s, e) => { this.ViewModel.IsListeningAvailable = true; };
            this.SpeechPlayer.MediaFailed += (s, e) => { this.ViewModel.IsListeningAvailable = true; };
            this.SpeechPlayer.PartialMediaFailureDetected += (s, e) => { this.ViewModel.IsListeningAvailable = true; };
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            bool cond1 = args.VirtualKey == VirtualKey.F5;
            bool cond2 = Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftControl) == CoreVirtualKeyStates.Down && args.VirtualKey == VirtualKey.R;
            bool cond3 = Window.Current.CoreWindow.GetKeyState(VirtualKey.RightControl) == CoreVirtualKeyStates.Down && args.VirtualKey == VirtualKey.R;

            if (cond1 || cond2 || cond3)
            {
                this.ViewModel.RefreshCommand.Execute(null);
            }
        }

        private void CoreWindow_CharacterReceived(CoreWindow sender, CharacterReceivedEventArgs args)
        {
            args.Handled = true;
            int number = (int)args.KeyCode - 48;

            if (number >= 1 && number <= 9)
            {
                NavigateToPageMessage msg = new NavigateToPageMessage();
                msg.PageName = "EpisodePage";
                msg.Parameter = this.ViewModel.Items[number - 1];

                Messenger.Default.Send<NavigateToPageMessage>(msg);
            }
        }
    }
}