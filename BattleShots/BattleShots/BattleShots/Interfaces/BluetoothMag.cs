using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BattleShots
{
    public class BluetoothMag
    {
        public BluetoothMag()
        {

        }
        public void SetupBt()
        {
            DependencyService.Get<IBluetooth>().SetupBt();
        }
        public List<string> GetKnowndevices()
        {
            return DependencyService.Get<IBluetooth>().GetKnownDevices();
        }

        public void StartDiscoveringDevices()
        {
            DependencyService.Get<IBluetooth>().StartDiscoveringDevices();
        }

        public void StopDiscoveringDevices()
        {
            DependencyService.Get<IBluetooth>().StopDiscoveringDevices();
        }

        public void EnableDiscoverable()
        {
            DependencyService.Get<IBluetooth>().EnableDiscoverable();
        }

        public void PairToDevice(string name, bool known)
        {
            DependencyService.Get<IBluetooth>().PairToDevice(name, known);
        }

        public void ReceivePair()
        {
            DependencyService.Get<IBluetooth>().ReceivePair();
        }

        public void ReadMessage()
        {
            DependencyService.Get<IBluetooth>().ReadMessage();
        }

        public void SendMessage(string message)
        {
            DependencyService.Get<IBluetooth>().SendMessage(message);
        }

        public bool GetMaster()
        {
            return DependencyService.Get<IBluetooth>().GetMaster();
        }

        public void CancelReconnection()
        {
            DependencyService.Get<IBluetooth>().CancelReconnection();
        }

        public string GetConnectedDeviceName()
        {
            return DependencyService.Get<IBluetooth>().GetConnectedDeviceName();
        }
    }
}
