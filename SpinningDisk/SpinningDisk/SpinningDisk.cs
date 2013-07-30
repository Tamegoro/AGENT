using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;

namespace SpinningDisk
{
    public class SpinningDisk
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Bitmap _background;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static Color colorForeground;
        static Color colorBackground;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int backgroundWidth = 0;
        static int backgroundHeight = 0;


        static int displayMode = DISPLAY_MODE_ANALOG;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 2;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 30;
        const int LENGTH_HOUR_HAND_TAIL = 12;
        const int LENGTH_MINUTE_HAND = 41;
        const int LENGTH_MINUTE_HAND_TAIL = 12;

        const int DISK_RADIUS = 63;
        
        const int DISPLAY_MODE_ANALOG = 0;
        const int DISPLAY_MODE_DIGITAL = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _background = new Bitmap(Resources.GetBytes(Resources.BinaryResources.SpinningDiskBackground), Bitmap.BitmapImageType.Gif);

            currentTime = new DateTime();

            colorForeground = new Color();
            colorBackground = new Color();

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            backgroundWidth = _background.Width;
            backgroundHeight = _background.Height;

            displayMode = DISPLAY_MODE_ANALOG;
            colorForeground = Color.Black;
            colorBackground = Color.White;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
        {

            if (showDigital != true)
            {

                currentTime = DateTime.Now;

                _display.Clear();

                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute) - degreeM;

                if (degreeM == 0)
                {
                    _display.DrawImage(1, 1, _background, 0, 0, backgroundWidth, backgroundHeight);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, DISK_RADIUS - 3, DISK_RADIUS - 3, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                    _display.DrawEllipse(colorForeground, 2, screenCenterX, screenCenterY, DISK_RADIUS, DISK_RADIUS, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }
                else
                {
                    _display.RotateImage(360 - degreeM, 1, 1, _background, 0, 0, backgroundWidth, backgroundHeight, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, DISK_RADIUS - 3, DISK_RADIUS - 3, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                    _display.DrawEllipse(colorForeground, 2, screenCenterX, screenCenterY, DISK_RADIUS, DISK_RADIUS, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }

                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH + 180, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, 0, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL);
                
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.Flush();

            }


        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {

                if (button == Buttons.MiddleRight)
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

