using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace DevInPills.Triggers
{
    public class NetworkConnectionTrigger : StateTriggerBase
    {
        public NetworkConnectionTrigger()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;
        }

        private async void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
                bool internetAvailable = (profile != null && profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);

                SetActive(!internetAvailable);
            });
        }
    }
}