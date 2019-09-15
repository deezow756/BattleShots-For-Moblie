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
    public partial class SetupGame2 : ContentPage
    {
        public BluetoothMag bluetooth { get; set; }
        public GameSettings gameSettings { get; set; }
        public bool Master { get; set; }

        public int ShotsLeft { get; set; }
        public SetupGame2(BluetoothMag bluetoothMag, GameSettings gameSettings)
        {
            InitializeComponent();
            Labels.Add(txtTitle);
            Labels.Add(txtShotsLabel);
            Labels.Add(txtNumOfShotsLeft);
            Labels.Add(txtEnReadyLabel);
            ApplyTheme();
            bluetooth = bluetoothMag;
            this.gameSettings = gameSettings;
            BGStuff.setUpGame2 = this;
            btnContinue.IsEnabled = false;
            txtEnReady.Text = "Not Ready";
            txtEnReady.TextColor = Color.Red;
            Master = bluetooth.GetMaster();
            bluetooth.ReadMessage();
            ShotsLeft = gameSettings.NumOfShots;
            txtNumOfShotsLeft.Text = ShotsLeft.ToString(); 
            SetupGameGrid setupGameGrid = new SetupGameGrid(this, MainLayout, gameSettings);
        }

        #region OnBack
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
        #endregion

        public void GridButton_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            if (btn.Text != "X")
            {
                if (!((ShotsLeft - 1) < 0))
                {
                    ShotsLeft -= 1;
                    btn.Text = "X";
                    gameSettings.YourShotCoodinates.Add(btn.ClassId);
                }
            }
            else
            {
                if (!((ShotsLeft + 1) > gameSettings.NumOfShots))
                {
                    ShotsLeft += 1;
                    btn.Text = "";
                    gameSettings.YourShotCoodinates.Remove(btn.ClassId);
                }
            }

            txtNumOfShotsLeft.Text = ShotsLeft.ToString();

            if(ShotsLeft == 0)
            {
                btnContinue.IsEnabled = true;
            }
            else
            {
                btnContinue.IsEnabled = false;
                gameSettings.Ready = false;
                bluetooth.SendMessage("unready");
            }
        }

        public void SetEnemyReady(bool ready)
        {
            if(ready)
            {
                gameSettings.EnemyReady = true;
                txtEnReady.Text = "Ready";
                txtEnReady.TextColor = Color.Green;
                if(gameSettings.Ready)
                {
                    Continue();
                }
            }
            else
            {
                gameSettings.EnemyReady = false;
                txtEnReady.Text = "Not Ready";
                txtEnReady.TextColor = Color.Red;
            }
        }

        #region Theme Stuff
        public List<Button> Buttons = new List<Button>();
        public List<Label> Labels = new List<Label>();
        public List<Entry> Entries = new List<Entry>();
        public List<Picker> Pickers = new List<Picker>();
        public void ApplyTheme()
        {
            this.BackgroundColor = Theme.BgColour;
            if (Labels.Count > 0)
            {
                for (int i = 0; i < Labels.Count; i++)
                {
                    Labels[i].TextColor = Theme.LabelTextColour;
                }
            }
            if (Buttons.Count > 0)
            {
                for (int i = 0; i < Buttons.Count; i++)
                {
                    Buttons[i].TextColor = Theme.ButtonTextColour;
                    Buttons[i].BackgroundColor = Theme.ButtonBgColour;
                    Buttons[i].BorderColor = Theme.ButtonBorderColour;
                }
            }
            if (Entries.Count > 0)
            {
                for (int i = 0; i < Entries.Count; i++)
                {
                    Entries[i].TextColor = Theme.EntryTextColour;
                    Entries[i].PlaceholderColor = Theme.EntryPlaceholderColour;
                }
            }
            if (Pickers.Count > 0)
            {
                for (int i = 0; i < Pickers.Count; i++)
                {
                    Pickers[i].TextColor = Theme.PickerTextColour;
                    Pickers[i].TitleColor = Theme.PickerTitleColour;
                    Pickers[i].BackgroundColor = Theme.PickerBgColour;
                }
            }
        }
        #endregion

        private void BtnContinue_Clicked(object sender, EventArgs e)
        {
            bluetooth.SendMessage("ready");
            gameSettings.Ready = true;
        }

        private void Continue()
        {
            BGStuff.settingUpGame2 = false;
            Navigation.PushAsync(new Game(bluetooth, gameSettings));
        }
        public void Reconnect()
        {
            Navigation.PushAsync(new ReconnectionPage(bluetooth));
        }
    }
}