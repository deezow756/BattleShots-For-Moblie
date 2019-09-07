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
            ApplyTheme();
            bluetooth = bluetoothMag;
            this.gameSettings = gameSettings;
            BGStuff.setUpGame2 = this;
            Master = bluetooth.GetMaster();
            bluetooth.ReadMessage();
            txtNumOfShotsLeft.Text = gameSettings.NumOfShots.ToString();
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
            if (ShotsLeft > 0)
            {
                if (btn.Text != "X")
                {
                    ShotsLeft -= 1;
                    btn.Text = "X";
                    gameSettings.ShotCoodinates.Add(btn.ClassId);
                }
                else
                {
                    ShotsLeft += 1;
                    btn.Text = "";
                    gameSettings.ShotCoodinates.Remove(btn.ClassId);
                }
            }
            else
            {
                if(btn.Text == "X")
                {
                    ShotsLeft += 1;
                    btn.Text = "";
                    gameSettings.ShotCoodinates.Remove(btn.ClassId);
                }
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

        }
    }
}