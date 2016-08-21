using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IotHubSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {        
        public MainPage()
        {
            this.InitializeComponent();
            ReceiveDataFromAzure();
            SendDataToAzure("Application startup");
        }

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            this.HelloMessage.Text = "Hello, Windows 10 IoT Core!";
            // send message to IOT Hub
            SendDataToAzure("Button clicked");
        }

        private async Task SendDataToAzure(String textMessage)
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(HubConfig.IOT_HUB_CONNECTION_STRING, TransportType.Http1);

            var msg = new Message(Encoding.UTF8.GetBytes(textMessage));

            await deviceClient.SendEventAsync(msg);
        }

        public async Task ReceiveDataFromAzure()
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(HubConfig.IOT_HUB_CONNECTION_STRING, TransportType.Http1);

            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());


                    // Report the change to the GUI (async)
                    var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        TxtReceivedMsg.Text = messageData;
                    });

                    
                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }
    }
}
