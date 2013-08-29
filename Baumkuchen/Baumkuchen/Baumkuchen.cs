using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Baumkuchen
{
    public class Baumkuchen
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

        static int radiusRing = 0;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int THIKNESS_RIM = 2;
        const int THIKNESS_RING = 1;
        const int THIKNESS_ERASE = 10;
        const int MARGINE_RING_RING = 3;
        const int RADIUS_HOLE = 25;
        const int RADIUS_BITE = 20;

        const int MARGIN_DISK_EDGE = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            colorForeground = Color.Black;
            colorBackground = Color.Black;
            colorDisk = Color.White;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            TimeSpan period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

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

                radiusRing = screenCenterX - MARGIN_DISK_EDGE;
                _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorDisk, 0, 0, colorDisk, 0, 0, 255);

                radiusRing = radiusRing - 1;
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                radiusRing = radiusRing - THIKNESS_RIM;
                _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorDisk, 0, 0, colorDisk, 0, 0, 255);

                for (int i = 0; i <= 10; i++)
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    radiusRing = radiusRing - MARGINE_RING_RING;
                }

/*
                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE);
                _display.DrawEllipse(colorForeground, _point.X, _point.Y, RADIUS_BITE, RADIUS_BITE);

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, RADIUS_HOLE - (RADIUS_BITE / 2));
                _display.DrawEllipse(colorForeground, _point.X, _point.Y, RADIUS_BITE, RADIUS_BITE);
*/

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, RADIUS_HOLE - (RADIUS_BITE / 2));
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADIUS_BITE, RADIUS_BITE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorDisk, 1, _point.X, _point.Y, RADIUS_BITE, RADIUS_BITE, colorDisk, 0, 0, colorDisk, 0, 0, 1);

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADIUS_BITE, RADIUS_BITE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorDisk, 1, _point.X, _point.Y, RADIUS_BITE, RADIUS_BITE, colorDisk, 0, 0, colorDisk, 0, 0, 1);

                radiusRing = screenCenterX + THIKNESS_ERASE;
                _display.DrawEllipse(colorBackground, THIKNESS_ERASE + 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorBackground, 0, 0, colorBackground, 0, 0, 0);

                radiusRing = screenCenterX - MARGIN_DISK_EDGE + 1;
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorBackground, 0, 0, colorBackground, 0, 0, 1);

                radiusRing = RADIUS_HOLE;
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, radiusRing, radiusRing, colorBackground, 0, 0, colorBackground, 0, 0, 255);

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

