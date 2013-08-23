using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace TimeFlies
{
    public class TimeFlies
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan periodNomal;
        static TimeSpan periodOnePointFive;
        static TimeSpan periodDouble;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

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

        static int displayMode = DISPLAY_MODE_BLACK_SQUHAND_NOMAL;

        static int handType = HAND_TYPE_POINT;
        static int speedRate = SPEED_NOMAL;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

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


        const int MARGIN_DIAL_EDGE = 3;
        const int MARGIN_NUMBER_DIAL = -3;
        const int MARGIN_SPEED_EDGE = 5;


        const int MAX_DISPLAY_MODE = 11;

        const int HAND_TYPE_SQUARE = 0;
        const int HAND_TYPE_POINT = 1;

        const int SPEED_NOMAL = 0;
        const int SPEED_ONE_POINT_FIVE = 1;
        const int SPEED_DOUBLE = 2;

        const int DISPLAY_MODE_BLACK_SQUHAND_NOMAL = 0;
        const int DISPLAY_MODE_BLACK_POIHAND_NOMAL = 1;
        const int DISPLAY_MODE_BLACK_SQUHAND_ONE_POINT_FIVE = 2;
        const int DISPLAY_MODE_BLACK_POIHAND_ONE_POINT_FIVE = 3;
        const int DISPLAY_MODE_BLACK_SQUHAND_DOUBLE = 4;
        const int DISPLAY_MODE_BLACK_POIHAND_DOUBLE = 5;
        const int DISPLAY_MODE_WHITE_SQUHAND_NOMAL = 6;
        const int DISPLAY_MODE_WHITE_POIHAND_NOMAL = 7;
        const int DISPLAY_MODE_WHITE_SQUHAND_ONE_POINT_FIVE = 8;
        const int DISPLAY_MODE_WHITE_POIHAND_ONE_POINT_FIVE = 9;
        const int DISPLAY_MODE_WHITE_SQUHAND_DOUBLE = 10;
        const int DISPLAY_MODE_WHITE_POIHAND_DOUBLE = 11;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_SQUHAND_NOMAL;
            handType = HAND_TYPE_SQUARE;
            speedRate = SPEED_NOMAL;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 0, 0);

            periodNomal = new TimeSpan(0, 0, 0, 1, 0);
            periodOnePointFive = new TimeSpan(0, 0, 0, 0, 1000 / 3 * 2);
            periodDouble = new TimeSpan(0, 0, 0, 0, 1000 / 2);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, periodNomal);

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

            if (showDigital == false)
            {

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawDial(_display, colorForeground, colorBackground, 3, MARGIN_DIAL_EDGE);
                _azmdrawing.DrawHourNumbers(_display, colorForeground, fontNinaB, screenCenterX - (MARGIN_DIAL_EDGE + fontNinaB.Height + MARGIN_NUMBER_DIAL), 0);

                switch (speedRate)
                {

                    case SPEED_NOMAL:

                        degreeS = _azmdrawing.SecondToAngle(currentTime.Second);
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontNinaB, "x1.0", AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_BOTTOM, MARGIN_DIAL_EDGE + fontNinaB.Height + MARGIN_NUMBER_DIAL + MARGIN_SPEED_EDGE);

                        break;

                    case SPEED_ONE_POINT_FIVE:

                        degreeS = _azmdrawing.SecondToAngle(currentTime.Second) + (6 * currentTime.Millisecond / 1000);
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontNinaB, "x1.5", AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_BOTTOM, MARGIN_DIAL_EDGE + fontNinaB.Height + MARGIN_NUMBER_DIAL + MARGIN_SPEED_EDGE);

                        break;

                    case SPEED_DOUBLE:

                        degreeS = _azmdrawing.SecondToAngle(currentTime.Second) + (6 * currentTime.Millisecond / 1000);
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontNinaB, "x2.0", AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_BOTTOM, MARGIN_DIAL_EDGE + fontNinaB.Height + MARGIN_NUMBER_DIAL + MARGIN_SPEED_EDGE);

                        break;

                }

                switch (handType)
                {

                    case HAND_TYPE_POINT:

                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH - 180, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL, handType);

                        break;

                    case HAND_TYPE_SQUARE:

                        _point = _azmdrawing.FindPointDegreeDistance(degreeH + 180, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, handType);
                        _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, handType);
                        _point = _azmdrawing.FindPointDegreeDistance(degreeS + 180, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point.X, _point.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL, handType);

                        break;

                }

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                //_display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 3, 3, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                //_display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);

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

                    case DISPLAY_MODE_BLACK_SQUHAND_NOMAL:

                        handType = HAND_TYPE_SQUARE;
                        speedRate = SPEED_NOMAL;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_NOMAL:

                        handType = HAND_TYPE_POINT;
                        speedRate = SPEED_NOMAL;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_NOMAL:

                        handType = HAND_TYPE_SQUARE;
                        speedRate = SPEED_NOMAL;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_NOMAL:

                        handType = HAND_TYPE_POINT;
                        speedRate = SPEED_NOMAL;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_ONE_POINT_FIVE:

                        handType = HAND_TYPE_SQUARE;
                        speedRate = SPEED_ONE_POINT_FIVE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_ONE_POINT_FIVE:

                        handType = HAND_TYPE_POINT;
                        speedRate = SPEED_ONE_POINT_FIVE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_ONE_POINT_FIVE:

                        handType = HAND_TYPE_SQUARE;
                        speedRate = SPEED_ONE_POINT_FIVE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_ONE_POINT_FIVE:

                        handType = HAND_TYPE_POINT;
                        speedRate = SPEED_ONE_POINT_FIVE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_DOUBLE:

                        handType = HAND_TYPE_SQUARE;
                        speedRate = SPEED_DOUBLE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_DOUBLE:

                        handType = HAND_TYPE_POINT;
                        speedRate = SPEED_DOUBLE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_DOUBLE:

                        handType = HAND_TYPE_SQUARE;
                        speedRate = SPEED_DOUBLE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_DOUBLE:

                        handType = HAND_TYPE_POINT;
                        speedRate = SPEED_DOUBLE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;

                        break;

                }

                _updateClockTimer.Dispose();

                switch (speedRate)
                {

                    case SPEED_NOMAL:

                        _updateClockTimer = new Timer(UpdateTime, null, dueTime, periodNomal);

                        break;

                    case SPEED_ONE_POINT_FIVE:

                        _updateClockTimer = new Timer(UpdateTime, null, dueTime, periodOnePointFive);

                        break;

                    case SPEED_DOUBLE:

                        _updateClockTimer = new Timer(UpdateTime, null, dueTime, periodDouble);

                        break;

                }

                UpdateTime(null);

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

