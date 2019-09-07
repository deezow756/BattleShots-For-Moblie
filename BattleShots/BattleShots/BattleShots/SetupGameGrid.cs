using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BattleShots
{
    public class SetupGameGrid
    {
        SetupGame2 SetupGame2;
        StackLayout mainStack;
        GameSettings gameSettings;

        public SetupGameGrid(SetupGame2 setupGame2, StackLayout mainStack, GameSettings gameSettings)
        {
            SetupGame2 = setupGame2;
            this.mainStack = mainStack;
            this.gameSettings = gameSettings;
            Execute();
        }

        public void Execute()
        {
            double buttonSize;
            buttonSize = Application.Current.MainPage.Width / (gameSettings.SizeOfGrid + 1);

            for (int i = 0; i < gameSettings.SizeOfGrid; i++)
            {
                StackLayout stack = new StackLayout();
                stack.Orientation = StackOrientation.Horizontal;

                for (int j = 0; j < gameSettings.SizeOfGrid; j++)
                {
                    Button button = new Button()
                    {
                        BackgroundColor = Theme.ButtonBgColour,
                        BorderColor = Theme.ButtonBorderColour,
                        TextColor = Theme.ButtonTextColour,
                        ClassId = i.ToString() + "," + j.ToString(),
                        BorderWidth = 1,
                        HeightRequest = buttonSize,
                        WidthRequest = buttonSize,
                        MinimumHeightRequest = buttonSize,
                        MinimumWidthRequest = buttonSize
                    };
                    button.Clicked += SetupGame2.GridButton_Clicked;
                    stack.Children.Add(button);
                    gameSettings.GridButtons[i, j] = button;
                }

                mainStack.Children.Add(stack);
            }
        }
        
    }
}
