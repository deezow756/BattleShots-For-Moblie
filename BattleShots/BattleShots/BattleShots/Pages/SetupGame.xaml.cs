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
        public GameSettings settings { get; set; }

        public const int MaxNumOfShots = 10;

        public SetupGame(BluetoothMag bluetoothMag)
        {
            InitializeComponent();
            bluetooth = bluetoothMag;
            Setup();
        }
        public void Setup()
        {
            BGStuff.setupGame = this;
            BGStuff.settingUpGame = true;
            settings = new GameSettings();
            bluetooth.ReadMessage();
            settings.Master = bluetooth.GetMaster();
            settings.ConnectedDeviceName = bluetooth.GetConnectedDeviceName();
            FileManager file = new FileManager();
            if (settings.Master)
            {
                //if (file.CheckForExistingGame(settings.ConnectedDeviceName))
                //{                    
                //    ResumeGame();
                //}
            }
            else
            {
                entryNumOfShots.IsReadOnly = true;
                btnContinue.IsEnabled = false;
                pickerSizeOfBoard.IsEnabled = false;
            }
            Labels.Add(txtEnterName);
            Labels.Add(txtTitle);
            Labels.Add(txtSizeOfGrid);
            Labels.Add(txtNumOfShots);
            Entries.Add(entryNumOfShots);
            Entries.Add(entName);
            Buttons.Add(btnContinue);
            Pickers.Add(pickerSizeOfBoard);
            ApplyTheme();
        }

        public async void ResumeGame()
        {
            var result = await DisplayAlert("Detected Game", "Would You Like To Resume Game With Player", "Yes", "No");

            if (result)
            {
                bluetooth.SendMessage("resume");
                GoToGame();
            }
            else
            {
                FileManager file = new FileManager();
                file.DeleteGameSetting(settings.ConnectedDeviceName);
            }
        }

        public async void ReceiveResume()
        {
            FileManager file = new FileManager();
            if (file.CheckForExistingGame(settings.ConnectedDeviceName))
            {
                var result = await DisplayAlert("Resume Game Request", "Would You Like To Resume Game With " + settings.ConnectedDeviceName, "Yes", "No");

                if (result)
                {
                    bluetooth.SendMessage("accept");
                    settings = file.GetGameSettings(settings.ConnectedDeviceName);
                    GoToGame();
                }
                else
                {
                    bluetooth.SendMessage("reject");
                    file.DeleteGameSetting(settings.ConnectedDeviceName);
                    Setup();
                }
            }
            else
            {
                bluetooth.SendMessage("reject");
                Setup();
            }
        }

        public void AcceptResume()
        {
            ToastManager.Show("Resuming Game");
            FileManager file = new FileManager();
            settings = file.GetGameSettings(settings.ConnectedDeviceName);
            GoToGame();
        }

        public void RejectResume()
        {
            ToastManager.Show("Resume Game Rejected");
            FileManager file = new FileManager();
            file.DeleteGameSetting(settings.ConnectedDeviceName);
        }

        public void GoToGame()
        {
            Navigation.PushAsync(new Game(bluetooth, settings));
        }

        protected override bool OnBackButtonPressed()
        {
            Back();
            return true;
        }

        private async void Back()
        {
            var result = await DisplayAlert("Alert!", "Are You Sure You Want To Exit Game?", "Yes", "No");

            if (result)
            {
                Application.Current.Quit();
            }
        }

        private void PickerSizeOfBoard_SelectedIndexChanged(object sender, EventArgs e)
        {             
            switch(pickerSizeOfBoard.SelectedIndex)
            {
                case 0:
                    settings.SizeOfGrid = 6;
                    break;
                case 1:
                    settings.SizeOfGrid = 8;
                    break;
                case 2:
                    settings.SizeOfGrid = 10;
                    break;
                default:
                    settings.SizeOfGrid = 6;
                    break;
            }

            bluetooth.SendMessage(settings.SizeOfGrid + "," + "grid");
        }

        public static void StatSetPicker(string SizeOfGrid)
        {
            BGStuff.setupGame.SetPicker(SizeOfGrid);
        }

        public void SetPicker(string sizeOfGrid)
        {
            try
            {
                settings.SizeOfGrid = int.Parse(sizeOfGrid);
                switch (sizeOfGrid)
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
            catch(Exception ex)
            {

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
                        bluetooth.SendMessage(intTemp.ToString() + ",num");
                        settings.NumOfShots = intTemp;
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
                settings.NumOfShots = 0;
            }
        }

        public static void StatSetNumOfShotsEntry(string _numOfShots)
        {
            BGStuff.setupGame.SetNumOfShotsEntry(_numOfShots);
        }
        public void SetNumOfShotsEntry(string numOfShots)
        {
            entryNumOfShots.Text = numOfShots;
            try
            {
                settings.NumOfShots = int.Parse(numOfShots);
            }
            catch(Exception ex)
            {
                settings.NumOfShots = 0;
            }
        }
        private void EntName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string stringTemp = entName.Text;
            if (string.IsNullOrEmpty(stringTemp) || stringTemp == "")
            { stringTemp = ""; }
            if (stringTemp != "")
            {
                try
                {
                        bluetooth.SendMessage(stringTemp + ",nam");
                        settings.YourName = stringTemp;
                }
                catch (Exception ex)
                {
                    ToastManager.Show(ex.Message);
                }
            }
            else
            {
                bluetooth.SendMessage(",n");
            }
        }

        public static void StatSetEnemyName(string emName)
        {
            BGStuff.setupGame.SetEnemyName(emName);
        }

        public void SetEnemyName(string emName)
        {
            settings.EnemyName = emName;
        }

        public static void StatGoToSetup2()
        {
            BGStuff.setupGame.GoToSetup2();
        }

        private void BtnContinue_Clicked(object sender, EventArgs e)
        {
            if ((settings.EnemyName != null) && settings.NumOfShots > 0 && settings.SizeOfGrid > 0 && (settings.YourName != null))
            {
                bluetooth.SendMessage("Setup2");
                GoToSetup2();
            }
            else
            {
                if(settings.EnemyName == null)
                {
                    ToastManager.Show("Enemy Not Entered Name Yet");
                }
                else if(settings.NumOfShots == 0)
                {
                    ToastManager.Show("Number Of Shots Hasn't Been Selected");
                }
                else if (settings.SizeOfGrid == 0)
                {
                    ToastManager.Show("Size Of Grid Hasn't Been Selected");
                }
                else if(settings.YourName == null)
                {
                    ToastManager.Show("Please Enter A Name");
                }
            }
        }

        public void GoToSetup2()
        {
            BGStuff.settingUpGame = false;
            Navigation.PushAsync(new SetupGame2(bluetooth, settings));
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