using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;

namespace CuckooClock
{
    public class CuckooClock
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerCuckoo;
        static TimeSpan dueTimeCuckoo;
        static TimeSpan periodCuckoo;

        static DateTime currentTime;

        static Bitmap _cuckooFrames;

        static Font fontRomenNumerals06 = Resources.GetFont(Resources.FontResources.RomenNumerals06);

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static Color colorForeground;
        static Color colorBackground;

        static int oldH = 0;
        static int newH = 0;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int diskCenterX = 0;
        static int diskCenterY = 0;
        static int diskRadius = 0;

        static int displayMode = DISPLAY_MODE_WHITE_RECT;

        static int handType = HAND_TYPE_RECT;

        static bool showCuckoo = false;
        static int cuckooTimesMax = 0;
        static int cuckooTimes = 0;
        static int cuckooCounter = 0;
        static int cuckooFrame = 0;
        static int cuckooX = 0;
        static int cuckooY = 0;

        const int MAX_DISPLAY_MODE = 3;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 22;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 2;
        const int LENGTH_MINUTE_HAND = 27;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 2;

        const int DISK_RADIUS = 63;

        const int CUCKOO_WIDTH = 41;
        const int CUCKOO_HEIGHT = 18;
        const int ROOF_EDGE_WIDTH = 10;
        const int BASE_HEIGHT = 15;

        const int MARGIN_CUCKOO_TOP = 9;
        const int MARGIN_CUCKOO_BOTTOM = 2;
        const int MARGIN_DISK_RIM = 3;
        const int MARGIN_DISK_NUMBER = 12;
        const int MARGIN_DISK_BOTTOM = 2;

        const int DISPLAY_MODE_WHITE_POINT = 0;
        const int DISPLAY_MODE_WHITE_RECT = 1;
        const int DISPLAY_MODE_BLACK_POINT = 2;
        const int DISPLAY_MODE_BLACK_RECT = 3;

        const int HAND_TYPE_RECT = 0;
        const int HAND_TYPE_POINT = 1;



        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _cuckooFrames = new Bitmap(Resources.GetBytes(Resources.BinaryResources.CuckooFrame), Bitmap.BitmapImageType.Gif);

            currentTime = new DateTime();

            colorForeground = new Color();
            colorBackground = new Color();

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            diskCenterX = screenCenterX;
            diskRadius = (screenHeight - (MARGIN_CUCKOO_TOP + CUCKOO_HEIGHT + MARGIN_CUCKOO_BOTTOM + MARGIN_DISK_BOTTOM)) / 2;
            diskCenterY = screenHeight - MARGIN_DISK_BOTTOM - diskRadius - 1;

            displayMode = DISPLAY_MODE_WHITE_POINT;
            handType = HAND_TYPE_POINT;
            colorForeground = Color.Black;
            colorBackground = Color.White;

            cuckooFrame = 0;
            DrawWatchfaceBase();

            currentTime = DateTime.Now;

            if (currentTime.Hour % 12 != 0)
            {
                newH = currentTime.Hour % 12;
            }
            else
            {
                newH = 12;
            }

            oldH = newH;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeCuckoo = new TimeSpan(0, 0, 0, 0, 0);
            periodCuckoo = new TimeSpan(0, 0, 0, 0, 150);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.ButtonSetup = new Buttons[]
            {
                Buttons.BottomRight, Buttons.MiddleRight, Buttons.TopRight
            };

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        private static void DrawWatchfaceBase()
        {

            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _display.DrawLine(colorForeground, 1, screenCenterX, 0, 0, screenHeight / 5);
            _display.DrawLine(colorForeground, 1, 0, screenHeight / 5, ROOF_EDGE_WIDTH, screenHeight / 5);
            _display.DrawLine(colorForeground, 1, ROOF_EDGE_WIDTH, screenHeight / 5, ROOF_EDGE_WIDTH, screenHeight - BASE_HEIGHT);
            _display.DrawLine(colorForeground, 1, ROOF_EDGE_WIDTH, screenHeight - BASE_HEIGHT, 0, screenHeight - BASE_HEIGHT);

            _display.DrawLine(colorForeground, 1, screenCenterX, 0, screenWidth, screenHeight / 5);
            _display.DrawLine(colorForeground, 1, screenWidth, screenHeight / 5, screenWidth - ROOF_EDGE_WIDTH, screenHeight / 5);
            _display.DrawLine(colorForeground, 1, screenWidth - ROOF_EDGE_WIDTH, screenHeight / 5, screenWidth - ROOF_EDGE_WIDTH, screenHeight - BASE_HEIGHT);
            _display.DrawLine(colorForeground, 1, screenWidth - ROOF_EDGE_WIDTH, screenHeight - BASE_HEIGHT, screenWidth, screenHeight - BASE_HEIGHT);

            _azmdrawing.FillArea(_display, colorForeground, 0, 0);
            _azmdrawing.FillArea(_display, colorForeground, screenWidth - 1, 0);
            _azmdrawing.FillArea(_display, colorForeground, 0, screenHeight - BASE_HEIGHT - 1);
            _azmdrawing.FillArea(_display, colorForeground, screenWidth - 1, screenHeight - BASE_HEIGHT - 1);

            _display.SetPixel(screenCenterX, 0, colorBackground);

            for (int i = 1; i <= 3; i++)
            {
                _display.SetPixel(screenCenterX - i, 0, colorForeground);
                _display.SetPixel(screenCenterX + i, 0, colorForeground);
            }

            _display.DrawEllipse(colorForeground, 1, diskCenterX, diskCenterY, diskRadius, diskRadius, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            _display.DrawEllipse(colorBackground, 1, diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM, diskRadius - MARGIN_DISK_RIM, colorBackground, 0, 0, colorBackground, 0, 0, 255);
            _display.DrawEllipse(colorForeground, 1, diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER, colorForeground, 0, 0, colorForeground, 0, 0, 255);

            for (int i = 1; i <= 12; i++)
            {
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, (30 / 2) + (30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER, MARGIN_DISK_NUMBER + 2);
            }

            for (int i = 1; i < 3; i++)
            {
                _point = _azmdrawing.FindPointDegreeDistance((30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2) + 1);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * i), _point.X, _point.Y, i.ToString());
            }

            for (int i = 3; i < 4; i++)
            {
                _point = _azmdrawing.FindPointDegreeDistance((30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2));
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * i), _point.X, _point.Y, i.ToString());
            }

            for (int i = 4; i < 6; i++)
            {
                _point = _azmdrawing.FindPointDegreeDistance((30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2) + 1);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * i), _point.X, _point.Y, i.ToString());
            }

            for (int i = 6; i < 7; i++)
            {
                _point = _azmdrawing.FindPointDegreeDistance((30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2) );
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * i), _point.X, _point.Y, i.ToString());
            }

            for (int i = 7; i < 9; i++)
            {
                _point = _azmdrawing.FindPointDegreeDistance((30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2) + 1);
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * i), _point.X, _point.Y, i.ToString());
            }

            for (int i = 9; i < 10; i++)
            {
                _point = _azmdrawing.FindPointDegreeDistance((30 * i), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2));
                _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * i), _point.X, _point.Y, i.ToString());
            }

            _point = _azmdrawing.FindPointDegreeDistance((30 * 10), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2) + 1);
            _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * 10), _point.X, _point.Y, "A");

            _point = _azmdrawing.FindPointDegreeDistance((30 * 11), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2) + 1);
            _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * 11), _point.X, _point.Y, "B");

            _point = _azmdrawing.FindPointDegreeDistance((30 * 12), diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - (MARGIN_DISK_NUMBER / 2));
            _azmdrawing.DrawStringAngled(_display, colorForeground, fontRomenNumerals06, (30 * 12), _point.X, _point.Y, "C");


            _display.DrawEllipse(colorBackground, 1, diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER - MARGIN_DISK_RIM, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER - MARGIN_DISK_RIM, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            DrawInitialCuckoo();

        }

        private static void DrawInitialCuckoo()
        {

            if (displayMode == DISPLAY_MODE_WHITE_RECT || displayMode == DISPLAY_MODE_WHITE_POINT)
            {
                cuckooX = 1;
            }
            else if (displayMode == DISPLAY_MODE_BLACK_RECT || displayMode == DISPLAY_MODE_BLACK_POINT)
            {
                cuckooX = 1 + ((CUCKOO_WIDTH + 1) * 9) + 1;
            }
            cuckooY = 1;
            _display.DrawImage(screenCenterX - (CUCKOO_WIDTH / 2), MARGIN_CUCKOO_TOP, _cuckooFrames, cuckooX, cuckooY, CUCKOO_WIDTH, CUCKOO_HEIGHT);

        }

        private static void UpdateTime(object state)
        {

            currentTime = DateTime.Now;

            degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
            degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

            _display.DrawEllipse(colorForeground, 1, diskCenterX, diskCenterY, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER - MARGIN_DISK_RIM - 1, diskRadius - MARGIN_DISK_RIM - MARGIN_DISK_NUMBER - MARGIN_DISK_RIM - 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

            _azmdrawing.DrawAngledLine(_display, colorBackground, THICKNESS_HOUR_HAND, degreeH, diskCenterX, diskCenterY, 0, LENGTH_HOUR_HAND, handType);
            _azmdrawing.DrawAngledLine(_display, colorBackground, THICKNESS_HOUR_HAND, (degreeH + 180) % 360, diskCenterX, diskCenterY, 0, LENGTH_HOUR_HAND_TAIL, handType);

            _azmdrawing.DrawAngledLine(_display, colorBackground, THICKNESS_MINUTE_HAND, degreeM, diskCenterX, diskCenterY, 0, LENGTH_MINUTE_HAND, handType);
            _azmdrawing.DrawAngledLine(_display, colorBackground, THICKNESS_MINUTE_HAND, (degreeM + 180) % 360, diskCenterX, diskCenterY, 0, LENGTH_MINUTE_HAND_TAIL, handType);

            _display.DrawEllipse(colorForeground, 1, diskCenterX, diskCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            //_display.DrawEllipse(colorForeground, 1, diskCenterX, diskCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);

/*
            cuckooFrame = 0;
            cuckooX = 1 + (CUCKOO_WIDTH * cuckooFrame) + (1 * cuckooFrame);
            cuckooY = 1;
            _display.DrawImage(screenCenterX - (CUCKOO_WIDTH / 2), MARGIN_CUCKOO_TOP, _cuckooFrames, cuckooX, cuckooY, CUCKOO_WIDTH, CUCKOO_HEIGHT);
*/

            _display.Flush();

            if (currentTime.Hour % 12 != 0)
            {
                newH = currentTime.Hour % 12;
            }
            else
            {
                newH = 12;
            }

            if (oldH != newH)
            {
                oldH = newH;
                cuckooTimesMax = oldH;
                showCuckoo = true;
                cuckooTimes = 0;
                cuckooCounter = 0;
                cuckooFrame = 0;
                _updateClockTimerCuckoo = new Timer(UpdateTimeCuckoo, null, dueTimeCuckoo, periodCuckoo);
            }

            if (currentTime.Minute == 30)
            {
                cuckooTimesMax = 1;
                showCuckoo = true;
                cuckooTimes = 0;
                cuckooCounter = 0;
                cuckooFrame = 0;
                _updateClockTimerCuckoo = new Timer(UpdateTimeCuckoo, null, dueTimeCuckoo, periodCuckoo);
            }


        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.TopRight)
                {
                    if (showCuckoo == false)
                    {
                        --displayMode;
                        if (displayMode < 0)
                        {
                            displayMode = MAX_DISPLAY_MODE;
                        }
                    }
                    else
                    {
                        showCuckoo = false;
                    }
                }
                else if (button == Buttons.MiddleRight)
                {
                    if (showCuckoo != true)
                    {
                        cuckooTimesMax = oldH;
                        showCuckoo = true;
                        cuckooTimes = 0; 
                        cuckooCounter = 0;
                        cuckooFrame = 0;
                        _updateClockTimerCuckoo = new Timer(UpdateTimeCuckoo, null, dueTimeCuckoo, periodCuckoo);
                    }
                    else
                    {
                        showCuckoo = false;
                    }
                }
                else if (button == Buttons.BottomRight)
                {
                    if (showCuckoo == false)
                    {
                        ++displayMode;
                        if (displayMode > MAX_DISPLAY_MODE)
                        {
                            displayMode = 0;
                        }
                    }
                    else
                    {
                        showCuckoo = false;
                    }
                }

                switch (displayMode)
                {


                    case DISPLAY_MODE_WHITE_POINT:

                        handType = HAND_TYPE_POINT;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        DrawWatchfaceBase();
                        UpdateTime(null);

                        break;


                    case DISPLAY_MODE_WHITE_RECT:

                        handType = HAND_TYPE_RECT;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        DrawWatchfaceBase();
                        UpdateTime(null);

                        break;


                    case DISPLAY_MODE_BLACK_POINT:

                        handType = HAND_TYPE_POINT;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        DrawWatchfaceBase();
                        UpdateTime(null);

                        break;


                    case DISPLAY_MODE_BLACK_RECT:

                        handType = HAND_TYPE_RECT;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        DrawWatchfaceBase();
                        UpdateTime(null);

                        break;


                }

                DrawInitialCuckoo();

                _display.Flush();

            }

        }


        private static void UpdateTimeCuckoo(object state)
        {


            if (showCuckoo == false)
            {
                cuckooTimesMax = 0;
                cuckooCounter = 0;
                cuckooTimes = 0;
                cuckooFrame = 0;
                DrawInitialCuckoo();
                _display.Flush();
                _updateClockTimerCuckoo.Dispose();
            }
            else
            {

                if (cuckooCounter < 9)
                {

                    if (displayMode == DISPLAY_MODE_WHITE_RECT || displayMode == DISPLAY_MODE_WHITE_POINT)
                    {
                        cuckooX = 1 + (CUCKOO_WIDTH * cuckooFrame) + (1 * cuckooFrame);
                    }
                    else if (displayMode == DISPLAY_MODE_BLACK_RECT || displayMode == DISPLAY_MODE_BLACK_POINT)
                    {
                        cuckooX = 1 + ((CUCKOO_WIDTH + 1) * 9) + 1 + (CUCKOO_WIDTH * cuckooFrame) + (1 * cuckooFrame);
                    }
                    
                    cuckooY = 1;
                    _display.DrawImage(screenCenterX - (CUCKOO_WIDTH / 2), MARGIN_CUCKOO_TOP, _cuckooFrames, cuckooX, cuckooY, CUCKOO_WIDTH, CUCKOO_HEIGHT);

                    _display.Flush();
                    cuckooFrame++;
                }
                else if (cuckooCounter == 10)
                {
                    cuckooFrame--;
                }
                else if (12 < cuckooCounter && 0 <= cuckooFrame)
                {
                    if (displayMode == DISPLAY_MODE_WHITE_RECT || displayMode == DISPLAY_MODE_WHITE_POINT)
                    {
                        cuckooX = 1 + (CUCKOO_WIDTH * cuckooFrame) + (1 * cuckooFrame);
                    }
                    else if (displayMode == DISPLAY_MODE_BLACK_RECT || displayMode == DISPLAY_MODE_BLACK_POINT)
                    {
                        cuckooX = 1 + ((CUCKOO_WIDTH + 1) * 9) + 1 + (CUCKOO_WIDTH * cuckooFrame) + (1 * cuckooFrame);
                    }

                    cuckooY = 1;
                    _display.DrawImage(screenCenterX - (CUCKOO_WIDTH / 2), MARGIN_CUCKOO_TOP, _cuckooFrames, cuckooX, cuckooY, CUCKOO_WIDTH, CUCKOO_HEIGHT);

                    _display.Flush();
                    cuckooFrame--;
                }
                
                cuckooCounter++;

                if (25 <= cuckooCounter)
                {
                    cuckooFrame = 0;
                    cuckooCounter = 0;
                    ++cuckooTimes;
                }

                if (cuckooTimes >= cuckooTimesMax)
                {
                    showCuckoo = false;
/*
                    cuckooTimesMax = 0;
                    cuckooFrame = 0;
                    cuckooCounter = 0;
                    cuckooTimes = 0;
                    _updateClockTimerCuckoo.Dispose();
*/

                }

            }


        }

    }

}

