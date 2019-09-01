using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;

namespace BattleShots.Droid
{
    public class BGData
    {
        public static Activity activity { get; set; }
        public static BluetoothManager btManager { get; set; }
        public static List<BluetoothDevice> bluetoothDevices { get; set; }
        public static BluetoothDevice CurrentDevice { get; set; }
    }
}