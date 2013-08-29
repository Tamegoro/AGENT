using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace DontHurry
{
    public class DontHurry
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
        static Font fontNinaB = Resources.GetFont(Resources.FontResources.NinaB);
        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);

        static Color colorForeground;
        static Color colorBackground;
        static Color colorDisk;

        static Font fontDay;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeD = 0;

        static int numberDay = 0;
        static int degreeDay = 0;
        static String stringDay = "";

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_POIHAND_DISK;

        static int handType = HAND_TYPE_POINT;

        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 15;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_DAY_HAND = 28;
        const int THICKNESS_DAY_HAND = 4;

        const int THIKNESS_RIM = 5;
        const int MARGIN_EDGE_DISK = 3;
        const int MARGIN_CHAR_CHAR = 13;

        const int RADIUS_CIRCLE_DAY = 11;
        const int THICKNESS_CIRCLE_DAY = 3;

        const int HAND_TYPE_SQUARE = 0;
        const int HAND_TYPE_POINT = 1;

        const int DISPLAY_MODE_WHITE_POIHAND_DISK = 0;
        const int DISPLAY_MODE_WHITE_POIHAND_DISK_DATE = 1;
        const int DISPLAY_MODE_WHITE_SQUHAND_DISK = 2;
        const int DISPLAY_MODE_WHITE_SQUHAND_DISK_DATE = 3;
        const int DISPLAY_MODE_WHITE_POIHAND = 4;
        const int DISPLAY_MODE_WHITE_POIHAND_DATE = 5;
        const int DISPLAY_MODE_WHITE_SQUHAND = 6;
        const int DISPLAY_MODE_WHITE_SQUHAND_DATE = 7;
        const int DISPLAY_MODE_BLACK_POIHAND_DISK = 8;
        const int DISPLAY_MODE_BLACK_POIHAND_DISK_DATE = 9;
        const int DISPLAY_MODE_BLACK_SQUHAND_DISK = 10;
        const int DISPLAY_MODE_BLACK_SQUHAND_DISK_DATE = 11;
        const int DISPLAY_MODE_BLACK_POIHAND = 12;
        const int DISPLAY_MODE_BLACK_POIHAND_DATE = 13;
        const int DISPLAY_MODE_BLACK_SQUHAND = 14;
        const int DISPLAY_MODE_BLACK_SQUHAND_DATE = 15;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            fontDay = fontNinaB;

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_WHITE_POIHAND_DISK;
            handType = HAND_TYPE_POINT;
            showDate = false;
            colorForeground = Color.Black;
            colorBackground = Color.Black;
            colorDisk = Color.White;

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

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_BLACK_POIHAND_DISK || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_POIHAND_DISK || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK || displayMode == DISPLAY_MODE_BLACK_POIHAND_DISK_DATE || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK_DATE || displayMode == DISPLAY_MODE_WHITE_POIHAND_DISK_DATE || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK_DATE)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK, screenCenterY - MARGIN_EDGE_DISK, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }
                else if (displayMode == DISPLAY_MODE_BLACK_POIHAND || displayMode == DISPLAY_MODE_BLACK_SQUHAND || displayMode == DISPLAY_MODE_WHITE_POIHAND || displayMode == DISPLAY_MODE_WHITE_SQUHAND || displayMode == DISPLAY_MODE_BLACK_POIHAND_DATE || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DATE || displayMode == DISPLAY_MODE_WHITE_POIHAND_DATE || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DATE)
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK, screenCenterY - MARGIN_EDGE_DISK, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }

                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM, screenCenterY - MARGIN_EDGE_DISK - THIKNESS_RIM, colorForeground, 0, 0, colorForeground, 0, 0, 0);



                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 0, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM, THIKNESS_RIM + 1);

                for (int i = 0; i < 6; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 27 + (51 * i) + (51 / 2), screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM, THIKNESS_RIM + 1);
                }

                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 360 - 27, screenCenterX, screenCenterY, 0, screenCenterX - MARGIN_EDGE_DISK + 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 27, screenCenterX, screenCenterY, 0, screenCenterX - MARGIN_EDGE_DISK + 1);

                for (int i = 1; i < 6; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, 1, 27 + (51 * i), screenCenterX, screenCenterY, 0, screenCenterX - MARGIN_EDGE_DISK + 1);
                }

                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK + 1, screenCenterY - MARGIN_EDGE_DISK + 1, colorBackground, 0, 0, colorBackground, 0, 0, 0);

                degreeDay = 0;

                stringDay = "S";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "U";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "N";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                
                numberDay = 1;
                degreeDay = 27 + ((51 * (numberDay - 1)) + (51 / 2));

                stringDay = "M";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "O";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "N";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                
                numberDay = 2;
                degreeDay = 27 + ((51 * (numberDay - 1)) + (51 / 2));

                stringDay = "T";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "U";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "E";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                
                numberDay = 3;
                degreeDay = 27 + ((51 * (numberDay - 1)) + (51 / 2));

                stringDay = "W";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "E";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "D";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                
                numberDay = 4;
                degreeDay = 27 + ((51 * (numberDay - 1)) + (51 / 2));

                stringDay = "T";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "H";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "U";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                
                numberDay = 5;
                degreeDay = 27 + ((51 * (numberDay - 1)) + (51 / 2));

                stringDay = "F";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "R";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "I";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                
                numberDay = 6;
                degreeDay = 27 + ((51 * (numberDay - 1)) + (51 / 2));

                stringDay = "S";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay - MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay - MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                stringDay = "A";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay, _point.X, _point.Y, stringDay);

                stringDay = "T";
                _point = _azmdrawing.FindPointDegreeDistance(degreeDay + MARGIN_CHAR_CHAR, screenCenterX, screenCenterY, screenCenterX - MARGIN_EDGE_DISK - THIKNESS_RIM - (fontDay.Height / 2) - 2);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontDay, degreeDay + MARGIN_CHAR_CHAR, _point.X, _point.Y, stringDay);

                if ((int)currentTime.DayOfWeek == 0)
                {
                    degreeD = (-27 + (54 * ((currentTime.Hour * 60) + currentTime.Minute) / (60 * 24)) + 360) % 360;
                }
                else
                {
                    degreeD = (27 + (51 * ((int)currentTime.DayOfWeek - 1))) + 51 * ((currentTime.Hour * 60) + currentTime.Minute) / (60 * 24);
                }
                
                _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_DAY_HAND, degreeD, screenCenterX, screenCenterY, RADIUS_CIRCLE_DAY - 1, LENGTH_DAY_HAND, handType);

                if (displayMode == DISPLAY_MODE_BLACK_POIHAND_DISK || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK || displayMode == DISPLAY_MODE_WHITE_POIHAND_DISK || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK || displayMode == DISPLAY_MODE_BLACK_POIHAND_DISK_DATE || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DISK_DATE || displayMode == DISPLAY_MODE_WHITE_POIHAND_DISK_DATE || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DISK_DATE)
                {
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_CIRCLE_DAY, RADIUS_CIRCLE_DAY, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, RADIUS_CIRCLE_DAY - THICKNESS_CIRCLE_DAY, RADIUS_CIRCLE_DAY - THICKNESS_CIRCLE_DAY, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }
                else if (displayMode == DISPLAY_MODE_BLACK_POIHAND || displayMode == DISPLAY_MODE_BLACK_SQUHAND || displayMode == DISPLAY_MODE_WHITE_POIHAND || displayMode == DISPLAY_MODE_WHITE_SQUHAND || displayMode == DISPLAY_MODE_BLACK_POIHAND_DATE || displayMode == DISPLAY_MODE_BLACK_SQUHAND_DATE || displayMode == DISPLAY_MODE_WHITE_POIHAND_DATE || displayMode == DISPLAY_MODE_WHITE_SQUHAND_DATE)
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_CIRCLE_DAY, RADIUS_CIRCLE_DAY, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_CIRCLE_DAY - THICKNESS_CIRCLE_DAY, RADIUS_CIRCLE_DAY - THICKNESS_CIRCLE_DAY, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }

                if (showDate == true)
                {
                    _display.DrawText(currentTime.Day.ToString("D2"), fontsmall, colorForeground, screenCenterX - fontsmall.CharWidth('0') + 1, screenCenterY - (fontsmall.Height / 2));
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


                    case DISPLAY_MODE_WHITE_POIHAND_DISK:

                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.Black;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_DISK_DATE:

                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.Black;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_DISK:

                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.Black;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_DISK_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.Black;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND:

                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_POIHAND_DATE:

                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND:

                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_SQUHAND_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        colorDisk = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_DISK:

                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_DISK_DATE:

                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_DISK:

                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_DISK_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.White;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND:

                        handType = HAND_TYPE_POINT;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_POIHAND_DATE:

                        handType = HAND_TYPE_POINT;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND:

                        handType = HAND_TYPE_SQUARE;
                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_SQUHAND_DATE:

                        handType = HAND_TYPE_SQUARE;
                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        colorDisk = Color.Black;
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

