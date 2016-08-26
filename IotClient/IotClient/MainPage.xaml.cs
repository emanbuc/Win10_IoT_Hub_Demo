﻿using System;
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

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IotClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RegistryManager registryManager;
        private DeviceClient deviceClient;
        private string iotHubUri = "{iot hub hostname}";
        private string deviceKey = "{device key}";
        private string deviceId = "mydevice";
        private string connectionString = "{iothub connection string}";

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();
            deviceKey = txtDeviceKey.Text;

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = TxtDeviceId.Text,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                AppendToConsole(String.Format("{0} > Sending message: {1}", DateTime.Now, messageString));
                

                Task.Delay(1000).Wait();
            }
        }

        private void AppendToConsole(string newMsg)
        {
            String s = newMsg + "\r\n";
            MessageTextBox.Text += s;
        }

        private  async Task AddDeviceAsync(string deviceId)
        {
            if (String.IsNullOrEmpty(deviceId))
                throw new NullReferenceException("DeviceId can not be null");

            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            txtDeviceKey.Text = device.Authentication.SymmetricKey.PrimaryKey;
            AppendToConsole(String.Format("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey));

        }

        private void OnNewDeviceBtnClick(object sender, RoutedEventArgs e)
        {
            connectionString = txtConnectionString.Text;
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync(TxtDeviceId.Text);
        }

        private void bntSendMessage_Click(object sender, RoutedEventArgs e)
        {
            iotHubUri = txtHubUri.Text;
            deviceKey = txtDeviceKey.Text;
            deviceId = TxtDeviceId.Text;
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));
            SendDeviceToCloudMessagesAsync();

        }
    }
}
