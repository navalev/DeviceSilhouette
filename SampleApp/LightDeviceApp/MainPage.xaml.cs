﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.System.Threading;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LightDeviceApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool sensorActivated = false;
        CancellationTokenSource _sensorCancellationTokenSource = new CancellationTokenSource();

        public MainPage()
        {
            this.InitializeComponent();
           
            Task.Run(
                 async () => {
                     while (true)
                     {                         
                         var message = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
                         
                         await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {                             
                             proccesMessage(message);                             
                         });                         
                     }
                 }
                 );

        }

        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                while (true)
                {
                    reportState();
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
                
            });
        }

        private void proccesMessage(Message message)
        {
            if (message == null) return;
                     
            var messageData = Encoding.ASCII.GetString(message.GetBytes());
            incomingMessage.Text = "Last incoming message: " + messageData;            

            // TODO: add error handling
            string messageType = message.Properties["MessageType"];
            string messageSubType = message.Properties["MessageSubType"];

            if (messageType.Equals("CommandRequest"))
            {
                switch (messageSubType)
                {
                    case "SetState":
                        setState(messageData);
                        break;
                    case "LatestState":
                        break;
                    case "ReportState":
                        reportState();
                        break;
                }
            }
            else if (messageType.Equals("InquiryResponse") && messageSubType.Equals("GetState")) {
                reportState();
            }
        }

        private void reportState()
        {
            if (toggleSwitch != null)
            {
                DeviceMessage msg = new DeviceMessage(toggleSwitch.IsOn);
                string json = JsonConvert.SerializeObject(msg);
                AzureIoTHub.SendDeviceToCloudMessageAsync(json, "Report", "State");
                outgoingMessage.Text = "Last outgoing message: " + json;
            }
        }

        private void setState(string messageData)
        {
            // expect the message data to hold a state for the light - on or off
            // "{"status" : "on"}"           
            DeviceMessage msg = JsonConvert.DeserializeObject<DeviceMessage>(messageData);
            toggleSwitch.IsOn = msg.status.Equals("on");
        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                reportState();            
            }
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (!sensorActivated)
            {
                button.Content = "Stop Sensor";
                sensorActivated = true;
                _sensorCancellationTokenSource = new CancellationTokenSource();
                // simulate light sensor changing the light status every 5 minutes
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    while (!_sensorCancellationTokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        toggleSwitch.IsOn = !toggleSwitch.IsOn;
                    }
                });
            }
            else
            {
                button.Content = "Start Sensor";
                sensorActivated = false;
                _sensorCancellationTokenSource.Cancel();
            }
        }
      
    }

    internal class DeviceMessage
    {
        public string status;

        public DeviceMessage(bool isOn)
        {
            status = isOn ? "on" : "off";
        }
    }
}
