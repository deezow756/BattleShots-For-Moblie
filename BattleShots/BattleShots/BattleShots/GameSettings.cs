using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BattleShots
{
    public class GameSettings
    {
        public string ConnectedDeviceName { get; set; }
        public bool Master { get; set; }
        public string YourName { get; set; }
        public string EnemyName { get; set; }
        private int sizeOfGrid;
        public int SizeOfGrid
        {
            get
            {
                return sizeOfGrid;
            }
            set
            {
                GridButtons = new Button[value, value];
                sizeOfGrid = value;                
            }
        }
        public int NumOfShots { get; set; }

        public Button[,] GridButtons;

        public List<string> ShotCoodinates = new List<string>();
        public bool Ready { get; set; }
        public bool EnemyReady { get; set; }

        public GameSettings()
        {
        }
    }
}
