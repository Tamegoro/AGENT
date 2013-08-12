using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace DiversWatch
{
    public class DiversWatch
    {

        static Bitmap _display;
        static Bitmap bmpBezel;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point0;
        static AGENT.AZMutil.Point _point1;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int degreeBezel = 0;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 25;
        const int LENGTH_HOUR_HAND_TAIL = 8;
        const int THICKNESS_HOUR_HAND = 2;

        const int LENGTH_MINUTE_HAND = 30;
        const int LENGTH_MINUTE_HAND_TAIL = 8;
        const int THICKNESS_MINUTE_HAND = 2;

        const int LENGTH_SECOND_HAND = 28;
        const int LENGTH_SECOND_HAND_TAIL = 8;
        const int THICKNESS_SECOND_HAND = 1;

        const int THIKNESS_BEZEL = 15;

        const int LENGTH_OUTER_DIAL = 2;
        const int RADIUS_INNER_DIAL_CIRCLE = 2;
        const int LENGTH_INNER_DIAL_QUARTER = 10;
        const int GAP_DIAL_QUARTER = 5;

        const int MARGIN_EDGE_BEZEL = 3;
        const int MARGIN_OUTER_DIAL_BEZEL = 1;
        const int MARGIN_INNER_DIAL_OUTER_DIAL = 2;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            bmpBezel = new Bitmap(Resources.GetBytes(Resources.BinaryResources.DiversWatchBezel), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point0 = new AGENT.AZMutil.Point();
            _point1 = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

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

                if (degreeBezel == 0)
                {
                    _display.DrawImage((_display.Width - bmpBezel.Width) / 2 + 1, (_display.Height - bmpBezel.Height) / 2 + 1, bmpBezel, 0, 0, bmpBezel.Width, bmpBezel.Height);
                }
                else 
                {
                    _display.RotateImage(degreeBezel, (_display.Width - bmpBezel.Width) / 2 + 1, (_display.Height - bmpBezel.Height) / 2 + 1, bmpBezel, 0, 0, bmpBezel.Width, bmpBezel.Height, 255);
                }

                for (int i = 0; i < 60; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 6 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL, LENGTH_OUTER_DIAL);
                }


                _point0 = _azmdrawing.FindPointDegreeDistance(0, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL - MARGIN_INNER_DIAL_OUTER_DIAL - LENGTH_INNER_DIAL_QUARTER - 1);
                _point1 = _azmdrawing.FindPointDegreeDistance(0 - 90, _point0.X, _point0.Y, 3);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 3, 0, _point1.X, _point1.Y, 0, LENGTH_INNER_DIAL_QUARTER);
                _point1 = _azmdrawing.FindPointDegreeDistance(0 + 90, _point0.X, _point0.Y, 3);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 3, 0, _point1.X, _point1.Y, 0, LENGTH_INNER_DIAL_QUARTER);

                for (int i = 1; i < 12; i++)
                {

                    if (i % 3 == 0)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 3, 30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL - MARGIN_INNER_DIAL_OUTER_DIAL - LENGTH_INNER_DIAL_QUARTER, LENGTH_INNER_DIAL_QUARTER);
                    }
                    else if (1 <= i && i <= 2)
                    {
                        _point0 = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL - MARGIN_INNER_DIAL_OUTER_DIAL - LENGTH_OUTER_DIAL);
                        _display.DrawEllipse(colorForeground, 1, _point0.X, _point0.Y, RADIUS_INNER_DIAL_CIRCLE, RADIUS_INNER_DIAL_CIRCLE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    }
                    else if (4 <= i && i <= 5)
                    {
                        _point0 = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL - MARGIN_INNER_DIAL_OUTER_DIAL - LENGTH_OUTER_DIAL - 1);
                        _display.DrawEllipse(colorForeground, 1, _point0.X, _point0.Y, RADIUS_INNER_DIAL_CIRCLE, RADIUS_INNER_DIAL_CIRCLE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    }
                    else if (7 <= i && i <= 8)
                    {
                        _point0 = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL - MARGIN_INNER_DIAL_OUTER_DIAL - LENGTH_OUTER_DIAL);
                        _display.DrawEllipse(colorForeground, 1, _point0.X, _point0.Y, RADIUS_INNER_DIAL_CIRCLE, RADIUS_INNER_DIAL_CIRCLE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    }
                    else if (10 <= i && i <= 11)
                    {
                        _point0 = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_BEZEL - THIKNESS_BEZEL - MARGIN_OUTER_DIAL_BEZEL - LENGTH_OUTER_DIAL - MARGIN_INNER_DIAL_OUTER_DIAL - LENGTH_OUTER_DIAL);
                        _display.DrawEllipse(colorForeground, 1, _point0.X, _point0.Y, RADIUS_INNER_DIAL_CIRCLE, RADIUS_INNER_DIAL_CIRCLE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    }
                
                }

                _point0 = _azmdrawing.FindPointDegreeDistance(degreeH + 180, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point0.X, _point0.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 3);

                _point0 = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point0.X, _point0.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 3);

                _point0 = _azmdrawing.FindPointDegreeDistance(degreeS + 180, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point0.X, _point0.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL);

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 4, 4, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);

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
                        degreeBezel += 30;
                        degreeBezel = degreeBezel % 360;
                        UpdateTime(null);
                    }
                    else
                    {
                        showDigital = false;
                        UpdateTime(null);
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
                        UpdateTime(null);
                    }
                }
                else if (button == Buttons.BottomRight)
                {
                    if (showDigital == false)
                    {
                        degreeBezel += 6;
                        degreeBezel = degreeBezel % 360;
                        UpdateTime(null);
                    }
                    else
                    {
                        showDigital = false;
                        UpdateTime(null);
                    }
                }

            }

        }


    }

}

