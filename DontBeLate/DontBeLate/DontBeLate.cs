using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace DontBeLate
{
    public class DontBeLate
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontNinaB = Resources.GetFont(Resources.FontResources.NinaB);

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

        static int adjustMinute = 0;
        static bool showAdjustMinute = false;
        static int showAdjustMinuteCounter = 0;

        static bool showDigital = false;
        static int showDigitalCounter = 0;


        const int SHOW_ADJUST_SECOND = 5;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 30;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 4;
        const int LENGTH_MINUTE_HAND = 39;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 4;
        const int LENGTH_SECOND_HAND = 39;
        const int LENGTH_SECOND_HAND_TAIL = 10;
        const int THICKNESS_SECOND_HAND = 1;

        const int RADIUS_CLEAR = 41;

        const int MARGIN_DIAL_EDGE = 3;
        const int MARGIN_NUMBER_DIAL = -3;
        const int MARGIN_ADJUST_EDGE = 11;

        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            colorForeground = Color.White;
            colorBackground = Color.Black;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            DrawHourDial();
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

            //_display.Clear();

            if (showDigital == false)
            {

                currentTime = currentTime.AddMinutes((double)adjustMinute);

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _display.DrawEllipse(colorBackground, screenCenterX, screenCenterY, RADIUS_CLEAR, RADIUS_CLEAR);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH + 180, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL);
                _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL);
                _point = _azmdrawing.FindPointDegreeDistance(degreeS + 180, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point.X, _point.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                if (showAdjustMinute == true)
                {

                    if (showAdjustMinuteCounter <= SHOW_ADJUST_SECOND)
                    {
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontNinaB, "+" + adjustMinute.ToString("D2") + "min", AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_BOTTOM, MARGIN_DIAL_EDGE + fontNinaB.Height + MARGIN_NUMBER_DIAL + MARGIN_ADJUST_EDGE);
                        showAdjustMinuteCounter++;
                    }
                    else
                    {
                        showAdjustMinuteCounter = 0;
                        showAdjustMinute = false;
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
                    DrawHourDial();
                    UpdateTime(null);
                }

            }

            _display.Flush();

        }

        private static void DrawHourDial()
        {

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _azmdrawing.DrawDial(_display, colorForeground, colorBackground, 3, MARGIN_DIAL_EDGE);
            _azmdrawing.DrawHourNumbers(_display, colorForeground, fontNinaB, screenCenterX - (MARGIN_DIAL_EDGE + fontNinaB.Height + MARGIN_NUMBER_DIAL), 0);

        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showDigital == false)
                    {
                        --adjustMinute;
                        //if (adjustMinute < 0)
                        //{
                        //    adjustMinute = 0;
                        //}
                        adjustMinute = (adjustMinute + 60) % 60;
                        showAdjustMinute = true;
                        showAdjustMinuteCounter = 0;
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
                        ++adjustMinute;
                        //if (59 < adjustMinute)
                        //{
                        //    adjustMinute = 59;
                        //}
                        adjustMinute = (adjustMinute + 60) % 60;
                        showAdjustMinute = true;
                        showAdjustMinuteCounter = 0;
                    }
                    else
                    {
                        showDigital = false;
                    }
                }

                UpdateTime(null);

            }

        }

    }

}

