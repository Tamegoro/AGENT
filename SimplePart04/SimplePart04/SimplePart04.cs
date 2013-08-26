using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace SimplePart04
{
    public class SimplePart04
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

        static Color colorForeground;
        static Color colorBackground;
        static Color colorDisk;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 3;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 30;
        const int THICKNESS_HOUR_HAND = 4;
        const int LENGTH_MINUTE_HAND = 40;
        const int THICKNESS_MINUTE_HAND = 4;

        const int LENGTH_DIAL = 10;
        const int THIKNESS_DIAL = 4;
        const int THIKNESS_RIM = 4;
        const int RADIUS_PIN = 5;

        const int MARGIN_DISK_EDGE = 3;
        const int MARGIN_DIAL_DISK = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DISK = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_DISK = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK;
            colorForeground = Color.White;
            colorBackground = Color.Black;
            colorDisk = Color.Black;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

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

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_BLACK_DISK || displayMode == DISPLAY_MODE_WHITE_DISK)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }
                else if (displayMode == DISPLAY_MODE_BLACK || displayMode == DISPLAY_MODE_WHITE)
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE - THIKNESS_RIM, screenCenterY - MARGIN_DISK_EDGE - THIKNESS_RIM, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }

                for (int i = 0; i < 12; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_DIAL, 30 * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_DISK_EDGE + THIKNESS_RIM + MARGIN_DIAL_DISK + LENGTH_DIAL), LENGTH_DIAL);
                }

                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND);

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);

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


                    case DISPLAY_MODE_BLACK:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DISK:

                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DISK:

                        colorForeground = Color.Black;
                        colorBackground = Color.Black;
                        colorDisk = Color.White;
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

