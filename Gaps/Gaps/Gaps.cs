using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Gaps
{
    public class Gaps
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
        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 40;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 4;
        const int LENGTH_MINUTE_HAND = 50;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 4;
        const int LENGTH_SECOND_HAND = 50;
        const int LENGTH_SECOND_HAND_TAIL = 10;
        const int THICKNESS_SECOND_HAND = 1;
        const int LENGTH_GAP = 10;


        const int MARGIN_CENTER_NUMBER = 23;
        const int MARGIN_DIAL_EDGE = 3;

        const int DATE_WIDTH = 19;
        const int DATE_HEIGHT = 13;
        const int DATE_MARGIN = 11;

        const int MAX_DISPLAY_MODE = 5;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DIAL = 1;
        const int DISPLAY_MODE_BLACK_DIAL_DATE = 2;
        const int DISPLAY_MODE_WHITE = 3;
        const int DISPLAY_MODE_WHITE_DIAL = 4;
        const int DISPLAY_MODE_WHITE_DIAL_DATE = 5;


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
            showDial = false;
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


                _point = _azmdrawing.FindPointDegreeDistance(degreeH + 180, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, MARGIN_CENTER_NUMBER + LENGTH_HOUR_HAND_TAIL - (LENGTH_GAP / 2));
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, MARGIN_CENTER_NUMBER + LENGTH_GAP, LENGTH_HOUR_HAND - (MARGIN_CENTER_NUMBER + LENGTH_GAP));
                _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, MARGIN_CENTER_NUMBER + LENGTH_MINUTE_HAND_TAIL - (LENGTH_GAP / 2));
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, MARGIN_CENTER_NUMBER + LENGTH_GAP, LENGTH_MINUTE_HAND - (MARGIN_CENTER_NUMBER + LENGTH_GAP));
                _point = _azmdrawing.FindPointDegreeDistance(degreeS + 180, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point.X, _point.Y, 0, MARGIN_CENTER_NUMBER + LENGTH_SECOND_HAND_TAIL - (LENGTH_GAP / 2));
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, screenCenterX, screenCenterY, MARGIN_CENTER_NUMBER + LENGTH_GAP, LENGTH_SECOND_HAND - (MARGIN_CENTER_NUMBER + LENGTH_GAP));

                _azmdrawing.DrawHourNumbers(_display, colorForeground, fontsmall, MARGIN_CENTER_NUMBER, 5);

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);


                if (showDial == true)
                {
                    _azmdrawing.DrawDial(_display, colorForeground, 0, MARGIN_DIAL_EDGE);
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

                        showDial = false;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DIAL:

                        showDial = true;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DIAL_DATE:

                        showDial = true;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        showDial = false;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DIAL:

                        showDial = true;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DIAL_DATE:

                        showDial = true;
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

