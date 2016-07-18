using System;
using System.Threading.Tasks;

namespace DevInPills.Speech
{
    interface ISpeechEngine
    {
        Task<string> SpeakAsync(object[] parameter);
    }
}