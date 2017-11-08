using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace DeviceController
{
    public sealed class StartupTask : IBackgroundTask
    {
        static string connectionString = "HostName=INetHub.azure-devices.net;DeviceId=IoTPi;SharedAccessKey=xK4k7QihqslhUNh6VEiEy41903AX0NmmMcLjPSCbZuM=";
        DeviceClient client;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            client = DeviceClient.CreateFromConnectionString(connectionString);
            var deferral = taskInstance.GetDeferral();

            deferral.Complete();
        }

        private async Task ExecuteCommandAsync()
        {
            while (true)
            {
                Message command = await client.ReceiveAsync();
                //command.SequenceNumber
                if (command != null)
                {
                    //process command 
                    await client.CompleteAsync(command.LockToken);
                }
            }
        }
    }
}
