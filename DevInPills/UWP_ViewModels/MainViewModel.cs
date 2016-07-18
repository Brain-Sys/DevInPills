using DevInPills.DomainModels;
using DevInPills.Messages;
using DevInPills.Speech;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using DevInPills.ViewModels;

namespace DevInPills.UWP_ViewModels
{
    public class MainViewModel : DevInPills.ViewModels.MainViewModel
    {
        private SpeechSynthesizer speech = new SpeechSynthesizer();
        private SpeechRecognizer speechRecognizer;

        public MediaElement Media { get; set; }
        public RelayCommand ListenCommand { get; set; }
        public RelayCommand CancelListenCommand { get; set; }
        public RelayCommand<ItemClickEventArgs> NavigateToEpisodeCommand { get; set; }

        public MainViewModel()
        {
            if (!base.IsInDesignMode)
            {
                this.LoadVideoCompleted += MainViewModel_LoadVideoCompleted;
                this.ListenCommand = new RelayCommand(ListenCommandExecute);
                this.CancelListenCommand = new RelayCommand(CancelListenCommandExecute);
                this.InitializeRecognizer(SpeechRecognizer.SystemSpeechLanguage);
                NetworkInformation_NetworkStatusChanged(this);
                NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
                Messenger.Default.Register<GoToLastEpisodeMessage>(this, goToLastEpisode);
                Messenger.Default.Register<GoToEpisodeMessage>(this, goToEpisode);
            }
        }

        private async void goToEpisode(GoToEpisodeMessage obj)
        {
            Video v = this.Items.FirstOrDefault(i => i.Date.Day == obj.Date.Day &&
                i.Date.Month == obj.Date.Month && i.Date.Year == obj.Date.Year);

            if (v != null)
            {
                NavigateToPageMessage msg = new NavigateToPageMessage();
                msg.PageName = "EpisodePage";
                msg.Parameter = v;
                Messenger.Default.Send<NavigateToPageMessage>(msg);
            }
            else
            {
                var text = "Spiacente, in quella data non c'è alcuna puntata";
                var stream = await speech.SynthesizeTextToStreamAsync(text);
                this.Speak(stream);
            }
        }

        private void MainViewModel_LoadVideoCompleted(object sender, EventArgs e)
        {
            this.AddNewPhrases();
        }

        private void goToLastEpisode(GoToLastEpisodeMessage obj)
        {
            NavigateToPageMessage msg = new NavigateToPageMessage();
            msg.PageName = "EpisodePage";
            msg.Parameter = this.Items[0];
            Messenger.Default.Send<NavigateToPageMessage>(msg);
        }

        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            bool value = (profile != null
                && profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    new DispatchedHandler(() =>
                    {
                        this.IsConnected = value;

                        if (this.IsConnected && this.Items != null && !this.Items.Any())
                        {
                            this.LoadVideo(string.Empty);
                        }
                    }));
        }

        private async void ListenCommandExecute()
        {
            this.IsListeningAvailable = false;

            var recognitionOperation = speechRecognizer.RecognizeAsync();
            SpeechRecognitionResult speechRecognitionResult = await recognitionOperation;

            if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
            {
                if (speechRecognitionResult.Constraint != null && speechRecognitionResult.RawConfidence >= 0.55)
                {
                    this.Speak(speechRecognitionResult.Constraint.Tag, speechRecognitionResult.Text);
                }
            }
        }

        #region Voice and speech recognition management
        public async void AddNewPhrases()
        {
            try
            {
                List<string> commands1 = this.Items.Select(i => "episodio del " + i.Date.ToString("d MMMM")).ToList();
                List<string> commands2 = this.Items.Select(i => i.Date.ToString("d MMMM")).ToList();
                speechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(commands1, "episode"));
                speechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(commands2, "episode"));
                await speechRecognizer.CompileConstraintsAsync();
            }
            catch (Exception) { }
        }


        public async void InitializeRecognizer(Language recognizerLanguage)
        {
            try
            {
                speechRecognizer = new SpeechRecognizer(recognizerLanguage);

                speechRecognizer.Constraints.Add(
                    new SpeechRecognitionListConstraint(
                        new List<string>() { "ultima puntata" }, "lastepisode")
                        );

                SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

                if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                {
                    this.IsListeningAvailable = false;
                }
                else
                {
                    this.IsListeningAvailable = true;
                }
            }
            catch (Exception)
            {
                this.IsListeningAvailable = false;
            }
        }

        private async void CancelListenCommandExecute()
        {
            this.IsListeningAvailable = true;

            try
            {
                await speechRecognizer.StopRecognitionAsync();
            }
            catch (Exception)
            {

            }

            if (this.Media != null)
            {
                if (this.Media.CanPause)
                {
                    this.Media.Pause();
                }
            }
        }

        public void Speak(SpeechSynthesisStream stream)
        {
            try
            {
                this.Media.SetSource(stream, stream.ContentType);
                this.Media.Play();
            }
            catch (Exception)
            {

            }
        }

        public async void Speak(string tag, string spokenText)
        {
            string text = string.Empty;
            var answers = GetPossibleAnswers(tag);

            if (answers.Count == 1)
            {
                if (answers[0].StartsWith("DevInPills."))
                {
                    string typename = answers[0];
                    var engine = Activator.CreateInstance(Type.GetType(typename)) as ISpeechEngine;
                    text = await engine.SpeakAsync(new object[] { tag, spokenText });
                }
            }
            else
            {
                var rnd = new Random();
                text = answers[rnd.Next(0, answers.Count)];
            }

            if (string.IsNullOrEmpty(text))
            {
                var stream = await speech.SynthesizeTextToStreamAsync(text);
                this.Speak(stream);
            }
        }

        private List<string> GetPossibleAnswers(string tag)
        {
            List<string> result = new List<string>();
            string answer;
            int count = 1;

            ResourceLoader loader = new ResourceLoader();
            answer = loader.GetString(tag);

            if (!string.IsNullOrEmpty(answer))
            {
                result.Add(answer);
            }

            do
            {
                answer = loader.GetString(tag + (count++));

                if (!string.IsNullOrEmpty(answer))
                {
                    result.Add(answer);
                }
            }
            while (!string.IsNullOrEmpty(answer));

            return result;
        }
        #endregion
    }
}