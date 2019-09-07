﻿using System;
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
        public SetupGame2(BluetoothMag bluetoothMag, GameSettings gameSettings)
        {
            InitializeComponent();
            bluetooth = bluetoothMag;
            this.gameSettings = gameSettings;
            Master = bluetooth.GetMaster();

            SetupGameGrid setupGameGrid = new SetupGameGrid(MainLayout, gameSettings);
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
    }
}