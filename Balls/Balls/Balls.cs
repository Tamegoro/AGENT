using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Balls
{
    public class Balls
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showNumber = false;
        static bool showDial = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int RADISU_HOUR = 7;
        const int RADISU_MINUTE = 4;
        const int RADISU_SECOND = 2;

        const int MARGIN_NUMBER_EDGE = 7;
        const int MARGIN_DIAL_EDGE = 4;
        const int MARGIN_BALL_NUMBER = 1;

        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK_DIAL = 0;
        const int DISPLAY_MODE_BLACK_NUMBER = 1;
        const int DISPLAY_MODE_BLACK_NUMBER_DIAL = 2;
        const int DISPLAY_MODE_BLACK = 3;
        const int DISPLAY_MODE_WHITE_DIAL = 4;
        const int DISPLAY_MODE_WHITE_NUMBER = 5;
        const int DISPLAY_MODE_WHITE_NUMBER_DIAL = 6;
        const int DISPLAY_MODE_WHITE = 7;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_DIAL;
            showNumber = false;
            showDial = true;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

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

            _display.Clear();


            if (showDigital == false)
            {

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - (fontsmall.Height / 2) - MARGIN_BALL_NUMBER - RADISU_HOUR);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADISU_HOUR, RADISU_HOUR, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - (fontsmall.Height / 2) - MARGIN_BALL_NUMBER - RADISU_MINUTE);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADISU_MINUTE, RADISU_MINUTE, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - (fontsmall.Height / 2) - MARGIN_BALL_NUMBER - RADISU_SECOND);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADISU_SECOND, RADISU_SECOND, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                if (showNumber == true)
                {
                    _azmdrawing.DrawHourNumbers(_display, colorForeground, fontsmall, screenCenterX - MARGIN_NUMBER_EDGE, 5);
                }

                if (showDial == true)
                {
                    if (showNumber == true)
                    {
                        _azmdrawing.DrawDial(_display, colorForeground, colorBackground, 7, MARGIN_DIAL_EDGE);
                    }
                    else
                    {
                        _azmdrawing.DrawDial(_display, colorForeground, colorBackground, 6, MARGIN_DIAL_EDGE);
                    }
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

                switch (displayMode)
                {

                    case DISPLAY_MODE_BLACK:

                        showNumber = false;
                        showDial = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NUMBER:

                        showNumber = true;
                        showDial = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DIAL:

                        showNumber = false;
                        showDial = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NUMBER_DIAL:

                        showNumber = true;
                        showDial = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        showNumber = false;
                        showDial = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NUMBER:

                        showNumber = true;
                        showDial = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DIAL:

                        showNumber = false;
                        showDial = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NUMBER_DIAL:

                        showNumber = true;
                        showDial = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

