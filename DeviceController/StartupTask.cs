using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

namespace DeviceController
{
    public sealed class StartupTask : IBackgroundTask
    {
        static string connectionString = "<Device Primary Connection String - from IoT Hub>";
        DeviceClient client;
        GpioController deviceController;
        GpioPin greenPin;
        GpioPin redPin;
        GpioPinValue greenState;
        GpioPinValue redState;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            client = DeviceClient.CreateFromConnectionString(connectionString);
            deviceController = GpioController.GetDefault();
            if (deviceController != null)
            {
                greenPin = deviceController.OpenPin(18);
                greenPin.SetDriveMode(GpioPinDriveMode.Output);
                redPin = deviceController.OpenPin(27);
                redPin.SetDriveMode(GpioPinDriveMode.Output);

                greenState = GpioPinValue.High;
                redState = GpioPinValue.Low;
                greenPin.Write(greenState);
                redPin.Write(redState);
            }

            var deferral = taskInstance.GetDeferral();

            await ExecuteCommandAsync();

            deferral.Complete();
        }
        
        async Task ExecuteCommandAsync()
        {
            while (true)
            {
                Message command = await client.ReceiveAsync();

                if (command != null)
                {
                    string cmdText = Encoding.ASCII.GetString(command.GetBytes());
                    if (cmdText == "green")
                    {
                        greenPin.Write(GpioPinValue.High);
                        redPin.Write(GpioPinValue.Low);
                    }
                    else if (cmdText == "red")
                    {
                        greenPin.Write(GpioPinValue.Low);
                        redPin.Write(GpioPinValue.High);
                    }
                    await client.CompleteAsync(command.LockToken);
                }

            }
        }
    }
}
