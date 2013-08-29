using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace WindowedHands
{
    public class WindowedHands
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;
        static Color colorDisk;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point0;
        static AGENT.AZMutil.Point _point1;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int handType = 0;

        static int displayMode = DISPLAY_MODE_BLACK_SQUHAND;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND_ROU = 36;
        const int LENGTH_HOUR_HAND_TAIL_ROU = 0;
        const int THICKNESS_HOUR_HAND_ROU = 5;
        const int LENGTH_MINUTE_HAND_ROU = 50;
        const int LENGTH_MINUTE_HAND_TAIL_ROU = 0;
        const int THICKNESS_MINUTE_HAND_ROU = 5;
        const int LENGTH_HOUR_HAND_SQU = 42;
        const int LENGTH_HOUR_HAND_TAIL_SQU = 3;
        const int THICKNESS_HOUR_HAND_SQU = 7;
        const int LENGTH_MINUTE_HAND_SQU = 56;
        const int LENGTH_MINUTE_HAND_TAIL_SQU = 3;
        const int THICKNESS_MINUTE_HAND_SQU = 7;

        const int RADIUS_PIN = 3;

        const int MARGIN_DISK_EDGE = 3;
        const int MARGIN_CENTER_HOUR = 34;
        const int MARGIN_CENTER_MINUTE = 47;

        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK_ROUHAND = 0;
        const int DISPLAY_MODE_BLACK_ROUHAND_DISK = 1;
        const int DISPLAY_MODE_BLACK_SQUHAND = 2;
        const int DISPLAY_MODE_BLACK_SQUHAND_DISK = 3;
        const int DISPLAY_MODE_WHITE_ROUHAND = 4;
        const int DISPLAY_MODE_WHITE_ROUHAND_DISK = 5;
        const int DISPLAY_MODE_WHITE_SQUHAND = 6;
        const int DISPLAY_MODE_WHITE_SQUHAND_DISK = 7;

        const int HAND_TYPE_SQU = 0;
        const int HAND_TYPE_ROU = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point0 = new AGENT.AZMutil.Point();
            _point1 = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_ROUHAND;
            handType = HAND_TYPE_ROU;
            colorForeground = Color.White;
            colorBackground = Color.Black;
            colorDisk = Color.Black;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

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

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, 0);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_BLACK_ROUHAND_DISK || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_ROUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_DISK_EDGE, screenCenterY - MARGIN_DISK_EDGE, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }

                switch (handType)
                {

                    case HAND_TYPE_ROU:

                        _point0 = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_ROU);
                        _point1 = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL_ROU);
                        _display.DrawLine(colorForeground, THICKNESS_HOUR_HAND_ROU, _point0.X, _point0.Y, _point1.X, _point1.Y);

                        DrawHourNumber(_display);

                        _point0 = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_ROU);
                        _point1 = _azmdrawing.FindPointDegreeDistance((degreeM + 180) % 360, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL_ROU);
                        _display.DrawLine(colorForeground, THICKNESS_MINUTE_HAND_ROU, _point0.X, _point0.Y, _point1.X, _point1.Y);

                        DrawMinuteNumber(_display);

                        break;

                    case HAND_TYPE_SQU:

                        _point0 = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL_SQU);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND_SQU, degreeH, _point0.X, _point0.Y, 0, LENGTH_HOUR_HAND_SQU + LENGTH_HOUR_HAND_TAIL_SQU);

                        DrawHourNumber(_display);

                        _point0 = _azmdrawing.FindPointDegreeDistance((degreeM + 180) % 360, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL_SQU);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND_SQU, degreeM, _point0.X, _point0.Y, 0, LENGTH_MINUTE_HAND_SQU + LENGTH_MINUTE_HAND_TAIL_SQU);

                        DrawMinuteNumber(_display);

                        break;

                }

                if (displayMode == DISPLAY_MODE_BLACK_ROUHAND_DISK || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_ROUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorDisk, 0, 0, colorDisk, 0, 0, 0);
                }
                else
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                }

                _display.Flush();

            }

        }

        private static void DrawHourNumber(Bitmap screen)
        {

            if (displayMode == DISPLAY_MODE_BLACK_ROUHAND_DISK || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_ROUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK)
            {

                for (int i = 1; i <= 12; i++)
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorDisk, fontsmall, i.ToString(), 30 * i, MARGIN_CENTER_HOUR);
                }

            }
            else
            {

                for (int i = 1; i <= 12; i++)
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorBackground, fontsmall, i.ToString(), 30 * i, MARGIN_CENTER_HOUR);
                }

            }

        }

        private static void DrawMinuteNumber(Bitmap screen)
        {

            if (displayMode == DISPLAY_MODE_BLACK_ROUHAND_DISK || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_ROUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK)
            {

                for (int i = 0; i < 12; i++)
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorDisk, fontsmall, (5 * i).ToString("D2"), 30 * i, MARGIN_CENTER_MINUTE);
                }

            }
            else
            {

                for (int i = 0; i < 12; i++)
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorBackground, fontsmall, (5 * i).ToString("D2"), 30 * i, MARGIN_CENTER_MINUTE);
                }

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


                    case DISPLAY_MODE_BLACK_ROUHAND:

                        handType = HAND_TYPE_ROU;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_ROUHAND_DISK:

                        handType = HAND_TYPE_ROU;
                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND:

                        handType = HAND_TYPE_SQU;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_DISK:

                        handType = HAND_TYPE_SQU;
                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_ROUHAND:

                        handType = HAND_TYPE_ROU;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_ROUHAND_DISK:

                        handType = HAND_TYPE_ROU;
                        colorForeground = Color.Black;
                        colorBackground = Color.Black;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND:

                        handType = HAND_TYPE_SQU;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_DISK:

                        handType = HAND_TYPE_SQU;
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

