using DevInPills.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.Speech
{
    public class GoToLastEpisode : ISpeechEngine
    {
        public Task<string> SpeakAsync(object[] parameter)
        {
            return Task.Run<string>(() =>
            {
                Messenger.Default.Send<GoToLastEpisodeMessage>(new GoToLastEpisodeMessage());
                return string.Empty;
            });
        }
    }
}