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
    public partial class Game : ContentPage
    {
        BluetoothMag bluetooth;
        GameSettings settings;
        public Game(BluetoothMag bluetooth, GameSettings gameSettings)
        {
            InitializeComponent();
            BGStuff.game = this;
            BGStuff.InGame = true;
            Labels.Add(txtNumOfShots);
            ApplyTheme();
            txtNumOfShots.Text = settings.NumOfShots.ToString();
            this.bluetooth = bluetooth;
            this.settings = gameSettings;
            GameGrid grid = new GameGrid(this, MainStack, settings);
            IsEnableEnemyGrid(false);

            // check for who goes first
            if(settings.Master)
            {
                Random random = new Random();
                int ranNum = random.Next(0, 1);
                if(ranNum == 1)
                {
                    bluetooth.SendMessage(ranNum.ToString());
                    ToastManager.Show("Enemy Goes First");
                }
                else
                {
                    ToastManager.Show("You Go First");
                    IsEnableEnemyGrid(true);
                }
            }

        }

        public void GridButton_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            IsEnableEnemyGrid(false);
            bluetooth.SendMessage(btn.ClassId);
            btn.Text = "X";
        }

        public void GridDisabled_Clicked(object sender, EventArgs e)
        {
            ToastManager.Show("Not Your Turn");
        }

        public void GridAlready_Clicked(object sender, EventArgs e)
        {
            ToastManager.Show("Already Tried This One");
        }

        public void ReceiveCheck(string coordenates)
        {
            string[] split = coordenates.Split(',');
            bool hit = false;

            foreach (string coord in settings.YourShotCoodinates)
            {
                if (coord == coordenates)
                {
                    settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])].Text = "!";
                    settings.YourShotCoodinates.Remove(coord);
                    bluetooth.SendMessage("hit");
                    ToastManager.Show("Drink Up!");
                    hit = true;
                }
            }

            if(!hit)
            {
                bluetooth.SendMessage("miss");
            }            
        }

        public void ReceiveHit(bool value)
        {
            if (value)
            {
                settings.NumOfShots -= 1;
                if (settings.NumOfShots == 0)
                {
                    ToastManager.Show("You Win");
                    bluetooth.SendMessage("endgame");
                    EndGame();
                }
                else
                {
                    ToastManager.Show("Hit!");
                    bluetooth.SendMessage("ready");
                }
            }
            else
            {
                ToastManager.Show("Miss!, " + settings.EnemyName + "'s Turn");
                bluetooth.SendMessage("ready");
            }
        }

        public void Ready()
        {
            IsEnableEnemyGrid(true);
            ToastManager.Show("Your Turn");
        }

        public void EndGame()
        {
            ToastManager.Show("You Lose! Drink The Rest Of Your Shots!");

            Button btn = new Button
            {
                Text = "Exit Game",
                TextColor = Theme.ButtonTextColour,
                BackgroundColor = Theme.ButtonBgColour,
                BorderColor = Theme.ButtonBorderColour
            };
            btn.Clicked += EndGame_Clicked;
            MainStack.Children.Add(btn);

        }

        public void EndGame_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        public void IsEnableEnemyGrid(bool value)
        {
            for (int i = 0; i < settings.SizeOfGrid; i++)
            {
                for (int j = 0; j < settings.SizeOfGrid; j++)
                {
                    if (value)
                    {
                        if (settings.EnemyGrid[i, j].Text == "X")
                        {
                            settings.EnemyGrid[i, j].Clicked += GridAlready_Clicked;
                        }
                        else
                        {
                            settings.EnemyGrid[i, j].Clicked += GridButton_Clicked;
                        }
                    }
                    else
                    {
                        settings.EnemyGrid[i, j].Clicked += GridDisabled_Clicked;
                    }
                }
            }
        }

        public void GoesFirst(int value)
        {
            if (value == 1)
            {
                ToastManager.Show("You Go First");
                IsEnableEnemyGrid(true);
            }
            else
            {
                ToastManager.Show("Enemy Goes First");
            }
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

        public void Reconnect()
        {
            Navigation.PushAsync(new ReconnectionPage(bluetooth));
        }
    }
}