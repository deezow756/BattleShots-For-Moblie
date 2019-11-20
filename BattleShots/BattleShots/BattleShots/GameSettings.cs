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
                YourGrid = new ImageButton[value, value];
                EnemyGrid = new ImageButton[value, value];
                sizeOfGrid = value;                
            }
        }
        public int NumOfShots { get; set; }

        public int EnemyShots { get; set; }

        public Dictionary<string, string> ShotNames = new Dictionary<string, string>();

        public ImageButton[,] EnemyGrid;

        public ImageButton[,] YourGrid;

        public List<String> AllReadySelected = new List<string>();

        public List<string> YourShotCoodinates = new List<string>();
        public bool Ready { get; set; }
        public bool EnemyReady { get; set; }

        public bool YourTurn { get; set; }

        public GameSettings()
        {
        }
    }
}
