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
            Labels.Add(txtStatus);
            imageShotGlass.Source = FileManager.SRCShotGlass;
            ApplyTheme();
            this.bluetooth = bluetooth;
            this.settings = gameSettings;
            settings.YourTurn = false;
            settings.EnemyShots = settings.NumOfShots;
            txtNumOfShots.Text = settings.NumOfShots.ToString();            
            GameGrid grid = new GameGrid(this, MainStack, SecStack, settings);

            // check for who goes first
            if(settings.Master)
            {
                Random random = new Random();
                int ranNum = random.Next(0, 1);
                if(ranNum == 1)
                {
                    bluetooth.SendMessage(ranNum.ToString());
                    txtStatus.Text = settings.EnemyName + "'s Goes First";
                }
                else
                {
                    settings.YourTurn = true;
                    bluetooth.SendMessage(ranNum.ToString());
                    txtStatus.Text = "You Go First";
                }
            }
        }

        ImageButton btn;

        public void GridButton_Clicked(object sender, EventArgs e)
        {
            if (settings.YourTurn)
            {
                btn = (ImageButton)sender;

                settings.YourTurn = false;
                ToastManager.Show(btn.ClassId);
                settings.AllReadySelected.Add(btn.ClassId);
                bluetooth.SendMessage(btn.ClassId);
            }
            else
            {
                ToastManager.Show("Not Your Turn");
                Task.Run(async () =>
                {
                    uint timeout = 50;
                    await MainStack.TranslateTo(-15, 0, timeout);
                    await MainStack.TranslateTo(15, 0, timeout);
                    await MainStack.TranslateTo(-9, 0, timeout);
                    await MainStack.TranslateTo(9, 0, timeout);
                    await MainStack.TranslateTo(-5, 0, timeout);
                    await MainStack.TranslateTo(5, 0, timeout);
                    await MainStack.TranslateTo(-2, 0, timeout);
                    await MainStack.TranslateTo(2, 0, timeout);
                    MainStack.TranslationX = 0;
                });
            }
        }

        public void ReceiveCheck(string coordenates)
        {
            string[] split = coordenates.Split(',');

            if (settings.YourShotCoodinates.Contains(coordenates))
            {
                settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])].Source = FileManager.SRCGridButtonShotGlassCross;
                settings.YourShotCoodinates.Remove(coordenates);
                settings.NumOfShots -= 1;
                txtNumOfShots.Text = settings.NumOfShots.ToString();
                bluetooth.SendMessage("hit");
                ToastManager.Show("Drink Up!");
            }
            else
            {
                bluetooth.SendMessage("miss");
            }            
        }

        public void ReceiveHit(bool value)
        {
            if (value)
            {
                btn.Source = FileManager.SRCGridButtonShotGlassCross;
                settings.EnemyShots -= 1;
                if (settings.EnemyShots == 0)
                {
                    ToastManager.Show("You Win");
                    bluetooth.SendMessage("endgame");
                    EndGame(true);
                }
                else
                {
                    ToastManager.Show("Hit!");
                    bluetooth.SendMessage("ready");
                    txtStatus.Text = settings.EnemyName + "'s Turn";
                }
            }
            else
            {
                btn.Source = FileManager.SRCGridButtonCross;
                ToastManager.Show("Miss!");
                bluetooth.SendMessage("ready");
                txtStatus.Text = settings.EnemyName + "'s Turn";
            }
        }

        public void Ready()
        {
            settings.YourTurn = true;
            txtStatus.Text = "Your Turn";
        }

        public void EndGame(bool value)
        {
            if (!value)            
            {
                ToastManager.Show("You Lose! Drink The Rest Of Your Shots!");
            }

            Button btn = new Button
            {
                Text = "Exit Game",
                TextColor = Theme.ButtonTextColour,
                BackgroundColor = Theme.ButtonBgColour,
                BorderColor = Theme.ButtonBorderColour
            };
            btn.Clicked += EndGame_Clicked;
            PageStack.Children.Add(btn);

        }

        public void EndGame_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        public void GoesFirst(int value)
        {
            if (value == 1)
            {
                txtStatus.Text = "You Go First";
                
            }
            else
            {
                txtStatus.Text = settings.EnemyName + "'s Goes First";
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