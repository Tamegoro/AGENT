using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace SimplePart1
{
    public class SimplePart1
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

        static int displayMode = DISPLAY_MODE_WHITE;

        static bool showNumber = false;
        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 5;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 36;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int LENGTH_MINUTE_HAND = 48;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int LENGTH_SECOND_HAND = 48;
        const int LENGTH_SECOND_HAND_TAIL = 10;
        const int DISTANCE_HAND_CIRCLE = 26;
        const int RADIUS_HAND_CIRCLE = 6;

        const int NUMBER_MARGIN = 21;

        const int DATE_WIDTH = 19;
        const int DATE_HEIGHT = 13;
        const int DATE_MARGIN = 25;
        
        const int DISPLAY_MODE_WHITE = 0;
        const int DISPLAY_MODE_BLACK = 1;
        const int DISPLAY_MODE_WHITE_NUMBER = 2;
        const int DISPLAY_MODE_BLACK_NUMBER = 3;
        const int DISPLAY_MODE_WHITE_NUMBER_DATE = 4;
        const int DISPLAY_MODE_BLACK_NUMBER_DATE = 5;


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
            colorForeground = Color.Black;
            colorBackground = Color.White;

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

                _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, null, 7);


                _point = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point.X - 1, _point.Y - 1, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point.X - 1, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point.X, _point.Y - 1, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 0);

                
                _point = _azmdrawing.FindPointDegreeDistance((degreeM + 180) % 360, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point.X - 1, _point.Y - 1, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point.X - 1, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point.X, _point.Y - 1, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 0);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 0);

                _display.DrawEllipse(colorBackground, RADIUS_HAND_CIRCLE - 1, screenCenterX, screenCenterY, DISTANCE_HAND_CIRCLE, DISTANCE_HAND_CIRCLE, colorBackground, 0, 0, colorBackground, 0, 0, 0);

                _point = _azmdrawing.FindPointDegreeDistance((degreeS + 180) % 360, screenCenterX - 1, screenCenterY - 1, LENGTH_SECOND_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, _point.X, _point.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL, 0);

                _point = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX , screenCenterY , DISTANCE_HAND_CIRCLE);
                _azmdrawing.DarwCircle(_display, colorForeground, 2, _point.X, _point.Y, RADIUS_HAND_CIRCLE, RADIUS_HAND_CIRCLE);

                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX , screenCenterY , DISTANCE_HAND_CIRCLE);
                _azmdrawing.DarwCircle(_display, colorForeground, 2, _point.X, _point.Y, RADIUS_HAND_CIRCLE, RADIUS_HAND_CIRCLE);

                _display.DrawEllipse(colorForeground, 1, screenCenterX - 1, screenCenterY - 1, 4, 4, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, screenCenterX - 1, screenCenterY - 1, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX - 1, screenCenterY - 1, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                if (showNumber == true)
                {
                    _azmdrawing.DrawHourNumbers(_display, colorForeground, fontNinaB, screenCenterX - NUMBER_MARGIN, 0);

                }

                if (showDate == true)
                {
                    _display.DrawRectangle(colorForeground, 1, screenWidth - DATE_MARGIN - DATE_WIDTH, screenCenterY - (DATE_HEIGHT / 2), DATE_WIDTH, DATE_HEIGHT, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    _display.DrawText(currentTime.Day.ToString("D2"), fontsmall, colorForeground, screenWidth - DATE_MARGIN - DATE_WIDTH + 4, screenCenterY - (DATE_HEIGHT / 2));
                }


            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > 10)
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

                        showNumber = false;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK:

                        showNumber = false;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NUMBER:

                        showNumber = true;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NUMBER:

                        showNumber = true;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NUMBER_DATE:

                        showNumber = true;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NUMBER_DATE:

                        showNumber = true;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                }

            }

        }


    }

}

