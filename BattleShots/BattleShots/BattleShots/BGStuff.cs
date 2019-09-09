using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShots
{
    public class BGStuff
    {
        public static MainPage mainPage { get; set; }
        public static bool ConnectionSetup { get; set; }
        public static SetupGame setupGame { get; set; }
        public static bool settingUpGame { get; set; }
        public static SetupGame2 setUpGame2 { get; set; }
        public static bool settingUpGame2 { get; set; }
        public static Game game { get; set; }
        public static bool InGame { get; set; }
        public static ReconnectionPage reconnectionPage { get; set; }
        public static bool Reconnecting { get; set; }
    }
}
