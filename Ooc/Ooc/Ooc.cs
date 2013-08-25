using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Ooc
{
    public class Ooc
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontIPAexGothic08Bold = Resources.GetFont(Resources.FontResources.IPAexGothic08Bold);
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

        static int ringX = 0;
        static int ringY = 0;
        static int ringRadius = 0;

        static int displayMode = DISPLAY_MODE_BLACK;


        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_RING_EDGE = 3;
        const int MARGIN_RING_RING = 4;
        const int THICKNESS_RING = 13;
        const int THICKNESS_GAP = 5;
        const int MOD_RING = 5;

        const int MAX_DISPLAY_MODE = 1;

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

                ringRadius = screenCenterX - MARGIN_RING_EDGE;
                ringX = screenCenterX;
                ringY = screenCenterY;

                _display.DrawEllipse(colorForeground, 1, ringX, ringY, ringRadius, ringRadius, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, ringX, ringY, ringRadius - THICKNESS_RING, ringRadius - THICKNESS_RING, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 1; i <= 12; i++)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(((30 * i) - degreeH + 360) % 360, ringX, ringY, ringRadius - (THICKNESS_RING / 2));
                    _azmdrawing.DrawStringAngled(_display, colorBackground, fontIPAexGothic08Bold, ((30 * i) - degreeH + 360) % 360, _point.X, _point.Y, i.ToString());
                }


                ringRadius = ringRadius - THICKNESS_RING - MARGIN_RING_RING - MOD_RING;
                ringX = screenWidth - (MARGIN_RING_EDGE + THICKNESS_RING + MARGIN_RING_RING + ringRadius);
                ringY = screenHeight - (MARGIN_RING_EDGE + THICKNESS_RING + MARGIN_RING_RING + ringRadius);

                _display.DrawEllipse(colorForeground, 1, ringX, ringY, ringRadius, ringRadius, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, ringX, ringY, ringRadius - THICKNESS_RING, ringRadius - THICKNESS_RING, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 0; i < 60; i++)
                {
                    if (i % 15 == 0)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(((6 * i) - degreeM + 360) % 360, ringX, ringY, ringRadius - (THICKNESS_RING / 2));
                        _azmdrawing.DrawStringAngled(_display, colorBackground, fontIPAexGothic08Bold, ((6 * i) - degreeM + 360) % 360, _point.X, _point.Y, i.ToString("D2"));
                    }
                    else if (i % 5 == 0)
                    {
                        _point = _azmdrawing.FindPointDegreeDistance(((6 * i) - degreeM + 360) % 360, ringX, ringY, ringRadius - (THICKNESS_RING / 2) + 2);
                        _display.DrawEllipse(colorBackground, _point.X, _point.Y, 1, 1);
                    }
                }


                ringRadius = ringRadius - THICKNESS_RING - MARGIN_RING_RING - MOD_RING;
                ringX = screenWidth - (MARGIN_RING_EDGE + THICKNESS_RING + MARGIN_RING_RING + +THICKNESS_RING + MARGIN_RING_RING + ringRadius);
                ringY = screenHeight - (MARGIN_RING_EDGE + THICKNESS_RING + MARGIN_RING_RING + +THICKNESS_RING + MARGIN_RING_RING + ringRadius);

                _display.DrawEllipse(colorForeground, 1, ringX, ringY, ringRadius, ringRadius, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, ringX, ringY, ringRadius - THICKNESS_RING + 3, ringRadius - THICKNESS_RING + 3, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _azmdrawing.DrawAngledLine(_display, colorBackground, THICKNESS_GAP, degreeS, ringX, ringY, ringRadius - THICKNESS_RING - 1, THICKNESS_RING + 3);

/*
                radiusRing = screenCenterX - MARGIN_RING_EDGE;
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, radiusRing - THICKNESS_RING, radiusRing - THICKNESS_RING, colorBackground, 0, 0, colorBackground, 0, 0, 1);

                radiusRing = radiusRing - THICKNESS_RING - MARGIN_RING_RING - MOD_RING;
                _display.DrawEllipse(colorBackground, 1, screenWidth - (MARGIN_RING_EDGE + THICKNESS_RING + MARGIN_RING_RING + radiusRing), screenHeight - (MARGIN_RING_EDGE + THICKNESS_RING + MARGIN_RING_RING + radiusRing), radiusRing - THICKNESS_RING, radiusRing - THICKNESS_RING, colorBackground, 0, 0, colorBackground, 0, 0, 1);
*/

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

