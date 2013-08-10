using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace TimeGoRound
{
    public class TimeGoRound
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontNinaB = Resources.GetFont(Resources.FontResources.NinaB);
        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
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

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDial = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 3;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 36;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int LENGTH_MINUTE_HAND = 48;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int LENGTH_SECOND_HAND = 48;
        const int LENGTH_SECOND_HAND_TAIL = 10;

        const int RADIUS_DISK_INNER = 30;

        const int THIKNESS_RIM_OUTER = 4;
        const int THIKNESS_RIM_INNER = 3;

        const int MARGIN_EDGE_DISK = 3;
        const int MARGIN_DISK_DISK = 2;


        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DIAL = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_DIAL = 3;


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

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK, screenCenterY - MARGIN_EDGE_DISK, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK, screenCenterY - MARGIN_EDGE_DISK, colorBackground, 0, 0, colorBackground, 0, 0, 0);
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM_OUTER, screenCenterY - MARGIN_EDGE_DISK - THIKNESS_RIM_OUTER, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _point = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, screenCenterX - (MARGIN_EDGE_DISK + THIKNESS_RIM_OUTER + MARGIN_DISK_DISK + RADIUS_DISK_INNER));
                _display.DrawEllipse(colorForeground, 1, _point.X, _point.Y, RADIUS_DISK_INNER, RADIUS_DISK_INNER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, RADIUS_DISK_INNER - THIKNESS_RIM_INNER, RADIUS_DISK_INNER - THIKNESS_RIM_INNER, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH, _point.X, _point.Y, 0, RADIUS_DISK_INNER - THIKNESS_RIM_INNER - 4 - 5, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM, _point.X, _point.Y, 0, RADIUS_DISK_INNER - THIKNESS_RIM_INNER - 4, 0);

                if (showDial == true)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, screenCenterX - (MARGIN_EDGE_DISK + THIKNESS_RIM_OUTER + MARGIN_DISK_DISK + RADIUS_DISK_INNER));

                    for (int i = 0; i < 12; i++)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorBackground, 2, 30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM_OUTER - 1, THIKNESS_RIM_OUTER + 1);
                        _azmdrawing.DrawAngledLine(_display, colorBackground, 2, 30 * i, _point.X, _point.Y, RADIUS_DISK_INNER - THIKNESS_RIM_INNER - 1, THIKNESS_RIM_INNER + 1);
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
                        UpdateTime(null);
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

                        showDial = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DIAL:

                        showDial = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK:

                        showDial = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DIAL:

                        showDial = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

