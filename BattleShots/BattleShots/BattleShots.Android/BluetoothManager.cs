using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Util;
using Xamarin.Forms;

[assembly: Dependency(typeof(BattleShots.Droid.BluetoothManager))]
namespace BattleShots.Droid
{
    public class BluetoothManager : BroadcastReceiver, IBluetooth
    {
        public BluetoothSocket Socket { get; set; }
        public BluetoothServerSocket mmServerSocket { get; set; }
        public bool ReceivingConnection { get; set; }
        public BluetoothHeadset BtHeadset { get; set; }
        public BluetoothAdapter BtAdapter { get; set; }

        public List<BluetoothDevice> PairedDevices;

        private const int REQUEST_ENABLE_BT = 2;

        public List<string> Names = new List<string>();

        public Stream inputStream { get; set; }
        public Stream outputStream { get; set; }
        public bool Reading { get; set; }

        public bool Master { get; set; }

        public BluetoothManager()
        {
            BGData.btManager = this;
            BGData.bluetoothDevices = new List<BluetoothDevice>();
            ReceivingConnection = false;
            Reading = false;
        }

        public void SetupBt()
        {
            BtAdapter = BluetoothAdapter.DefaultAdapter;

            if (BtAdapter.IsEnabled == false)
            {
                TryEnableBluetooth();
            }
        }

        public void TryEnableBluetooth()
        {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            BGData.activity.StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        }

        public List<string> GetKnownDevices()
        {
            if (BtAdapter.IsEnabled)
            {
                PairedDevices = BtAdapter.BondedDevices.ToList();

                List<string> deviceNames = new List<string>();
                if (PairedDevices.Count > 0)
                {
                    // There are paired devices. Get the name and address of each paired device.
                    foreach (BluetoothDevice device in PairedDevices)
                    {
                        deviceNames.Add(device.Name);
                    }
                    return deviceNames;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                TryEnableBluetooth();
                return null;
            }
        }

        public void StartDiscoveringDevices()
        {
            if (BtAdapter.IsEnabled && !BtAdapter.IsDiscovering)
            {
                BGData.bluetoothDevices.Clear();

                BGData.activity.RegisterReceiver(this, new IntentFilter(BluetoothDevice.ActionFound));

                this.BtAdapter.StartDiscovery();
            }
        }

        public void StopDiscoveringDevices()
        {
            this.BtAdapter.CancelDiscovery();

            BGData.activity.UnregisterReceiver(this);

            Names.Clear();
        }

        public void EnableDiscoverable()
        {
            if (BtAdapter.IsEnabled)
            {
                Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                BGData.activity.StartActivityForResult(discoverableIntent, 300);
            }
            else
            {
                TryEnableBluetooth();
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;

            if (action == BluetoothDevice.ActionFound)
            {
                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

                if (!BGData.bluetoothDevices.Contains(device))
                {
                    BGData.bluetoothDevices.Add(device);
                    Names.Add(device.Name);
                    MainPage.DeviceNames = Names;
                }
            }
        }

        public async void PairToDevice(string name, bool known)
        {            
            if (BtAdapter.IsEnabled)
            {
                if (BtAdapter.IsDiscovering)
                {
                    BtAdapter.CancelDiscovery();
                }

                ReceivingConnection = false;

                BluetoothDevice Device = null;
                if (known)
                {
                    foreach (BluetoothDevice device in PairedDevices)
                    {
                        if (device.Name == name)
                        {
                            Device = device;
                        }
                    }
                }
                else
                {
                    foreach (BluetoothDevice device in BGData.bluetoothDevices)
                    {
                        if (device.Name == name)
                        {
                            Device = device;
                        }
                    }
                }

                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

                if ((int)Android.OS.Build.VERSION.SdkInt >= 10) // Gingerbread 2.3.3 2.3.4
                    Socket = Device.CreateInsecureRfcommSocketToServiceRecord(uuid);
                else
                    Socket = Device.CreateRfcommSocketToServiceRecord(uuid);

                if (Socket != null)
                {
                    try
                    {
                        await Socket.ConnectAsync();
                        Master = true;
                        ToastLoader toastLoader = new ToastLoader();
                        toastLoader.Show("Connected To Player");
                        SetupChat();
                        MainPage.StatSetupGame();
                    }
                    catch (Exception ex)
                    {
                        ToastLoader toastLoader = new ToastLoader();
                        toastLoader.Show(ex.Message);
                        ReceivingConnection = true;
                        ReceivePair();
                    }
                }
            }
        }
        public async void PairToDevice(BluetoothDevice Device)
        {
            if (BtAdapter.IsEnabled)
            {
                if (BtAdapter.IsDiscovering)
                {
                    BtAdapter.CancelDiscovery();
                }

                ReceivingConnection = false;

                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

                if ((int)Android.OS.Build.VERSION.SdkInt >= 10) // Gingerbread 2.3.3 2.3.4
                    Socket = Device.CreateInsecureRfcommSocketToServiceRecord(uuid);
                else
                    Socket = Device.CreateRfcommSocketToServiceRecord(uuid);

                if (Socket != null)
                {
                    try
                    {
                        await Socket.ConnectAsync();
                        Master = true;
                        BGData.CurrentDevice = Device;
                        
                        ToastLoader toastLoader = new ToastLoader();
                        toastLoader.Show("Connected To Player");
                        SetupChat();
                        if (BGStuff.Reconnecting)
                        {
                            if (BGStuff.settingUpGame)
                            {
                                await BGStuff.setupGame.Navigation.PopAsync();
                                BGStuff.Reconnecting = false;
                            }
                        }
                        else
                        {
                            MainPage.StatSetupGame();
                        }
                    }
                    catch (Exception ex)
                    {
                        ToastLoader toastLoader = new ToastLoader();
                        toastLoader.Show(ex.Message);
                        ReceivingConnection = true;
                        ReceivePair();
                    }
                }
            }
        }

        public void ReceivePair()
        {
            if (BtAdapter.IsEnabled && !ReceivingConnection)
            {
                BluetoothServerSocket tmp = null;
                try
                {
                    // MY_UUID is the app's UUID string, also used by the client code.
                    tmp = BtAdapter.ListenUsingRfcommWithServiceRecord("BattleShots", UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                }
                catch (Exception e)
                {
                    ToastLoader toastLoader = new ToastLoader();
                    toastLoader.Show(e.Message);
                }
                mmServerSocket = tmp;
                ReceivingConnection = true;

                CheckForPair();
            }
        }
        private async void CheckForPair()
        {
            await Task.Run(() =>
             {
                 BluetoothSocket socket = null;
                // Keep listening until exception occurs or a socket is returned.
                while (ReceivingConnection)
                 {
                     try
                     {
                         socket = mmServerSocket.Accept();

                         if (socket != null)
                         {
                             // A connection was accepted. Perform work associated with
                             // the connection in a separate thread.
                             //ManageMyConnectedSocket(socket);
                             Socket = socket;
                             Master = false;                             
                             ReceivingConnection = false;
                             mmServerSocket.Close();
                             BGData.CurrentDevice = socket.RemoteDevice;
                             SetupChat();                             
                             Device.BeginInvokeOnMainThread(() =>
                             {
                                 ToastLoader toast = new ToastLoader();
                                 toast.Show("Connected With Player");

                                 if (BGStuff.Reconnecting)
                                 {
                                     if (BGStuff.settingUpGame)
                                     {
                                         BGStuff.setupGame.Navigation.PopAsync();
                                         BGStuff.Reconnecting = false;
                                     }
                                 }
                                 else
                                 {
                                     MainPage.StatSetupGame();
                                 }
                             });
                         }
                     }
                     catch (Exception e)
                     {
                         ToastLoader toastLoader = new ToastLoader();
                         toastLoader.Show(e.Message);
                     }                     
                 }
             });
        }      
        
        public void SetupChat()
        {
            if(Socket.IsConnected)
            {
                try
                {
                    inputStream = Socket.InputStream;
                    outputStream = Socket.OutputStream;
                }
                catch(Exception ex)
                {
                    ToastLoader toastLoader = new ToastLoader();
                    toastLoader.Show(ex.Message);
                }
            }
        }
        public async void ReadMessage()
        {
            await Task.Run(() =>
            {
                try
                {
                    var buffer = new byte[1028];
                    int numOfBytes;
                    while (Socket.IsConnected)
                    {
                        numOfBytes = inputStream.Read(buffer, 0, buffer.Length);
                        if (numOfBytes > 0)
                        {
                            string message = Encoding.ASCII.GetString(buffer);
                            if (BGStuff.settingUpGame)
                            {                                
                                if (message.Contains(","))
                                {                                    
                                    string[] split;
                                    split = message.Split(',');

                                    if (split[1].Contains("g"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            SetupGame.StatSetPicker(split[0]);
                                        });
                                    }
                                    else if (split[1].Contains("s"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            SetupGame.StatSetNumOfShotsEntry(split[0]);
                                        });
                                    }
                                }
                                else
                                {
                                    SetupGame.StatGoToSetup2();
                                }
                                buffer = new byte[1028];
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToastLoader toast = new ToastLoader();
                        toast.Show("Connection Lost");
                        Reconnect();
                    });
                }
            });
        }

        public async void SendMessage(string message)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (Socket.IsConnected)
                    {
                        var buffer = new byte[message.Length];

                        buffer = Encoding.ASCII.GetBytes(message);

                        outputStream.Write(buffer, 0, message.Length);
                    }
                }
                catch(Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToastLoader toast = new ToastLoader();
                        toast.Show("Connection Lost");
                        Reconnect();
                    });
                }
            });
        }

        public bool GetMaster()
        {
            return Master;
        }

        public async void Reconnect()
        {
            BGStuff.Reconnecting = true;

            while (BGStuff.Reconnecting)
            {
                if (BGStuff.settingUpGame)
                {
                    await BGStuff.setupGame.Navigation.PushAsync(new ReconnectionPage());
                    Socket = null;
                    if (!Master)
                    {
                        ReceivePair();
                    }
                    else
                    {
                        await Task.Delay(100);
                        PairToDevice(BGData.CurrentDevice);
                    }
                }
            }
        }

        public void CancelReconnection()
        {
            try
            {
                BGStuff.Reconnecting = false;
                ReceivingConnection = false;
                Socket = null;

                if (BGStuff.settingUpGame)
                {
                    BGStuff.setupGame.Navigation.PopToRootAsync();
                }
            }
            catch(Exception ex)
            {
                if (BGStuff.settingUpGame)
                {
                    BGStuff.setupGame.Navigation.PopToRootAsync();
                }
            }
        }
    }
}