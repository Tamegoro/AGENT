using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Afterimage
{
    public class Afterimage
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontSmall = Resources.GetFont(Resources.FontResources.small);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int lengthDecrease = 0;

        static int displayMode = DISPLAY_MODE_BLACK_FILHAND;

        static int handType = HAND_TYPE_FRAME;

        static int radiusPinOuter = RADIUS_PIN_FILOUTER;
        static int radiusPinInner = RADIUS_PIN_FILINNER;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static int showDecreaseCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int SHOW_DECREASE_SECOND = 3;

        const int MAX_DISPLAY_MODE = 3;

        const int LENGTH_HOUR_HAND = 45;
        const int THIKNESS_HOUR_HAND = 5;

        const int LENGTH_MINUTE_HAND = 61;
        const int THIKNESS_MINUTE_HAND = 5;

        const int LENGTH_SECOND_HAND = 60;
        const int THIKNESS_SECOND_HAND = 1;

        const int RADIUS_PIN_FILOUTER = 3;
        const int RADIUS_PIN_FRAOUTER = 3;
        const int RADIUS_PIN_FILINNER = 1;
        const int RADIUS_PIN_FRAINNER = 2;

        const int LENGTH_MAX_DECREASE = 10;
        
        const int MARGIN_DECREASE_X = 10;
        const int MARGIN_DECREASE_Y = 9;

        const int DISPLAY_MODE_BLACK_FRAHAND = 0;
        const int DISPLAY_MODE_BLACK_FILHAND = 1;
        const int DISPLAY_MODE_WHITE_FRAHAND = 2;
        const int DISPLAY_MODE_WHITE_FILHAND = 3;

        const int HAND_TYPE_FILL = 0;
        const int HAND_TYPE_FRAME = 6;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_FRAHAND;
            SetDisplayMode();

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            lengthDecrease = 1;
            showDecreaseCounter = SHOW_DECREASE_SECOND + 1;

            UpdateTime(null);

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

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_SECOND_HAND, degreeS, screenCenterX, screenCenterY, 0, LENGTH_SECOND_HAND, handType);

                DrawAfterimage();

                _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND, handType);

                _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, handType);


                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, radiusPinOuter, radiusPinOuter, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, radiusPinInner, radiusPinInner, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (0 < showDecreaseCounter)
                {
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, screenWidth - MARGIN_DECREASE_X, screenHeight - MARGIN_DECREASE_Y, lengthDecrease.ToString("D2"));
                    --showDecreaseCounter;
                }

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

        static void DrawAfterimage()
        {

            for (int i = 1; (lengthDecrease * i) <= LENGTH_SECOND_HAND; i++)
            {
                _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_SECOND_HAND, (degreeS - (360 / 60 * i) + 360) % 360, screenCenterX, screenCenterY, (lengthDecrease * i), LENGTH_SECOND_HAND - (lengthDecrease * i));
            }
        
        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showDigital == false)
                    {

                        if (lengthDecrease < LENGTH_MAX_DECREASE)
                        {
                            ++lengthDecrease;
                        }
                        else
                        {
                            lengthDecrease = 1;
                        }

                        showDecreaseCounter = SHOW_DECREASE_SECOND + 1;
                    
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

                SetDisplayMode();

                UpdateTime(null);

            }

        }

        private static void SetDisplayMode()
        {

            switch (displayMode)
            {


                case DISPLAY_MODE_BLACK_FRAHAND:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    handType = HAND_TYPE_FRAME;
                    radiusPinOuter = RADIUS_PIN_FRAOUTER;
                    radiusPinInner = RADIUS_PIN_FRAINNER;

                    break;

                case DISPLAY_MODE_BLACK_FILHAND:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    handType = HAND_TYPE_FILL;
                    radiusPinOuter = RADIUS_PIN_FILOUTER;
                    radiusPinInner = RADIUS_PIN_FILINNER;

                    break;

                case DISPLAY_MODE_WHITE_FRAHAND:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    handType = HAND_TYPE_FRAME;
                    radiusPinOuter = RADIUS_PIN_FRAOUTER;
                    radiusPinInner = RADIUS_PIN_FRAINNER;

                    break;

                case DISPLAY_MODE_WHITE_FILHAND:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    handType = HAND_TYPE_FILL;
                    radiusPinOuter = RADIUS_PIN_FILOUTER;
                    radiusPinInner = RADIUS_PIN_FILINNER;

                    break;


            }

        }

    }

}

