using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace NumeralHand2
{
    public class NumeralHand2
    {

        static Bitmap _display;
        static Bitmap _bmpRotate;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontNumberHand14 = Resources.GetFont(Resources.FontResources.NumberHand14);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_12;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_DIAL_EDGE = 3;
        const int MARGIN_CENTER_HAND = 6;

        const int CENTER_PIN_SIZE = 6;

        const int LENGTH_DIAL_LONG = 5;
        const int THICKNESS_DIAL_LONG = 2;
        const int LENGTH_DIAL_SHORT = 1;
        const int THICKNESS_DIAL_SHORT = 2;

        const int HAND_HEIGHT = 10;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_12 = 0;
        const int DISPLAY_MODE_BLACK_24 = 1;
        const int DISPLAY_MODE_WHITE_12 = 2;
        const int DISPLAY_MODE_WHITE_24 = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);
            _bmpRotate = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_12;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

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

            currentTime = DateTime.Now;

            if (showDigital == false)
            {

                string hhmm = "";

                if (displayMode == DISPLAY_MODE_BLACK_12 || displayMode == DISPLAY_MODE_WHITE_12)
                {
                    hhmm = (currentTime.Hour % 12).ToString("D2") + ":" + currentTime.Minute.ToString("D2");
                }
                else
                {
                    hhmm = currentTime.Hour.ToString("D2") + ":" + currentTime.Minute.ToString("D2");
                }

                currentTime = DateTime.Now;

                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _display.Clear();
                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (degreeS != 90)
                {

                    _bmpRotate.Clear();
                    _bmpRotate.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                    _bmpRotate.DrawText(hhmm, fontNumberHand14, colorForeground, screenCenterX + MARGIN_CENTER_HAND, screenCenterY - (HAND_HEIGHT / 2));

                    _display.RotateImage((degreeS - 90 + 360) % 360, 0, 0, _bmpRotate, 0, 0, screenWidth, screenHeight, 0);

                }
                else
                {
                    _display.DrawText(hhmm, fontNumberHand14, colorForeground, screenCenterX + MARGIN_CENTER_HAND, screenCenterY - (HAND_HEIGHT / 2));
                }

                for (int i = 0; i < 60; i++)
                {

                    if (i % 5 == 0)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DIAL_LONG, 6 * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DIAL_EDGE + LENGTH_DIAL_LONG), LENGTH_DIAL_LONG);
                    }
                    else
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DIAL_SHORT, 6 * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DIAL_EDGE + LENGTH_DIAL_SHORT), LENGTH_DIAL_SHORT);
                    }

                }


                _display.DrawRectangle(colorForeground, 1, screenCenterX - (CENTER_PIN_SIZE / 2), screenCenterY - (CENTER_PIN_SIZE / 2), CENTER_PIN_SIZE, CENTER_PIN_SIZE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.Flush();

            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > SHOW_DIGITAL_SECOND)
                {
                    showDigital = false;
                    showDigitalCounter = 0;
                }

            }

            _display.Flush();

        }


        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showDigital == false)
                    {
                        if (showDigital == false)
                        {
                            --displayMode;
                            if (displayMode < 0)
                            {
                                displayMode = MAX_DISPLAY_MODE;
                            }
                        }
                        else
                        {
                            showDigital = false;
                        }
                    }
                    else
                    {
                        showDigital = false;
                    }
                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital != true)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
                    }
                    else
                    {
                        showDigital = false;
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
                    }
                    else
                    {
                        showDigital = false;
                    }
                }

                SetDisplayMode(displayMode);

                UpdateTime(null);

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

    }

}

