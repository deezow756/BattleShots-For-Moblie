using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BattleShots
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.Register<IBluetooth>();
            DependencyService.Register<IToastInterface>();
            MainPage = new NavigationPage(new StartPage()) { BarBackgroundColor = Theme.BgColour};
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
