using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Rings
{
    public class Rings
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
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

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showNumber = false;
        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int THIKNESS_RING = 7;
        
        const int MARGIN_NUMBER_EDGE = 8;
        const int MARGIN_RING_NUMBER = 6;
        const int MARGIN_RING_RING = 6;

        const int MARGIN_DAY_X = 1;
        const int MARGIN_DAY_Y = 0;

        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_NUMBER = 1;
        const int DISPLAY_MODE_BLACK_DATE = 2;
        const int DISPLAY_MODE_BLACK_NUMBER_DATE = 3;
        const int DISPLAY_MODE_WHITE = 4;
        const int DISPLAY_MODE_WHITE_NUMBER = 5;
        const int DISPLAY_MODE_WHITE_DATE = 6;
        const int DISPLAY_MODE_WHITE_NUMBER_DATE = 7;


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

            displayMode = DISPLAY_MODE_BLACK;
            showDate = false;
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


                for (int i = 0; i <= 2; i++)
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * i), screenCenterY - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * i), colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * i) - THIKNESS_RING, screenCenterY - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * i) - THIKNESS_RING, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }

                _point0 = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * 0) - THIKNESS_RING - 1);
                _point1 = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * 0) - THIKNESS_RING + THIKNESS_RING + 1);
                _display.DrawLine(colorBackground, 4, _point0.X, _point0.Y, _point1.X, _point1.Y);

                _point0 = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * 1) - THIKNESS_RING - 1);
                _point1 = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * 1) - THIKNESS_RING + THIKNESS_RING + 1);
                _display.DrawLine(colorBackground, 4, _point0.X, _point0.Y, _point1.X, _point1.Y);

                _point0 = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * 2) - THIKNESS_RING - 1);
                _point1 = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, screenCenterX - MARGIN_NUMBER_EDGE - MARGIN_RING_NUMBER - ((THIKNESS_RING + MARGIN_RING_RING) * 2) - THIKNESS_RING + THIKNESS_RING + 1);
                _display.DrawLine(colorBackground, 4, _point0.X, _point0.Y, _point1.X, _point1.Y);

                if (showNumber == true)
                {
                    _azmdrawing.DrawHourNumbers(_display, colorForeground, fontsmall, screenCenterX - MARGIN_NUMBER_EDGE, 5);
                }


                if (showDate == true)
                {
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontsmall, screenCenterX, screenCenterY, currentTime.Day.ToString("D2"));
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

                    case DISPLAY_MODE_BLACK:

                        showNumber = false;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NUMBER:

                        showNumber = true;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DATE:

                        showNumber = false;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NUMBER_DATE:

                        showNumber = true;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        showNumber = false;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NUMBER:

                        showNumber = true;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DATE:

                        showNumber = false;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NUMBER_DATE:

                        showNumber = true;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

