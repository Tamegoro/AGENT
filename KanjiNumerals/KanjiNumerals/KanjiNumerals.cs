using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace KanjiNumerals
{
    public class KanjiNumerals
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontIPAexGothicKansuji05 = Resources.GetFont(Resources.FontResources.IPAexGothicKansuji05);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int marginX = 0;
        static int marginY = 0;

        static int currentH = 0;
        static int currentM = 0;

        static int fontWidth = 0;
        static int fontHeight = 0;

        static int displayMode = DISPLAY_MODE_BLACK_12;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_NUMBER_NUMBER_X = 3;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_12 = 0;
        const int DISPLAY_MODE_BLACK_24 = 1;
        const int DISPLAY_MODE_WHITE_12 = 2;
        const int DISPLAY_MODE_WHITE_24 = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);


            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            colorForeground = new Color();
            colorBackground = new Color();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            fontWidth = fontIPAexGothicKansuji05.CharWidth('零');
            fontHeight = fontIPAexGothicKansuji05.Height;

            marginX = (screenWidth - (fontWidth * 13) - (MARGIN_NUMBER_NUMBER_X * 4)) / 2;
            marginY = (screenHeight - (fontHeight * 5)) / 2;

            displayMode = DISPLAY_MODE_BLACK_12;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.ButtonSetup = new Buttons[]
            {
                Buttons.BottomRight, Buttons.MiddleRight, Buttons.TopRight
            };

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
        {

            if (showDigital == false)
            {

                currentTime = DateTime.Now;

                if (displayMode == DISPLAY_MODE_BLACK_12 || displayMode == DISPLAY_MODE_WHITE_12)
                {
                    currentH = currentTime.Hour % 12;
                }
                else
                {
                    currentH = currentTime.Hour;
                }

                currentM = currentTime.Minute;

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                DrawNumber(currentH / 10, marginX + ((fontWidth * 3) * 0 + (MARGIN_NUMBER_NUMBER_X * 0)), marginY);
                DrawNumber(currentH % 10, marginX + ((fontWidth * 3) * 1 + (MARGIN_NUMBER_NUMBER_X * 1)), marginY);
                DrawColon(marginX + ((fontWidth * 3) * 2 + (MARGIN_NUMBER_NUMBER_X * 2)), marginY);
                DrawNumber(currentM / 10, marginX + ((fontWidth * 3) * 2 + (MARGIN_NUMBER_NUMBER_X * 3)) + fontWidth + 1, marginY);
                DrawNumber(currentM % 10, marginX + ((fontWidth * 3) * 3 + (MARGIN_NUMBER_NUMBER_X * 4)) + fontWidth + 1, marginY);

                _display.Flush();

            }

        }

        private static void DrawNumber(int number, int x, int y)
        {

            switch (number)
            {

/*
                case 0:

                    _display.DrawText("零零零", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 0));
                    _display.DrawText("零　零", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 1));
                    _display.DrawText("零　零", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 2));
                    _display.DrawText("零　零", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 3));
                    _display.DrawText("零零零", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 4));

                    break;


                case 1:

                    _display.DrawText("　壱　", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 0));
                    _display.DrawText("壱壱　", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 1));
                    _display.DrawText("　壱　", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 2));
                    _display.DrawText("　壱　", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 3));
                    _display.DrawText("壱壱壱", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 4));

                    break;


                case 2:

                    _display.DrawText("弐弐弐", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 0));
                    _display.DrawText("　　弐", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 1));
                    _display.DrawText("弐弐弐", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 2));
                    _display.DrawText("弐　", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 3));
                    _display.DrawText("弐弐弐", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 4));

                    break;


                case 3:

                    _display.DrawText("参参参", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 0));
                    _display.DrawText("　　参", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 1));
                    _display.DrawText("参参参", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 2));
                    _display.DrawText("　　参", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 3));
                    _display.DrawText("参参参", fontIPAexMinchoKansuji05, colorForeground, x, y + (fontIPAexMinchoKansuji05.Height * 4));

                    break;
*/

                
                case 0:

                    _display.DrawText("〇〇〇", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("〇　〇", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("〇　〇", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("〇　〇", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("〇〇〇", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 1:

                    _display.DrawText("　一　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("一一　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("　一　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("　一　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("一一一", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 2:

                    _display.DrawText("二二二", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("　　二", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("二二二", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("二　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("二二二", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 3:

                    _display.DrawText("三三三", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("　　三", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("三三三", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("　　三", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("三三三", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 4:

                    _display.DrawText("四　四", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("四　四", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("四四四", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("　　四", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("　　四", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 5:

                    _display.DrawText("五五五", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("五　　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("五五五", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("　　五", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("五五五", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 6:

                    _display.DrawText("六六六", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("六　　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("六六六", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("六　六", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("六六六", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 7:

                    _display.DrawText("七七七", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("　　七", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("　　七", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("　　七", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("　　七", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 8:

                    _display.DrawText("八八八", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("八　八", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("八八八", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("八　八", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("八八八", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


                case 9:

                    _display.DrawText("九九九", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
                    _display.DrawText("九　九", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
                    _display.DrawText("九九九", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
                    _display.DrawText("　　九", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
                    _display.DrawText("九九九", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

                    break;


            }


        }

        private static void DrawColon(int x, int y)
        {

            _display.DrawText("　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 0));
            _display.DrawText("点", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 1));
            _display.DrawText("　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 2));
            _display.DrawText("点", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 3));
            _display.DrawText("　", fontIPAexGothicKansuji05, colorForeground, x, y + (fontIPAexGothicKansuji05.Height * 4));

        }


        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {

                if (button == Buttons.TopRight)
                {
                    if (showDigital == false)
                    {
                        --displayMode;
                        if (displayMode < 0)
                        {
                            displayMode = MAX_DISPLAY_MODE;
                        }
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital != true)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
                        UpdateTimeDigital(null);
                        _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
                    }
                    else
                    {
                        showDigital = false;
                        UpdateTime(null);
                    }
                }
                else if (button == Buttons.BottomRight)
                {

                    if (showDigital == false)
                    {
                        ++displayMode;
                        if (displayMode > MAX_DISPLAY_MODE)
                        {
                            displayMode = 0;
                        }
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

                }
            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK_12:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;

                    break;

                case DISPLAY_MODE_BLACK_24:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;

                    break;

                case DISPLAY_MODE_WHITE_12:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;

                    break;

                case DISPLAY_MODE_WHITE_24:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;

                    break;

            }

        }

        private static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;


            if (showDigital == false || showDigitalCounter > SHOW_DIGITAL_SECOND)
            {
                showDigital = false;
                UpdateTime(null);
                _updateClockTimerDigital.Dispose();
            }
            else
            {
                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                _display.Flush();
                showDigitalCounter++;
            }

        }

    }

}

