using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace OutOfPosition2
{
    public class OutOfPosition2
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

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int pinX = 0;
        static int pinY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_SQUAR;

        static int handType = HAND_TYPE_SQUARE;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int HAND_TYPE_SQUARE = 0;
        const int HAND_TYPE_FRAME = 6;


        const int LENGTH_HOUR_HAND = 25;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 5;
        const int LENGTH_MINUTE_HAND = 35;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 4;

        const int RADIUS_PIN = 1;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_SQUAR = 0;
        const int DISPLAY_MODE_BLACK_FRAME = 1;
        const int DISPLAY_MODE_WHITE_SQUAR = 2;
        const int DISPLAY_MODE_WHITE_FRAME = 3;



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            colorForeground = new Color();
            colorBackground = new Color();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_SQUAR;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

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

            currentTime = DateTime.Now;

            if (showDigital == false)
            {

                DrawFace();

            }

        }

        private static void DrawFace()
        {


            degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute) % 360;
            degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute) % 360;

            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND);
            _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, handType);

            if (handType == HAND_TYPE_FRAME)
            {
                _display.DrawEllipse(colorForeground, 0, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            }
            else
            {
                _display.DrawEllipse(colorBackground, 0, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorBackground, 0, 0, colorBackground, 0, 0, 255);
            }

            _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND - LENGTH_MINUTE_HAND_TAIL);

            pinX = _point.X;
            pinY = _point.Y;

            _point = _azmdrawing.FindPointDegreeDistance(degreeH + 180, _point.X, _point.Y, LENGTH_HOUR_HAND_TAIL);
            _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH , _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, handType);


            if (handType == HAND_TYPE_FRAME)
            {
                _display.DrawEllipse(colorForeground, 0, pinX, pinY, RADIUS_PIN, RADIUS_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            }
            else
            {
                _display.DrawEllipse(colorBackground, 0, pinX, pinY, RADIUS_PIN, RADIUS_PIN, colorBackground, 0, 0, colorBackground, 0, 0, 255);
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
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

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
                        UpdateTime(null);
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
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

                }
            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK_SQUAR:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    handType = HAND_TYPE_SQUARE;

                    break;

                case DISPLAY_MODE_BLACK_FRAME:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    handType = HAND_TYPE_FRAME;

                    break;

                case DISPLAY_MODE_WHITE_SQUAR:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    handType = HAND_TYPE_SQUARE;

                    break;

                case DISPLAY_MODE_WHITE_FRAME:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    handType = HAND_TYPE_FRAME;

                    break;

            }

        }

        private static void UpdateTimeDigital(object state)
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

