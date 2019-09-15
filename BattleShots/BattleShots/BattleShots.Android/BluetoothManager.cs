﻿using System;
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
        public bool BtBeingEnabled { get; set; }
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
            BtBeingEnabled = false;
        }

        public void SetupBt()
        {
            BtAdapter = BluetoothAdapter.DefaultAdapter;

            if (BtAdapter.IsEnabled == false)
            {
                if (!BtBeingEnabled)
                {
                    BtBeingEnabled = true;
                    TryEnableBluetooth();                    
                }
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
                if (!BtBeingEnabled)
                {
                    BtBeingEnabled = true;
                    TryEnableBluetooth();
                }
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
            else
            {
                if (!BtBeingEnabled)
                {
                    BtBeingEnabled = true;
                    TryEnableBluetooth();
                }
            }
        }

        public void StopDiscoveringDevices()
        {
            if (BtAdapter.IsEnabled)
            {
                BtAdapter.CancelDiscovery();
                try
                {
                    BGData.activity.UnregisterReceiver(this);
                    Names.Clear();
                }
                catch(Exception ex)
                {
                    Names.Clear();
                }
            }
            if (!BtBeingEnabled)
            {
                BtBeingEnabled = true;
                TryEnableBluetooth();
            }
        }

        public void EnableDiscoverable()
        {
            if (BtAdapter.IsEnabled)
            {
                Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                BGData.activity.StartActivityForResult(discoverableIntent, 300);
            }
            if (!BtBeingEnabled)
            {
                BtBeingEnabled = true;
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

                FileManager file = new FileManager();
                file.SaveDevice(name);

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

        public bool Pairing = false;
        public void PairToDevice()
        {
            Task.Run(async () =>
            {
                
                if (BtAdapter.IsEnabled)
                {
                    if (BtAdapter.IsDiscovering)
                    {
                        BtAdapter.CancelDiscovery();
                    }
                    List<BluetoothDevice> devices = BtAdapter.BondedDevices.ToList();
                    BluetoothDevice tempDevice = null;
                    string tempstring = "";
                    FileManager file = new FileManager();
                    tempstring = file.GetDevice();
               
                    await Task.Delay(300);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToastLoader toast = new ToastLoader();
                        toast.Show(tempstring);
                    });
                    await Task.Delay(500);
                    for (int i = 0; i < devices.Count; i++)
                    {
                        if(tempstring == devices[i].Name)
                        {
                            tempDevice = devices[i];
                        }
                    }

                    ReceivingConnection = false;
                    UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
                    BluetoothSocket temp = null;
                    try
                    {
                        if ((int)Android.OS.Build.VERSION.SdkInt >= 10) // Gingerbread 2.3.3 2.3.4
                            temp = tempDevice.CreateInsecureRfcommSocketToServiceRecord(uuid);
                        else
                            temp = tempDevice.CreateRfcommSocketToServiceRecord(uuid);

                        Socket = temp;
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ToastLoader toastLoader = new ToastLoader();
                            toastLoader.Show(ex.Message);
                        });
                    }

                    if (Socket != null)
                    {
                        try
                        {
                            await Socket.ConnectAsync();
                            Master = true;
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ToastLoader toastLoader = new ToastLoader();
                                toastLoader.Show("Reconnected To Player");
                            });
                            SetupChat();
                            if (BGStuff.Reconnecting)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    BGStuff.reconnectionPage.Navigation.PopAsync();
                                });
                                await Task.Delay(500);
                                BGStuff.Reconnecting = false;
                                Pairing = false;
                                BGStuff.reconnectionPage = null;
                                ReadMessage();
                            }
                            else
                            {
                                MainPage.StatSetupGame();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!BGStuff.Reconnecting)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ToastLoader toastLoader = new ToastLoader();
                                    toastLoader.Show(ex.Message);
                                });
                                    ReceivingConnection = true;
                                    ReceivePair();                                
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    ToastLoader toastLoader = new ToastLoader();
                                    toastLoader.Show(ex.Message);
                                });
                            }
                            
                        }
                    }
                }
            });
        }

        public void ReceivePair()
        {
            Task.Run(() =>
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
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ToastLoader toastLoader = new ToastLoader();
                            toastLoader.Show(e.Message);
                        });
                    }
                    mmServerSocket = tmp;
                    ReceivingConnection = true;

                    CheckForPair();
                }
            });
        }
        private void CheckForPair()
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

                        ReceivingConnection = false;
                        Socket = socket;
                        Master = false;
                        mmServerSocket.Close();
                        FileManager file = new FileManager();
                        file.SaveDevice(Socket.RemoteDevice.Name);
                        SetupChat();

                        if (BGStuff.Reconnecting)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ToastLoader toast = new ToastLoader();
                                toast.Show("Reconnected With Player");
                            });

                            Device.BeginInvokeOnMainThread(() =>
                                {
                                    BGStuff.reconnectionPage.Navigation.PopAsync();
                                });
                            BGStuff.Reconnecting = false;
                            ReadMessage();
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MainPage.StatSetupGame();
                                ToastLoader toast = new ToastLoader();
                                toast.Show("Connected With Player");
                            });
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToastLoader toastLoader = new ToastLoader();
                        toastLoader.Show(e.Message);
                    });
                }
            }
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
                    toastLoader.Show("Failed to SetupChat");
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
                                    else if (split[1].Contains("n"))
                                    {
                                         Device.BeginInvokeOnMainThread(() =>
                                        {
                                            SetupGame.StatSetEnemyName(split[0]);
                                        });
                                    }
                                }
                                else if(message.Contains("resume"))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        BGStuff.setupGame.ReceiveResume();
                                    });
                                }
                                else if (message.Contains("accept"))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        BGStuff.setupGame.AcceptResume();
                                    });
                                }
                                else if (message.Contains("reject"))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        BGStuff.setupGame.RejectResume();
                                    });
                                }
                                else if(message.Contains("Setup2"))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        SetupGame.StatGoToSetup2();
                                    });
                                }                               
                            }
                            else if(BGStuff.settingUpGame2)
                            {
                                if (message.Contains("ready"))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        BGStuff.setUpGame2.SetEnemyReady(true);
                                    });
                                }
                                else if(message.Contains("unready"))
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        BGStuff.setUpGame2.SetEnemyReady(false);
                                    });
                                }
                            }
                            else if(BGStuff.InGame)
                            {
                                if (message.Contains(","))
                                {
                                    string[] split = message.Split(',');

                                    if (split[1].Contains("gofirst"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BGStuff.game.GoesFirst(int.Parse(split[0]));
                                        });
                                    }
                                    else
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BGStuff.game.ReceiveCheck(message);
                                        });
                                    }
                                }
                                else
                                {
                                    if (message.Contains("hit"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BGStuff.game.ReceiveHit(true);
                                        });
                                    }
                                    else if (message.Contains("miss"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BGStuff.game.ReceiveHit(false);
                                        });
                                    }
                                    else if (message.Contains("ready"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BGStuff.game.Ready();
                                        });
                                    }
                                    else if (message.Contains("endgame"))
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            BGStuff.game.EndGame();
                                        });
                                    }
                                }
                            buffer = new byte[1028];
                        }
                    }
                    Reconnect();
                }
                catch(Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ToastLoader toast = new ToastLoader();
                        toast.Show("Connection Lost");

                        if(BGStuff.settingUpGame)
                        {
                            BGStuff.setupGame.Reconnect();
                        }
                        else if(BGStuff.settingUpGame2)
                        {
                            BGStuff.setUpGame2.Reconnect();
                        }
                        else if(BGStuff.InGame)
                        {
                            BGStuff.game.Reconnect();
                        }
                        
                    });
                    Reconnect();
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

        public void Reconnect()
        {
            Task.Run(async () =>
            {
                BGStuff.Reconnecting = true;

                while (BGStuff.Reconnecting)
                {      
                    if (!BtAdapter.IsEnabled)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            TryEnableBluetooth();
                        });
                        await Task.Delay(5000);
                    }
                    else
                    {                        
                        if (!Master)
                        {
                            if (!ReceivingConnection)
                            {
                                Socket.Close();
                                Socket.Dispose();
                                Socket = null;
                                ReceivePair();
                            }
                        }
                        else
                        {
                            if (!Pairing)
                            {
                                Socket.Close();
                                Socket.Dispose();
                                Socket = null;
                                PairToDevice();                                
                            }
                            await Task.Delay(2000);
                        }
                    }
                }
            });
        }

        public void CancelReconnection()
        {
            Task.Run(() =>
            {
                try
                {
                    BGStuff.Reconnecting = false;
                    ReceivingConnection = false;
                    Socket = null;

                    Device.BeginInvokeOnMainThread(() =>
                        {
                            BGStuff.reconnectionPage.Navigation.PopToRootAsync();
                        });

                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() =>
                        {
                            BGStuff.reconnectionPage.Navigation.PopToRootAsync();
                        });

                }
            });
        }

        public string GetConnectedDeviceName()
        {
            return Socket.RemoteDevice.Name;
        }

        public string GetDeviceName()
        {
            return BtAdapter.Name;
        }
    }
}