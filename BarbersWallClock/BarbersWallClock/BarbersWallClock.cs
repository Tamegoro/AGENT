using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace BarbersWallClock
{
    public class BarbersWallClock
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font fontNinaBReverse = Resources.GetFont(Resources.FontResources.NinaBReverse);
        static Font fontsmallReverse = Resources.GetFont(Resources.FontResources.smallReverse);
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
        
        static int displayMode = DISPLAY_MODE_SQUFACE_SQUHAND_BLACK;
        static int faceType = FACE_TYPE_SQUARE;
        static int handType = HAND_TYPE_SQUARE;
        static int dateType = DATE_TYPE_HIDE;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 15;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_HOUR_NUMBER_SQU = 16;
        const int MARGIN_HOUR_NUMBER_ROU = 15;

        const int LENGTH_HOUR_HAND_SQU = 30;
        const int LENGTH_HOUR_HAND_TAIL_SQU = 13;
        const int THICKNESS_HOUR_HAND_SQU = 2;
        const int LENGTH_MINUTE_HAND_SQU = 40;
        const int LENGTH_MINUTE_HAND_TAIL_SQU = 13;
        const int THICKNESS_MINUTE_HAND_SQU = 2;
        const int LENGTH_SECOND_HAND_SQU = 40;
        const int LENGTH_SECOND_HAND_TAIL_SQU = 13;
        const int THICKNESS_SECOND_HAND_SQU = 1;

        const int LENGTH_HOUR_HAND_ROU = 30;
        const int LENGTH_HOUR_HAND_TAIL_ROU = 13;
        const int THICKNESS_HOUR_HAND_ROU = 2;
        const int LENGTH_MINUTE_HAND_ROU = 40;
        const int LENGTH_MINUTE_HAND_TAIL_ROU = 13;
        const int THICKNESS_MINUTE_HAND_ROU = 2;
        const int LENGTH_SECOND_HAND_ROU = 40;
        const int LENGTH_SECOND_HAND_TAIL_ROU = 13;
        const int THICKNESS_SECOND_HAND_ROU = 1;

        const int DATE_WIDTH = 19;
        const int DATE_HEIGHT = 13;
        const int DATE_MARGIN_SQU = 24;
        const int DATE_MARGIN_ROU = 24;

        const int FACE_TYPE_SQUARE = 1;
        const int FACE_TYPE_ROUND = 4;
        const int HAND_TYPE_SQUARE = 0;
        const int HAND_TYPE_POINT = 1;
        const int DATE_TYPE_HIDE = 0;
        const int DATE_TYPE_SQU = 1;
        const int DATE_TYPE_ROU = 2;


        const int DISPLAY_MODE_SQUFACE_SQUHAND_BLACK = 0;
        const int DISPLAY_MODE_SQUFACE_SQUHAND_WHITE = 1;
        const int DISPLAY_MODE_SQUFACE_SQUHAND_DATE_BLACK = 2;
        const int DISPLAY_MODE_SQUFACE_SQUHAND_DATE_WHITE = 3;
        const int DISPLAY_MODE_SQUFACE_POIHAND_BLACK = 4;
        const int DISPLAY_MODE_SQUFACE_POIHAND_WHITE = 5;
        const int DISPLAY_MODE_SQUFACE_POIHAND_DATE_BLACK = 6;
        const int DISPLAY_MODE_SQUFACE_POIHAND_DATE_WHITE = 7;
        const int DISPLAY_MODE_ROUFACE_SQUHAND_BLACK = 8;
        const int DISPLAY_MODE_ROUFACE_SQUHAND_WHITE = 9;
        const int DISPLAY_MODE_ROUFACE_SQUHAND_DATE_BLACK = 10;
        const int DISPLAY_MODE_ROUFACE_SQUHAND_DATE_WHITE = 11;
        const int DISPLAY_MODE_ROUFACE_POIHAND_BLACK = 12;
        const int DISPLAY_MODE_ROUFACE_POIHAND_WHITE = 13;
        const int DISPLAY_MODE_ROUFACE_POIHAND_DATE_BLACK = 14;
        const int DISPLAY_MODE_ROUFACE_POIHAND_DATE_WHITE = 15;



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_SQUFACE_SQUHAND_BLACK;
            faceType = FACE_TYPE_SQUARE;
            handType = HAND_TYPE_SQUARE;
            dateType = DATE_TYPE_HIDE;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

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

                degreeH = 360 - _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = 360 - _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = 360 - _azmdrawing.SecondToAngle(currentTime.Second);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                switch (faceType)
                {                    

                    case FACE_TYPE_SQUARE:

                        _azmdrawing.DrawDial(_display,colorForeground,colorBackground, 1, 0);

                        _point.X = (screenWidth / 2) + (screenWidth / 4);
                        _point.Y = MARGIN_HOUR_NUMBER_SQU;
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X - (fontNinaBReverse.CharWidth('1') / 4), _point.Y, "11");

                        _point.X = screenWidth - MARGIN_HOUR_NUMBER_SQU;
                        _point.Y = (screenHeight / 2) - (screenHeight / 4);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X - (fontNinaBReverse.CharWidth('1') / 2), _point.Y, "01");

                        _point.X = screenWidth - MARGIN_HOUR_NUMBER_SQU;
                        _point.Y = (screenHeight / 2);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X + 1, _point.Y, "9");

                        _point.X = screenWidth - MARGIN_HOUR_NUMBER_SQU;
                        _point.Y = (screenHeight / 2) + (screenHeight / 4);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X + 1, _point.Y, "8");

                        _point.X = (screenWidth / 2) + (screenWidth / 4);
                        _point.Y = screenHeight - MARGIN_HOUR_NUMBER_SQU;
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X, _point.Y, "7");

                        _point.X = (screenWidth / 2);
                        _point.Y = screenHeight - MARGIN_HOUR_NUMBER_SQU;
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X, _point.Y + 1, "6");

                        _point.X = (screenWidth / 2) - (screenWidth / 4);
                        _point.Y = screenHeight - MARGIN_HOUR_NUMBER_SQU;
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X, _point.Y, "5");

                        _point.X = MARGIN_HOUR_NUMBER_SQU;
                        _point.Y = (screenHeight / 2) + (screenHeight / 4);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X , _point.Y, "4");

                        _point.X = MARGIN_HOUR_NUMBER_SQU;
                        _point.Y = (screenHeight / 2);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X - 1, _point.Y, "3");

                        _point.X = MARGIN_HOUR_NUMBER_SQU;
                        _point.Y = (screenHeight / 2) - (screenHeight / 4);
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X - 1, _point.Y, "2");

                        _point.X = (screenWidth / 2) - (screenWidth / 4);
                        _point.Y = MARGIN_HOUR_NUMBER_SQU;
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X, _point.Y - 1, "1");

                        _point.X = (screenWidth / 2);
                        _point.Y = MARGIN_HOUR_NUMBER_SQU;
                        _azmdrawing.DrawStringCentered(_display, colorForeground, fontNinaBReverse, _point.X, _point.Y - 1, "21");

                        break;
                
                    case FACE_TYPE_ROUND:

                        _azmdrawing.DrawDial(_display,colorForeground,colorBackground, 3, 3);

                        for (int i = 1; i <= 9; i++)
                        {

                            _point = _azmdrawing.FindPointDegreeDistance(30 * (12 - i), screenCenterX, screenCenterY, screenCenterX - MARGIN_HOUR_NUMBER_ROU);
                            _azmdrawing.DrawStringAngled(_display, colorForeground, fontNinaBReverse, 30 * (12 - i), _point.X, _point.Y, i.ToString());

                        }

                        for (int i = 10; i <= 12; i++)
                        {

                            _point = _azmdrawing.FindPointDegreeDistance(30 * (12 - i), screenCenterX, screenCenterY, screenCenterX - MARGIN_HOUR_NUMBER_ROU);
                            _azmdrawing.DrawStringAngled(_display, colorForeground, fontNinaBReverse, 30 * (12 - i), _point.X, _point.Y, (i % 10).ToString() + "1");

                        }

                        break;
               
                }

                switch (handType)
                {                    
                    case HAND_TYPE_SQUARE:

                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND_SQU, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_SQU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND_SQU, degreeH - 180, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL_SQU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND_SQU, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_SQU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND_SQU, degreeM - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL_SQU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND_SQU, degreeS, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_SQU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND_SQU, degreeS - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL_SQU, handType);
                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                        break;

                    case HAND_TYPE_POINT:

                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND_ROU, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_ROU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND_ROU, degreeH - 180, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL_ROU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND_ROU, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_ROU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND_ROU, degreeM - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL_ROU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND_ROU, degreeS, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_ROU, handType);
                        _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_SECOND_HAND_ROU, degreeS - 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL_ROU, handType);
                        _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                        break;

                }

                switch (dateType)
                {

                    case DATE_TYPE_SQU:

                        _display.DrawRectangle(colorForeground, 1, DATE_MARGIN_SQU, screenCenterY - (DATE_HEIGHT / 2), DATE_WIDTH, DATE_HEIGHT, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                        _display.DrawText((currentTime.Day % 10).ToString("D1") + (currentTime.Day / 10).ToString("D1"), fontsmallReverse, colorForeground, DATE_MARGIN_SQU + 4, screenCenterY - (DATE_HEIGHT / 2));

                        break;                       

                    case DATE_TYPE_ROU:

                        _display.DrawRectangle(colorForeground, 1, DATE_MARGIN_ROU, screenCenterY - (DATE_HEIGHT / 2), DATE_WIDTH, DATE_HEIGHT, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                        _display.DrawText((currentTime.Day % 10).ToString("D1") + (currentTime.Day / 10).ToString("D1"), fontsmallReverse, colorForeground, DATE_MARGIN_ROU + 4, screenCenterY - (DATE_HEIGHT / 2));
    
                        break;
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

                    case DISPLAY_MODE_SQUFACE_SQUHAND_BLACK:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_SQUHAND_WHITE:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_SQUHAND_DATE_BLACK:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_SQU;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_SQUHAND_DATE_WHITE:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_SQU;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_POIHAND_BLACK:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_POIHAND_WHITE:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_POIHAND_DATE_BLACK:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_SQU;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUFACE_POIHAND_DATE_WHITE:

                        faceType = FACE_TYPE_SQUARE;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_SQU;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_SQUHAND_BLACK:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_SQUHAND_WHITE:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_SQUHAND_DATE_BLACK:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_ROU;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_SQUHAND_DATE_WHITE:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_SQUARE;
                        dateType = DATE_TYPE_ROU;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_POIHAND_BLACK:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_POIHAND_WHITE:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_HIDE;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_POIHAND_DATE_BLACK:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_ROU;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUFACE_POIHAND_DATE_WHITE:

                        faceType = FACE_TYPE_ROUND;
                        handType = HAND_TYPE_POINT;
                        dateType = DATE_TYPE_ROU;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
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
