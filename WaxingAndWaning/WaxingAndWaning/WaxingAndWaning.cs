using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using Agent.Contrib.Hardware;
using AGENT.AZMutil.AZMDrawing;


namespace WaxingAndWaning
{
    public class WaxingAndWaning
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

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;
        
        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int radius = 0;

        static int displayMode = DISPLAY_MODE_WHITE;
        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 2;

        const int SHOW_DIGITAL_SECOND = 10;

        const int DISPLAY_MODE_WHITE = 0;
        const int DISPLAY_MODE_BLACK = 1;
        const int DISPLAY_MODE_HAND = 2;

        const int radiusMod = 4;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            currentTime = new DateTime();

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            radius = screenCenterX - radiusMod;

            displayMode = DISPLAY_MODE_WHITE;

            //displayMode = DISPLAY_MODE_BLACK;

            UpdateTime(null);

            currentTime = DateTime.Now;

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

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _display.Clear();

                _display.DrawRectangle(Color.Black, 1, 0, 0, screenWidth, screenHeight, 0, 0, Color.Black, 0, 0, Color.Black, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_BLACK || displayMode == DISPLAY_MODE_WHITE)
                {

                    _display.DrawEllipse(Color.White, 1, screenCenterX, screenCenterY, radius, radius, Color.White, 0, 0, Color.White, 0, 0, 255);

                    _azmdrawing.DrawAngledLine(_display, Color.Black, 1, degreeH, screenCenterX, screenCenterY, 0, screenCenterX);
                    _azmdrawing.DrawAngledLine(_display, Color.Black, 1, degreeM, screenCenterX, screenCenterY, 0, screenCenterX);

                    if (displayMode == DISPLAY_MODE_WHITE)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(degreeH - 5, screenCenterX, screenCenterY, radius / 2);
                    }
                    else
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(degreeH + 5, screenCenterX, screenCenterY, radius / 2);
                    }

                    //_display.SetPixel(_point.X, _point.Y, Color.Black);

                    _azmdrawing.FillArea(_display, Color.Black, _point.X, _point.Y);

                    _azmdrawing.DrawAngledLine(_display, Color.Black, 1, degreeH, screenCenterX, screenCenterY, 0, radius);
                    _azmdrawing.DrawAngledLine(_display, Color.Black, 1, degreeM, screenCenterX, screenCenterY, 0, radius);

                }
                else
                {
                    _azmdrawing.DrawAngledLine(_display, Color.White, 3, degreeH, screenCenterX, screenCenterY, 0, radius / 5 * 3);
                    _azmdrawing.DrawAngledLine(_display, Color.White, 3, degreeH + 180, screenCenterX, screenCenterY, 0, 10);
                    _azmdrawing.DrawAngledLine(_display, Color.White, 3, degreeM, screenCenterX, screenCenterY, 0, radius / 10 * 9);
                    _azmdrawing.DrawAngledLine(_display, Color.White, 3, degreeM + 180, screenCenterX, screenCenterY, 0,10);
                    _display.DrawEllipse(Color.Black, 1, screenCenterX, screenCenterY, 3, 3, Color.Black, 0, 0, Color.Black, 0, 0, 255);
                    _display.DrawEllipse(Color.White, 1, screenCenterX, screenCenterY, 1, 1, Color.White, 0, 0, Color.White, 0, 0, 255);
                    //_display.DrawEllipse(Color.Black, 1, screenCenterX, screenCenterY, 1, 1, Color.Black, 0, 0, Color.Black, 0, 0, 255);
                }

                _display.Flush();

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
                        UpdateTimeDigital(null);
                        _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
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

                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK:

                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_HAND:

                        UpdateTime(null);

                        break;

                }

            }

        }


        static void UpdateTimeDigital(object state)
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

