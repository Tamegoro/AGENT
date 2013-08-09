using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace SimplePart02
{
    public class SimplePart02
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

        static bool showDate = false;
        static bool showSecond = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 16;
        const int LENGTH_MINUTE_HAND = 26;
        const int LENGTH_SECOND_HAND = 26;

        const int RADIUS_CIRCLE_CENTER = 10;
        const int MARGIN_CIRCLE_CENTER_HAND = 5;

        const int LENGTH_DIAL = 10;
        const int LENGTH_DIAL_QUARTER = 10;
        const int GAP_DIAL_QUARTER = 5;

        const int MARGIN_DIAL_EDGE = 7;

        const int MARGIN_DAY_X = 1;
        const int MARGIN_DAY_Y = 0;

        const int MAX_DISPLAY_MODE = 7;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_SECOND_BLACK = 1;
        const int DISPLAY_MODE_BLACK_DATE = 2;
        const int DISPLAY_MODE_SECOND_BLACK_DATE = 3;
        const int DISPLAY_MODE_WHITE = 4;
        const int DISPLAY_MODE_SECOND_WHITE = 5;
        const int DISPLAY_MODE_WHITE_DATE = 6;
        const int DISPLAY_MODE_SECOND_WHITE_DATE = 7;


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

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 0; i <= 12; i++)
                {
                    if (i % 3 == 0)
                    {
                        _point0 = _azmdrawing.FindPointDegreeDistance(30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_DIAL_EDGE - LENGTH_DIAL);

                        _point1 = _azmdrawing.FindPointDegreeDistance(30 * i - 90, _point0.X, _point0.Y, GAP_DIAL_QUARTER / 2);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X - 1, _point1.Y - 1, 0, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X - 1, _point1.Y, 0, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X, _point1.Y - 1, 0, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X, _point1.Y, 0, LENGTH_DIAL);

                        _point1 = _azmdrawing.FindPointDegreeDistance(30 * i + 90, _point0.X, _point0.Y, GAP_DIAL_QUARTER / 2);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X - 1, _point1.Y - 1, 0, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X - 1, _point1.Y, 0, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X, _point1.Y - 1, 0, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, _point1.X, _point1.Y, 0, LENGTH_DIAL);
                    
                    }
                    else
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, screenCenterX - 1, screenCenterY - 1, screenCenterX - MARGIN_DIAL_EDGE - LENGTH_DIAL, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, screenCenterX - 1, screenCenterY, screenCenterX - MARGIN_DIAL_EDGE - LENGTH_DIAL, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, screenCenterX, screenCenterY - 1, screenCenterX - MARGIN_DIAL_EDGE - LENGTH_DIAL, LENGTH_DIAL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 30 * i, screenCenterX, screenCenterY, screenCenterX - MARGIN_DIAL_EDGE - LENGTH_DIAL, LENGTH_DIAL);
                    }

                    _azmdrawing.DrawCircle(_display, colorForeground, 2, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER, RADIUS_CIRCLE_CENTER);


/*
                    _point0 = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point0.X - 1, _point0.Y - 1, 0, LENGTH_HOUR_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point0.X - 1, _point0.Y, 0, LENGTH_HOUR_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point0.X, _point0.Y - 1, 0, LENGTH_HOUR_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, _point0.X, _point0.Y, 0, LENGTH_HOUR_HAND);

                    _azmdrawing.DrawCircle(_display, colorForeground, 2, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER, RADIUS_CIRCLE_CENTER);

                    _point0 = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point0.X - 1, _point0.Y - 1, 0, LENGTH_MINUTE_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point0.X - 1, _point0.Y, 0, LENGTH_MINUTE_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point0.X, _point0.Y - 1, 0, LENGTH_MINUTE_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, _point0.X, _point0.Y, 0, LENGTH_MINUTE_HAND);
*/

                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, screenCenterX - 1, screenCenterY - 1, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_HOUR_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, screenCenterX - 1, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_HOUR_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, screenCenterX, screenCenterY - 1, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_HOUR_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeH, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_HOUR_HAND);
                    
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, screenCenterX - 1, screenCenterY - 1, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_MINUTE_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, screenCenterX - 1, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_MINUTE_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, screenCenterX, screenCenterY - 1, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_MINUTE_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeM, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_MINUTE_HAND);

                }

                if (showSecond == true)
                {

                    degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

/*
                    _point0 = _azmdrawing.FindPointDegreeDistance(degreeS, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, _point0.X - 1, _point0.Y - 1, 0, LENGTH_SECOND_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, _point0.X - 1, _point0.Y, 0, LENGTH_SECOND_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, _point0.X, _point0.Y - 1, 0, LENGTH_SECOND_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, _point0.X, _point0.Y, 0, LENGTH_SECOND_HAND);
*/

                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, screenCenterX - 1, screenCenterY - 1, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_SECOND_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, screenCenterX - 1, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_SECOND_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, screenCenterX, screenCenterY - 1, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_SECOND_HAND);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, screenCenterX, screenCenterY, RADIUS_CIRCLE_CENTER + MARGIN_CIRCLE_CENTER_HAND, LENGTH_SECOND_HAND);
               
                }

                if (showDate == true)
                {
                    _azmdrawing.DrawStringCentered(_display, colorForeground, fontsmall, screenCenterX + MARGIN_DAY_X, screenCenterY + MARGIN_DAY_Y, currentTime.Day.ToString("D2"));
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

                        showDate = false;
                        showSecond = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        showDate = false;
                        showSecond = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DATE:

                        showDate = true;
                        showSecond = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DATE:

                        showDate = true;
                        showSecond = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SECOND_BLACK:

                        showDate = false;
                        showSecond = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SECOND_WHITE:

                        showDate = false;
                        showSecond = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SECOND_BLACK_DATE:

                        showDate = true;
                        showSecond = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SECOND_WHITE_DATE:

                        showDate = true;
                        showSecond = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;


                }

            }

        }


    }

}

