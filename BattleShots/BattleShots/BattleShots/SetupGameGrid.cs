using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BattleShots
{
    public class SetupGameGrid
    {
        StackLayout mainStack;
        GameSettings gameSettings;

        public SetupGameGrid(StackLayout mainStack, GameSettings gameSettings)
        {
            this.mainStack = mainStack;
            this.gameSettings = gameSettings;
            Execute();
        }

        public void Execute()
        {
            StackLayout gridStack = new StackLayout();
            gridStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            mainStack.Children.Add(gridStack);

            double buttonSize;
            buttonSize = mainStack.Width / gameSettings.SizeOfGrid;

            for (int i = 0; i < gameSettings.SizeOfGrid; i++)
            {
                StackLayout stack = new StackLayout();
                stack.HorizontalOptions = LayoutOptions.FillAndExpand;

                for (int j = 0; j < gameSettings.SizeOfGrid; j++)
                {
                    Button button = new Button()
                    {
                        BackgroundColor = Theme.ButtonBgColour,
                        BorderColor = Theme.ButtonBorderColour,
                        TextColor = Theme.ButtonTextColour,
                        ClassId = i.ToString() + "," + j.ToString(),
                        BorderWidth = 1,
                        HeightRequest = buttonSize - 2,
                        WidthRequest = buttonSize - 2
                    };
                    button.Clicked += Button_Clicked;

                    gameSettings.GridButtons[i, j] = button;
                }

                gridStack.Children.Add(stack);
            }

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            if (btn.Text != "X")
            {
                btn.Text = "X";
                gameSettings.ShotCoodinates.Add(btn.ClassId);
            }
            else
            {
                btn.Text = "";
                gameSettings.ShotCoodinates.Remove(btn.ClassId);
            }

        }
    }
}
