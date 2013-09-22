using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace SemiTransparent
{
    public class SemiTransparent
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font fontSmall = Resources.GetFont(Resources.FontResources.small);
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

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static int dialType = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int THICKNESS_FILL_CIRCLE = 20;
        const int RADIUS_FILL_CIRCLE = 77;

        const int MARGIN_OUTER_CIRCLE_EDGE = 3;
        const int MARGIN_INNER_CIRCLE_OUTER_CIRCLE = 5;
        const int MARGIN_MINUTE_NUMBER_OUTER_CIRCLE = 2;
        const int DISTANCE_HOUR = 25;
        const int DISTANCE_MINUTE = 44;

        const int MAX_DISPLAY_MODE = 5;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DIAL_12 = 1;
        const int DISPLAY_MODE_BLACK_DIAL_60 = 2;
        const int DISPLAY_MODE_WHITE = 3;
        const int DISPLAY_MODE_WHITE_DIAL_12 = 4;
        const int DISPLAY_MODE_WHITE_DIAL_60 = 5;


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

            if (showDigital == false)
            {

                currentTime = DateTime.Now;

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 0; i < 128; i++)
                {
                    for (int j = 0; j < 128; j++)
                    {
                        if ((i + j) % 2 == 0)
                        {
                            _display.SetPixel(i, j, colorForeground);
                        }
                    }
                }

                _display.DrawEllipse(colorBackground, THICKNESS_FILL_CIRCLE, screenCenterX, screenCenterY, RADIUS_FILL_CIRCLE, RADIUS_FILL_CIRCLE, colorBackground, 0, 0, colorBackground, 0, 0, 0);

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_OUTER_CIRCLE_EDGE, screenCenterY - MARGIN_OUTER_CIRCLE_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), screenCenterY - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), colorForeground, 0, 0, colorForeground, 0, 0, 0);

                for (int i = 0; i < dialType; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, (360 / dialType) * i, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), MARGIN_INNER_CIRCLE_OUTER_CIRCLE);
                }

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, DISTANCE_MINUTE);
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, 9, 9, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, 9, 9, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                _point = _azmdrawing.FindPointDegreeDistance(0, screenCenterX, screenCenterY, DISTANCE_MINUTE);
                _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X + 1, _point.Y, "00");

                for (int i = 1; i < 12; i++)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, DISTANCE_MINUTE);
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, (i * 5).ToString("D2"));
                }

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, DISTANCE_HOUR);
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, 9, 9, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, 9, 9, colorForeground, 0, 0, colorForeground, 0, 0, 0);

                for (int i = 1; i <= 12; i++)
                {
                    _point = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, DISTANCE_HOUR);
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontSmall, _point.X, _point.Y, i.ToString());
                }

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

                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    dialType = 0;

                    break;

                case DISPLAY_MODE_BLACK_DIAL_12:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    dialType = 12;

                    break;

                case DISPLAY_MODE_BLACK_DIAL_60:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    dialType = 60;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    dialType = 0;

                    break;

                case DISPLAY_MODE_WHITE_DIAL_12:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    dialType = 12;

                    break;

                case DISPLAY_MODE_WHITE_DIAL_60:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    dialType = 60;

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

