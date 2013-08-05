using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Moons
{
    public class Moons
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

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

        static int displayMode = DISPLAY_MODE_WHITE;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 1;

        const int SHOW_DIGITAL_SECOND = 10;


        const int RADIUS_BASE = 30;
        const int RADIUS_HOUR = 25;
        const int RADIUS_MINUTE = 20;
        const int RADIUS_SECOND = 1;
        const int MARGIN_HOUR = 2;
        const int MARGIN_MINUTE = 2;
        const int MARGIN_SECOND = 2;


        const int DISPLAY_MODE_WHITE = 0;
        const int DISPLAY_MODE_BLACK = 1;



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_WHITE;
            colorForeground = Color.Black;
            colorBackground = Color.White;

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

                _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _point.X = screenCenterX;
                _point.Y = screenCenterY;
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, RADIUS_BASE * 2, RADIUS_BASE * 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, _point.X, _point.Y, (RADIUS_BASE - RADIUS_HOUR) * 2 - MARGIN_HOUR);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADIUS_HOUR * 2, RADIUS_HOUR * 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, _point.X, _point.Y, (RADIUS_HOUR - RADIUS_MINUTE) * 2 - MARGIN_MINUTE);
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, RADIUS_MINUTE * 2, RADIUS_MINUTE * 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeS, _point.X, _point.Y, (RADIUS_MINUTE - RADIUS_SECOND) * 2 - MARGIN_SECOND);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADIUS_SECOND * 2, RADIUS_SECOND * 2, colorForeground, 0, 0, colorForeground, 0, 0, 255);

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
                        UpdateTime(null);
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

                    case DISPLAY_MODE_WHITE:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

