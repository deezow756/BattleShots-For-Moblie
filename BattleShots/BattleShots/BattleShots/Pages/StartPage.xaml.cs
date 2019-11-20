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
    public partial class StartPage : ContentPage
    {
        public List<Button> Buttons = new List<Button>();
        public List<Label> Labels = new List<Label>();

        bool animate = true;
        public StartPage()
        {
            InitializeComponent();
            Buttons.Add(BtnStart);
            ApplyTheme();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            animate = true;
            AnimateLabel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            animate = false;
        }

        void AnimateLabel()
        {
            Task.Run(async () =>
             {
                 Random ran = new Random();
                 await Task.Delay(2000);
                 do
                 {
                     startImage.TranslateTo(ran.Next(-((int)Math.Round(Application.Current.MainPage.Width / 2)), (int)Math.Round(Application.Current.MainPage.Width / 2)), ran.Next(0, (int)Math.Round(Application.Current.MainPage.Height / 1.5)), 5000);
                     startImage.RotateTo(-360, 5000);
                     await startImage.ScaleTo(2, 2500);
                     await startImage.ScaleTo(1, 2500);
                     startImage.Rotation = 0;

                 } while (animate);

             });
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage(), true);
        }

        public void ApplyTheme()
        {
            this.BackgroundColor = Theme.BgColour;
            for (int i = 0; i < Labels.Count; i++)
            {
                Labels[i].TextColor = Theme.LabelTextColour;
            }

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].TextColor = Theme.ButtonTextColour;
                Buttons[i].BackgroundColor = Theme.ButtonBgColour;
                Buttons[i].BorderColor = Theme.ButtonBorderColour;
            }
        }
    }
}