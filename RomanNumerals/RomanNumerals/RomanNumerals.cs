using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace RomanNumerals
{
    public class RomanNumerals
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;


        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int oldM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int distanceOuterCircle = 0;
        static int distanceInnerCircle = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showSecond = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 40;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 5;

        const int LENGTH_MINUTE_HAND = 55;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 5;

        const int LENGTH_SECOND_HAND = 54;
        const int LENGTH_SECOND_HAND_TAIL = 9;
        const int THICKNESS_SECOND_HAND = 1;

        const int RADIUS_PIN = 1;

        const int MARGIN_OUTER_CIRCLE_EDGE = 3;
        const int MARGIN_INNER_CIRCLE_OUTER_CIRCLE = 25;
        const int THICKNESS_CIRCLE = 3;

        const int MARGIN_NUMERAL_LINE_NUMERAL_LINE = 8;
        const int THICKNESS_NUMERALS = 3;
        

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_SECOND = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_SECOND = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            distanceOuterCircle = screenCenterX - MARGIN_OUTER_CIRCLE_EDGE;
            distanceInnerCircle = screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + THICKNESS_CIRCLE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE);
                
            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            oldM = -1;

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

            //dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            //period = new TimeSpan(0, 0, 1, 0, 0);

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

                if (oldM != currentTime.Minute || showSecond == true)
                {

                    oldM = currentTime.Minute;

                    _display.Clear();

                    _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, distanceOuterCircle, distanceOuterCircle, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, distanceOuterCircle - THICKNESS_CIRCLE, distanceOuterCircle - THICKNESS_CIRCLE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, distanceInnerCircle, distanceInnerCircle, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, distanceInnerCircle - THICKNESS_CIRCLE, distanceInnerCircle - THICKNESS_CIRCLE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                    _point = _azmdrawing.FindPointDegreeDistance(30, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 2 - 5, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 2, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 2 + 5, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 2, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 3 - 10, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 3, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 3, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 3, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 3 + 10, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 3, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 4 - 9, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 4, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 4 + 5, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 4 - 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 4 + 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 5, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 5 - 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 5 + 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 6 - 9, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 6 - 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 6 + 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 6 + 5, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 6, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 7 - 15, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 7 - 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 7 + 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 7 - 1, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 7, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 7 + 8, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 7, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 8 - 15, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 8 - 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 8 + 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 8 - 2, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 8 + 6, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 8 + 15, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 8, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 9 - 10, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 9, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 9 - 2, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 9 + 15, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 9 + 10, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 9 - 15, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 10 - 6, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 10 + 14, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 10 + 6, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 10 - 14, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 11 - 10, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 11 + 14, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 11 + 2, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 11 - 14, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 11 + 7, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 11, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    _point = _azmdrawing.FindPointDegreeDistance(30 * 12 - 15, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 12 + 14, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 12 - 4, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 12 - 14, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 12 + 4, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 12, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);
                    _point = _azmdrawing.FindPointDegreeDistance(30 * 12 + 13, screenCenterX, screenCenterY, distanceInnerCircle - 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_NUMERALS, 30 * 12, _point.X, _point.Y, 0, distanceOuterCircle - distanceInnerCircle + 1);

                    degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                    degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                    if (showSecond == true)
                    {

                        degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                        _point = _azmdrawing.FindPointDegreeDistance((degreeS + 180) % 360, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point.X, _point.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL);

                    }

                    _point = _azmdrawing.FindPointDegreeDistance((degreeH + 180) % 360, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, 6);

                    _point = _azmdrawing.FindPointDegreeDistance((degreeM + 180) % 360, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, 6);

                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);

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

                SetDisplayMode(displayMode);

                oldM = -1;
                UpdateTime(null);

            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    showSecond = false;

                    break;

                case DISPLAY_MODE_BLACK_SECOND:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    showSecond = true;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    showSecond = false;

                    break;

                case DISPLAY_MODE_WHITE_SECOND:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    showSecond = true;

                    break;

            }

        }

    }

}
