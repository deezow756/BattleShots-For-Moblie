using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BattleShots;

namespace BattleShots
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        BluetoothMag bluetoothMag;
        public static List<string> DeviceNames = new List<string>();

        List<BtDevice> KnownBtDevices = new List<BtDevice>();
        List<BtDevice> DiscoveredBtDevices = new List<BtDevice>();

        bool Scanning = false;
        List<string> prevDeviceNames;

        bool firstboot = false;

        public MainPage()
        {
            InitializeComponent();
            BGStuff.mainPage = this;
            Buttons.Add(btnKnownDevices);
            Buttons.Add(btnDiscoverable);
            Buttons.Add(btnStartScan);
            Labels.Add(txtTitle);
            Labels.Add(txtNewDevices);
            Labels.Add(txtPairedDevices);
            ApplyTheme();
            bluetoothMag = new BluetoothMag();
            bluetoothMag.SetupBt();
            DeviceNames = new List<string>();
            GetKnownDevices();
            StartScan();
            bluetoothMag.ReceivePair();
            firstboot = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(firstboot)
            {
                GetKnownDevices();
                StartScan();
                bluetoothMag.ReceivePair();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopScanning();
        }

        public void GetKnownDevices()
        {            
            lstPairedDevices.ItemsSource = null;
            if(KnownBtDevices != null)
                KnownBtDevices.Clear();
            List<string> knownDevices = bluetoothMag.GetKnowndevices();

            if (knownDevices != null)
            {
                if (knownDevices.Count > 0)
                {
                    for (int i = 0; i < knownDevices.Count; i++)
                    {
                        KnownBtDevices.Add(new BtDevice() { Name = knownDevices[i], TextColour = Theme.LabelTextColour });
                    }

                    lstPairedDevices.ItemsSource = KnownBtDevices;
                }
            }
        }

        private void BtnStartScan_Clicked(object sender, EventArgs e)
        {
            StartScan();
        }

        public async void StartScan()
        {
            if (Scanning)
            {
                btnStartScan.Text = "Start Scanning";

                StopScanning();
            }
            else
            {
                lstDiscoveredDevices.ItemsSource = null;
                DeviceNames = null;
                bluetoothMag.StartDiscoveringDevices();

                btnStartScan.Text = "Stop Scanning";
                Scanning = true;
                prevDeviceNames = DeviceNames;
                CheckForChanges();

                await Task.Delay(6000);
                if (Scanning)
                    StopScanning();
            }
        }
        
        public async void CheckForChanges()
        {
            if (Scanning)
            {
                if (prevDeviceNames != DeviceNames)
                {
                    DiscoveredBtDevices.Clear();
                    for (int i = 0; i < DeviceNames.Count; i++)
                    {
                        DiscoveredBtDevices.Add(new BtDevice() { Name = DeviceNames[i], TextColour = Theme.LabelTextColour });
                    }
                    lstDiscoveredDevices.ItemsSource = DiscoveredBtDevices;
                    prevDeviceNames = DeviceNames;
                }
                await Task.Delay(300);
                CheckForChanges();
            }
        } 
        
        public void StopScanning()
        {
            btnStartScan.Text = "Start Scanning";
            Scanning = false;
            prevDeviceNames = null;
            bluetoothMag.StopDiscoveringDevices();
        }

        private void BtnKnownDevices_Clicked(object sender, EventArgs e)
        {
            GetKnownDevices();
        }

        private void BtnDiscoverable_Clicked(object sender, EventArgs e)
        {
            bluetoothMag.EnableDiscoverable();
        }

        private void ViewCellKnown_Tapped(object sender, EventArgs e)
        {
            var vc = (ViewCell)sender;
            bluetoothMag.PairToDevice(vc.ClassId, true);
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            var vc = (ViewCell)sender;
            bluetoothMag.PairToDevice(vc.ClassId, false);
        }

        private void LstDiscoveredDevices_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstDiscoveredDevices.SelectedItem = null;
        }

        private void LstPairedDevices_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstPairedDevices.SelectedItem = null;
        }

        public List<Button> Buttons = new List<Button>();
        public List<Label> Labels = new List<Label>();

        public void ApplyTheme()
        {
            this.BackgroundColor = Theme.BgColour;
            for (int i = 0; i < Labels.Count; i++)
            {
                Labels[i].TextColor = Theme.LabelTextColour;
            }

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].TextColor = Theme.ButtonTextColour;
                Buttons[i].BackgroundColor = Theme.ButtonBgColour;
                Buttons[i].BorderColor = Theme.ButtonBorderColour;
            }
        }

        public static void StatSetupGame()
        {
            BGStuff.mainPage.SetupGame();
        }
        public void SetupGame()
        {
            Navigation.PushAsync(new SetupGame(bluetoothMag));
        }
    }
}
