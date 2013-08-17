using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Compass
{
    public class Compass
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontsmallNEWS = Resources.GetFont(Resources.FontResources.smallNEWS);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int degreeNorth = 0;
        static int degreeSouth = 0;
        static int degreeEast = 0;
        static int degreeWest = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_NORTHERN_POIHAND;

        static int handType = HAND_TYPE_POINT;
        static bool northernSouthern = NORTHERN_HEMISPHERE;
        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 25;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 4;
        const int LENGTH_MINUTE_HAND = 32;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 4;
        const int LENGTH_SECOND_HAND = 32;
        const int LENGTH_SECOND_HAND_TAIL = 10;
        const int THICKNESS_SECOND_HAND = 1;

        const int DATE_WIDTH = 19;
        const int DATE_HEIGHT = 13;
        const int DATE_MARGIN = 27;

        const int MARGIN_DIAL_RIM = 3;
        const int MARGIN_NUMBER_RIM = 3;
        const int MARGIN_RIM_EDGE = 3;
        const int MARGIN_NH_NUMBER = 9;
        const int MARGIN_SH_NUMBER = 20;
        const int THIKNESS_RIM = 12;

        const int MARGIN_SUNPOINTER_NEWS = 11;

        const int MAX_DISPLAY_MODE = 15;

        const int HAND_TYPE_SQUARE = 0;
        const int HAND_TYPE_POINT = 1;

        const bool NORTHERN_HEMISPHERE = true;
        const bool SOUTHERN_HEMISPHERE = false;


        const int DISPLAY_MODE_BLACK_NORTHERN_POIHAND = 0;
        const int DISPLAY_MODE_BLACK_NORTHERN_POIHAND_DATE = 1;
        const int DISPLAY_MODE_BLACK_NORTHERN_SQUHAND = 2;
        const int DISPLAY_MODE_BLACK_NORTHERN_SQUHAND_DATE = 3;
        const int DISPLAY_MODE_BLACK_SOUTHERN_POIHAND = 4;
        const int DISPLAY_MODE_BLACK_SOUTHERN_POIHAND_DATE = 5;
        const int DISPLAY_MODE_BLACK_SOUTHERN_SQUHAND = 6;
        const int DISPLAY_MODE_BLACK_SOUTHERN_SQUHAND_DATE = 7;
        const int DISPLAY_MODE_WHITE_NORTHERN_POIHAND = 8;
        const int DISPLAY_MODE_WHITE_NORTHERN_POIHAND_DATE = 9;
        const int DISPLAY_MODE_WHITE_NORTHERN_SQUHAND = 10;
        const int DISPLAY_MODE_WHITE_NORTHERN_SQUHAND_DATE = 11;
        const int DISPLAY_MODE_WHITE_SOUTHERN_POIHAND = 12;
        const int DISPLAY_MODE_WHITE_SOUTHERN_POIHAND_DATE = 13;
        const int DISPLAY_MODE_WHITE_SOUTHERN_SQUHAND = 14;
        const int DISPLAY_MODE_WHITE_SOUTHERN_SQUHAND_DATE = 15;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_NORTHERN_POIHAND;
            northernSouthern = NORTHERN_HEMISPHERE;
            handType = HAND_TYPE_POINT;
            showDate = false;
            colorForeground = Color.White;
            colorBackground = Color.Black;
            UpdateTime(null);

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

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE, screenCenterY - MARGIN_RIM_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_RIM_EDGE - THIKNESS_RIM, screenCenterY - MARGIN_RIM_EDGE - THIKNESS_RIM, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawDial(_display, colorForeground, colorBackground, 2, MARGIN_RIM_EDGE + THIKNESS_RIM + MARGIN_DIAL_RIM);
                _azmdrawing.DrawHourNumbers(_display, colorForeground, fontsmallNEWS, screenCenterX - (MARGIN_RIM_EDGE + THIKNESS_RIM + MARGIN_NUMBER_RIM + (fontsmallNEWS.Height / 2)), 0);

                switch (handType)
                {

                    case HAND_TYPE_POINT:

                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH - 180, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL, handType);

                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                        break;

                    case HAND_TYPE_SQUARE:

                        _point = _azmdrawing.FindPointDegreeDistance(degreeH + 180, screenCenterX, screenCenterY, LENGTH_HOUR_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, _point.X, _point.Y, 0, LENGTH_HOUR_HAND + LENGTH_HOUR_HAND_TAIL, handType);
                        _point = _azmdrawing.FindPointDegreeDistance(degreeM + 180, screenCenterX, screenCenterY, LENGTH_MINUTE_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, _point.X, _point.Y, 0, LENGTH_MINUTE_HAND + LENGTH_MINUTE_HAND_TAIL, handType);
                        _point = _azmdrawing.FindPointDegreeDistance(degreeS + 180, screenCenterX, screenCenterY, LENGTH_SECOND_HAND_TAIL);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND, degreeS, _point.X, _point.Y, 0, LENGTH_SECOND_HAND + LENGTH_SECOND_HAND_TAIL, handType);

                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                        break;

                }

                switch (northernSouthern)
                {

                    case NORTHERN_HEMISPHERE:

                        _display.DrawText("NH", fontsmallNEWS, colorForeground, screenCenterX - ((fontsmallNEWS.CharWidth('N') + fontsmallNEWS.CharWidth('H')) / 2), MARGIN_RIM_EDGE + THIKNESS_RIM + MARGIN_NUMBER_RIM + MARGIN_NH_NUMBER);

                        _azmdrawing.DrawAngledLine(_display, colorBackground, THIKNESS_RIM / 2, degreeH, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + THIKNESS_RIM) + 3, THIKNESS_RIM - 4, 1);

                        if (degreeH <= 180)
                        {
                            degreeSouth = degreeH / 2;
                        }
                        else
                        {
                            degreeSouth = degreeH + ((360 - degreeH) / 2);
                        }

                        if (degreeSouth < degreeH - MARGIN_SUNPOINTER_NEWS || degreeH + MARGIN_SUNPOINTER_NEWS < degreeSouth)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeSouth, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, degreeH, _point.X, _point.Y, "s");
                        }

                        degreeWest = (degreeSouth + 90) % 360;

                        if (degreeWest < degreeH - MARGIN_SUNPOINTER_NEWS || degreeH + MARGIN_SUNPOINTER_NEWS < degreeWest)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeWest, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, degreeH, _point.X, _point.Y, "w");
                        }

                        degreeNorth = (degreeSouth + 180) % 360;

                        if (degreeNorth < degreeH - MARGIN_SUNPOINTER_NEWS || degreeH + MARGIN_SUNPOINTER_NEWS < degreeNorth)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeNorth, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, degreeH, _point.X, _point.Y, "n");
                        }

                        degreeEast = (degreeSouth + 270) % 360;

                        if (degreeEast < degreeH - MARGIN_SUNPOINTER_NEWS || degreeH + MARGIN_SUNPOINTER_NEWS < degreeEast)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeEast, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, degreeH, _point.X, _point.Y, "e");
                        }
                        
                        break;

                    case SOUTHERN_HEMISPHERE:

                        _display.DrawText("SH", fontsmallNEWS, colorForeground, screenCenterX - ((fontsmallNEWS.CharWidth('S') + fontsmallNEWS.CharWidth('H')) / 2), screenHeight - (MARGIN_RIM_EDGE + THIKNESS_RIM + MARGIN_NUMBER_RIM + MARGIN_SH_NUMBER));

                        _azmdrawing.DrawAngledLine(_display, colorBackground, THIKNESS_RIM / 2, 0, screenCenterX, screenCenterY, screenCenterX - (MARGIN_RIM_EDGE + THIKNESS_RIM) + 3, THIKNESS_RIM - 4, 1);

                        if (degreeH <= 180)
                        {
                            degreeNorth = degreeH / 2;
                        }
                        else
                        {
                            degreeNorth = degreeH + ((360 - degreeH) / 2);
                        }

                        if (degreeNorth < 0 - MARGIN_SUNPOINTER_NEWS || 0 + MARGIN_SUNPOINTER_NEWS < degreeNorth)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeNorth, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, 0, _point.X, _point.Y, "n");
                        }

                        degreeWest = (degreeNorth + 90) % 360;

                        if (degreeWest < 0 - MARGIN_SUNPOINTER_NEWS || 0 + MARGIN_SUNPOINTER_NEWS < degreeWest)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeWest, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, 0, _point.X, _point.Y, "e");
                        }

                        degreeSouth = (degreeNorth + 180) % 360;

                        if (degreeSouth < 0 - MARGIN_SUNPOINTER_NEWS || 0 + MARGIN_SUNPOINTER_NEWS < degreeSouth)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeSouth, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, 0, _point.X, _point.Y, "s");
                        }

                        degreeEast = (degreeNorth + 270) % 360;

                        if (degreeEast < 0 - MARGIN_SUNPOINTER_NEWS || 0 + MARGIN_SUNPOINTER_NEWS < degreeEast)
                        {
                            _point = _azmdrawing.FindPointDegreeDistance(degreeEast, 64, 64, screenCenterX - (MARGIN_RIM_EDGE + (THIKNESS_RIM / 2)));
                            _azmdrawing.DrawStringAngled(_display, colorBackground, fontsmallNEWS, 0, _point.X, _point.Y, "w");
                        }
                        
                        break;

                }



                if (showDate == true)
                {

                    _display.DrawRectangle(colorForeground, 1, screenWidth - DATE_MARGIN - DATE_WIDTH, screenCenterY - (DATE_HEIGHT / 2), DATE_WIDTH, DATE_HEIGHT, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    _display.DrawText(currentTime.Day.ToString("D2"), fontsmallNEWS, colorForeground, screenWidth - DATE_MARGIN - DATE_WIDTH + 4, screenCenterY - (DATE_HEIGHT / 2));

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

                    case DISPLAY_MODE_BLACK_NORTHERN_POIHAND:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NORTHERN_POIHAND_DATE:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NORTHERN_SQUHAND:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_NORTHERN_SQUHAND_DATE:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SOUTHERN_POIHAND:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SOUTHERN_POIHAND_DATE:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SOUTHERN_SQUHAND:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SOUTHERN_SQUHAND_DATE:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NORTHERN_POIHAND:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NORTHERN_POIHAND_DATE:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NORTHERN_SQUHAND:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_NORTHERN_SQUHAND_DATE:

                        northernSouthern = NORTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SOUTHERN_POIHAND:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SOUTHERN_POIHAND_DATE:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SOUTHERN_SQUHAND:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SOUTHERN_SQUHAND_DATE:

                        northernSouthern = SOUTHERN_HEMISPHERE;
                        handType = HAND_TYPE_SQUARE;
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

