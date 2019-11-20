using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BattleShots
{
    public class GameGrid
    {
        Game game;
        StackLayout mainStack;
        StackLayout secStack;
        GameSettings gameSettings;

        public GameGrid(Game game, StackLayout mainStack, StackLayout secStack, GameSettings gameSettings)
        {
            this.game = game;
            this.mainStack = mainStack;
            this.secStack = secStack;
            this.gameSettings = gameSettings;
            Execute();
        }

        public void Execute()
        {
            double buttonSize;
            buttonSize = Application.Current.MainPage.Width / (gameSettings.SizeOfGrid + 2);

            mainStack.Children.Add(new Label()
            {
                Text = gameSettings.EnemyName + "'s",
                FontSize = 25,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Theme.LabelTextColour,
                BackgroundColor = Theme.BgColour
            });

            for (double i = -1; i < gameSettings.SizeOfGrid; i++)
            {
                StackLayout stack = new StackLayout();
                stack.Orientation = StackOrientation.Horizontal;
                stack.HorizontalOptions = LayoutOptions.CenterAndExpand;

                if (i == -1)
                {
                    Label flabel = new Label()
                    {
                        Text = " ",
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Theme.LabelTextColour,
                        HeightRequest = buttonSize / 2,
                        WidthRequest = buttonSize / 2,
                        MinimumHeightRequest = buttonSize / 2,
                        MinimumWidthRequest = buttonSize / 2
                    };
                    stack.Children.Add(flabel);
                    for (int j = 0; j < gameSettings.SizeOfGrid; j++)
                    {
                        Label label = new Label()
                        {
                            Text = (j + 1).ToString(),
                            TextColor = Theme.LabelTextColour,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HeightRequest = buttonSize,
                            WidthRequest = buttonSize,
                            MinimumHeightRequest = buttonSize,
                            MinimumWidthRequest = buttonSize
                        };
                        stack.Children.Add(label);
                    }
                }
                else
                {
                    for (double j = -1; j < gameSettings.SizeOfGrid; j++)
                    {
                        if (j == -1)
                        {
                            switch (i)
                            {
                                case 0:
                                    stack.Children.Add(MakeLabel("A", buttonSize));
                                    break;
                                case 1:
                                    stack.Children.Add(MakeLabel("B", buttonSize));
                                    break;
                                case 2:
                                    stack.Children.Add(MakeLabel("C", buttonSize));
                                    break;
                                case 3:
                                    stack.Children.Add(MakeLabel("D", buttonSize));
                                    break;
                                case 4:
                                    stack.Children.Add(MakeLabel("E", buttonSize));
                                    break;
                                case 5:
                                    stack.Children.Add(MakeLabel("F", buttonSize));
                                    break;
                                case 6:
                                    stack.Children.Add(MakeLabel("G", buttonSize));
                                    break;
                                case 7:
                                    stack.Children.Add(MakeLabel("H", buttonSize));
                                    break;
                                case 8:
                                    stack.Children.Add(MakeLabel("I", buttonSize));
                                    break;
                                case 9:
                                    stack.Children.Add(MakeLabel("J", buttonSize));
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            ImageButton button = new ImageButton()
                            {
                                Source = FileManager.SRCGridButton,
                                ClassId = i.ToString() + "," + j.ToString(),
                                BorderWidth = 1,
                                HeightRequest = buttonSize,
                                WidthRequest = buttonSize,
                                MinimumHeightRequest = buttonSize,
                                MinimumWidthRequest = buttonSize,
                                HorizontalOptions = LayoutOptions.CenterAndExpand
                            };
                            button.Clicked += game.GridButton_Clicked;
                            stack.Children.Add(button);

                            gameSettings.EnemyGrid[(int)i, (int)j] = button;
                        }
                    }
                }
                mainStack.Children.Add(stack);
            }

            buttonSize = buttonSize / 2;

            secStack.Children.Add(new Label()
            {
                Text = "Yours",
                FontSize = 25,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Theme.LabelTextColour,
                BackgroundColor = Theme.BgColour
            }) ;

            for (double i = -1; i < gameSettings.SizeOfGrid; i++)
            {
                StackLayout stack = new StackLayout();
                stack.Orientation = StackOrientation.Horizontal;
                stack.HorizontalOptions = LayoutOptions.CenterAndExpand;

                if (i == -1)
                {
                    Label flabel = new Label()
                    {
                        Text = " ",
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Theme.LabelTextColour,
                        HeightRequest = buttonSize,
                        WidthRequest = buttonSize,
                        MinimumHeightRequest = buttonSize,
                        MinimumWidthRequest = buttonSize
                    };
                    stack.Children.Add(flabel);
                    for (int j = 0; j < gameSettings.SizeOfGrid; j++)
                    {
                        Label label = new Label()
                        {
                            Text = (j + 1).ToString(),
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            TextColor = Theme.LabelTextColour,
                            HeightRequest = buttonSize,
                            WidthRequest = buttonSize,
                            MinimumHeightRequest = buttonSize,
                            MinimumWidthRequest = buttonSize
                        };
                        stack.Children.Add(label);
                    }
                }
                else
                {
                    for (double j = -1; j < gameSettings.SizeOfGrid; j++)
                    {
                        if (j == -1)
                        {
                            switch (i)
                            {
                                case 0:
                                    stack.Children.Add(MakeLabel("A", buttonSize));
                                    break;
                                case 1:
                                    stack.Children.Add(MakeLabel("B", buttonSize));
                                    break;
                                case 2:
                                    stack.Children.Add(MakeLabel("C", buttonSize));
                                    break;
                                case 3:
                                    stack.Children.Add(MakeLabel("D", buttonSize));
                                    break;
                                case 4:
                                    stack.Children.Add(MakeLabel("E", buttonSize));
                                    break;
                                case 5:
                                    stack.Children.Add(MakeLabel("F", buttonSize));
                                    break;
                                case 6:
                                    stack.Children.Add(MakeLabel("G", buttonSize));
                                    break;
                                case 7:
                                    stack.Children.Add(MakeLabel("H", buttonSize));
                                    break;
                                case 8:
                                    stack.Children.Add(MakeLabel("I", buttonSize));
                                    break;
                                case 9:
                                    stack.Children.Add(MakeLabel("J", buttonSize));
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            if (gameSettings.YourShotCoodinates.Contains(i.ToString() + "," + j.ToString()))
                            {
                                ImageButton button = new ImageButton()
                                {
                                    Source = FileManager.SRCGridButtonShotGlass,
                                    ClassId = i.ToString() + "," + j.ToString(),
                                    BorderWidth = 1,
                                    HeightRequest = buttonSize,
                                    WidthRequest = buttonSize,
                                    MinimumHeightRequest = buttonSize,
                                    MinimumWidthRequest = buttonSize,
                                    HorizontalOptions = LayoutOptions.CenterAndExpand
                                };

                                stack.Children.Add(button);
                                gameSettings.YourGrid[(int)i, (int)j] = button;
                            }
                            else
                            {
                                ImageButton button = new ImageButton()
                                {
                                    Source = FileManager.SRCGridButton,
                                    ClassId = i.ToString() + "," + j.ToString(),
                                    BorderWidth = 1,
                                    HeightRequest = buttonSize,
                                    WidthRequest = buttonSize,
                                    MinimumHeightRequest = buttonSize,
                                    MinimumWidthRequest = buttonSize,
                                    HorizontalOptions = LayoutOptions.CenterAndExpand
                                };

                                stack.Children.Add(button);
                                gameSettings.YourGrid[(int)i, (int)j] = button;
                            }
                        }
                    }
                }

                secStack.Children.Add(stack);
            }
        }

        private Label MakeLabel(string Text, double buttonSize)
        {
            Label label = new Label()
            {
                Text = Text,
                TextColor = Theme.LabelTextColour,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = buttonSize / 2,
                WidthRequest = buttonSize / 2,
                MinimumHeightRequest = buttonSize / 2,
                MinimumWidthRequest = buttonSize / 2,
            };

            return label;
        }
    }
}
