using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShots
{
    public interface IBluetooth
    {
        void SetupBt();
        List<string> GetKnownDevices();
        void StartDiscoveringDevices();
        void StopDiscoveringDevices();
        void PairToDevice(string name, bool known);
        void ReceivePair();
        void EnableDiscoverable();
        void ReadMessage();
        void SendMessage(string message);

        bool GetMaster();
        void CancelReconnection();
    }
}
