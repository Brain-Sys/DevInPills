using DevInPills.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevInPills.Speech
{
    public class GoToEpisode : ISpeechEngine
    {
        public Task<string> SpeakAsync(object[] parameter)
        {
            return Task.Run<string>(() =>
            {
                string dateString = parameter[1].ToString().Replace("episodio del ", string.Empty);
                DateTime dt;
                bool result = DateTime.TryParseExact(dateString, "dd MMMM", new CultureInfo("it-IT"), DateTimeStyles.None, out dt);

                if (result)
                {
                    Messenger.Default.Send<GoToEpisodeMessage>(new GoToEpisodeMessage() { Date = dt });
                }

                return string.Empty;
            });
        }
    }
}