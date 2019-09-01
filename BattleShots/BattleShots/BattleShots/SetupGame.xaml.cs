using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BattleShots
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupGame : ContentPage
    {
        public BluetoothMag bluetooth { get; set; }
        public bool Master { get; set; }
        public const int MaxNumOfShots = 10;

        public SetupGame(BluetoothMag bluetoothMag)
        {
            InitializeComponent();
            bluetooth = bluetoothMag;
            BGStuff.setupGame = this;
            BGStuff.settingUpGame = true;
            Master = bluetoothMag.GetMaster();
            if(!Master)
            {
                entryNumOfShots.IsReadOnly = true;
                btnContinue.IsEnabled = false;
                pickerSizeOfBoard.IsEnabled = false;
            }
            Labels.Add(txtTitle);
            Labels.Add(txtSizeOfGrid);
            Labels.Add(txtNumOfShots);
            Entries.Add(entryNumOfShots);
            Buttons.Add(btnContinue);
            Pickers.Add(pickerSizeOfBoard);
            ApplyTheme();
            bluetoothMag.ReadMessage();
        }

        private void PickerSizeOfBoard_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SizeOfGrid = 0;
            
            switch(pickerSizeOfBoard.SelectedIndex)
            {
                case 0:
                    SizeOfGrid = 6;
                    break;
                case 1:
                    SizeOfGrid = 8;
                    break;
                case 2:
                    SizeOfGrid = 10;
                    break;
                default:
                    SizeOfGrid = 6;
                    break;
            }

            bluetooth.SendMessage(SizeOfGrid + "," + "g");
        }

        public static void StatSetPicker(string SizeOfGrid)
        {
            BGStuff.setupGame.SetPicker(SizeOfGrid);
        }

        public void SetPicker(string SizeOfGrid)
        {
            switch(SizeOfGrid)
            {
                case "6":
                    pickerSizeOfBoard.SelectedIndex = 0;
                    break;
                case "8":
                    pickerSizeOfBoard.SelectedIndex = 1;
                    break;
                case "10":
                    pickerSizeOfBoard.SelectedIndex = 2;
                    break;
                default:
                    pickerSizeOfBoard.SelectedIndex = -1;
                    break;
            }
        }

        private void EntryNumOfShots_TextChanged(object sender, TextChangedEventArgs e)
        {
            string stringTemp = entryNumOfShots.Text;
            if (string.IsNullOrEmpty(stringTemp) || stringTemp == "")
            { stringTemp = ""; }
            int intTemp;
            if (stringTemp != "")
            {
                try
                {
                    intTemp = int.Parse(stringTemp);
                    if (intTemp <= MaxNumOfShots)
                    {
                        bluetooth.SendMessage(intTemp.ToString() + ",s");
                    }
                    else
                    {
                        entryNumOfShots.Text = "";
                        ToastManager.Show("Cannot Enter More Than " + MaxNumOfShots.ToString() + " Shots");
                    }
                }
                catch (Exception ex)
                {
                    entryNumOfShots.Text = "";
                    ToastManager.Show("Number Of Shots Needs To Be A Number");
                }
            }
            else
            {
                bluetooth.SendMessage(",s");
            }
        }


        public static void StatSetNumOfShotsEntry(string _numOfShots)
        {
            BGStuff.setupGame.SetNumOfShotsEntry(_numOfShots);
        }
        public void SetNumOfShotsEntry(string NumOfShots)
        {
            entryNumOfShots.Text = NumOfShots;
        }

        public static void StatGoToSetup2()
        {
            BGStuff.setupGame.GoToSetup2();
        }

        private void BtnContinue_Clicked(object sender, EventArgs e)
        {
            bluetooth.SendMessage("Setup2");
            GoToSetup2();
        }

        public void GoToSetup2()
        {
            Navigation.PushAsync(new SetupGame2(bluetooth));
        }

        public void Reconnect()
        {
            Navigation.PushAsync(new ReconnectionPage(bluetooth));
        }

        #region Theme Stuff
        public List<Button> Buttons = new List<Button>();
        public List<Label> Labels = new List<Label>();
        public List<Entry> Entries = new List<Entry>();
        public List<Picker> Pickers = new List<Picker>();
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

            for (int i = 0; i < Entries.Count; i++)
            {
                Entries[i].TextColor = Theme.EntryTextColour;
                Entries[i].PlaceholderColor = Theme.EntryPlaceholderColour;
            }
            for (int i = 0; i < Pickers.Count; i++)
            {
                Pickers[i].TextColor = Theme.PickerTextColour;
                Pickers[i].TitleColor = Theme.PickerTitleColour;
                Pickers[i].BackgroundColor = Theme.PickerBgColour;
            }
        }
        #endregion
       
    }
}