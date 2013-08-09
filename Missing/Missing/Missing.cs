using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Missing
{
    public class Missing
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerHands;
        static TimeSpan dueTimeHands;
        static TimeSpan periodHands;

        static DateTime currentTime;

        static Font fontNinaB = Resources.GetFont(Resources.FontResources.NinaB);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_ROUNDED_WHITE;
        static bool showHands = false;
        static int showHandsCounter = 0;

        static int hourNum = 0;

        const int MAX_DISPLAY_MODE = 1;

        const int DISTANCE_HOUR = 51;
        const int DISTANCE_MINUTE = 59;

        const int SHOW_HANDS_SECOND = 10;
        
        const int LENGTH_HOUR_HAND = 30;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 3;
        const int LENGTH_MINUTE_HAND = 40;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 3;

        const int DISPLAY_MODE_ROUNDED_WHITE = 0;
        const int DISPLAY_MODE_ROUNDED_BLACK = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_ROUNDED_WHITE;
            colorForeground = Color.Black;
            colorBackground = Color.White;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

            dueTimeHands = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodHands = new TimeSpan(0, 0, 0, 1, 0);

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

            if (showHands != true)
            {

                currentTime = DateTime.Now;

                _display.Clear();

                _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, fontNinaB, 2);

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, 0);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, DISTANCE_HOUR);

                hourNum = currentTime.Hour % 12;

                if (hourNum == 0)
                {
                    hourNum = 12;
                }

                if (1 <= hourNum && hourNum  <= 2)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX + 1, screenCenterY, DISTANCE_HOUR);
                }
                else if (3 <= hourNum && hourNum  <= 3)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX + 2, screenCenterY, DISTANCE_HOUR);
                }
                else if (4 <= hourNum && hourNum  <= 6)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX + 1, screenCenterY + 1, DISTANCE_HOUR);
                }
                else if (7 <= hourNum && hourNum  <= 9)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX, screenCenterY + 1, DISTANCE_HOUR);
                }
                else if (hourNum == 10)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX, screenCenterY, DISTANCE_HOUR);
                }
                else if (hourNum == 11)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX, screenCenterY, DISTANCE_HOUR);
                }
                else if (hourNum  == 12)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX, screenCenterY, DISTANCE_HOUR);
                }

/*
                if (hourNum == 10)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX, screenCenterY, DISTANCE_HOUR - (fontNinaB.CharWidth('1') / 2));
                }
                if (hourNum == 11)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * hourNum, screenCenterX, screenCenterY, DISTANCE_HOUR - (fontNinaB.CharWidth('1') / 3));
                }
                else if (hourNum != 10 && hourNum != 11)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, DISTANCE_HOUR);
                }
*/

                _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaB, _point.X - 1, _point.Y - 1, hourNum.ToString());
                _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaB, _point.X, _point.Y, hourNum.ToString());

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, DISTANCE_MINUTE);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);


            }

            _display.Flush();

        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showHands == false)
                    {
                        --displayMode;
                        if (displayMode < 0)
                        {
                            displayMode = MAX_DISPLAY_MODE;
                        }
                    }
                    else
                    {
                        showHands = false;
                    }
                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showHands != true)
                    {
                        showHands = true;
                        showHandsCounter = 0;
                        UpdateTimeDigital(null);
                        _updateClockTimerHands = new Timer(UpdateTimeDigital, null, dueTimeHands, periodHands);
                    }
                    else
                    {
                        showHands = false;
                    }
                }
                else if (button == Buttons.BottomRight)
                {
                    if (showHands == false)
                    {
                        ++displayMode;
                        if (displayMode > MAX_DISPLAY_MODE)
                        {
                            displayMode = 0;
                        }
                    }
                    else
                    {
                        showHands = false;
                    }
                }

                switch (displayMode)
                {

                    case DISPLAY_MODE_ROUNDED_WHITE:

                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUNDED_BLACK:

                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                }

            }

        }

        static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;


            if (showHands == false || showHandsCounter > SHOW_HANDS_SECOND)
            {
                showHands = false;
                UpdateTime(null);
                _updateClockTimerHands.Dispose();
            }
            else
            {

                currentTime = DateTime.Now;

                _display.Clear();

                _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, fontNinaB, 2);

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, 51);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH + 180, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL);
                _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.Flush();

                ++showHandsCounter;

                if (showHandsCounter > 10)
                {
                    showHands = false;
                }


            }


        }

    }

}

