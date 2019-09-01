using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BattleShots
{
    public class ToastManager
    {
        public static void Show(string message)
        {
            DependencyService.Get<IToastInterface>().Show(message);
        }
    }
}
