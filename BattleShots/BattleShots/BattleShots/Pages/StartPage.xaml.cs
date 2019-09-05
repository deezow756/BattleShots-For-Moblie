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
        public StartPage()
        {
            InitializeComponent();
            Buttons.Add(BtnStart);
            Labels.Add(txtTitle);
            ApplyTheme();
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage());
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