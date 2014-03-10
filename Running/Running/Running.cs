using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Running
{
    public class Running
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontRunning = Resources.GetFont(Resources.FontResources.Running);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int oldMinute = 0;

        static int frameRunning = 0;
        static int frameJump = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MILLISECOND_UPDATE = 250;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_CIRCLE_EDGE = 2;
        const int WIDTH_CIRCLE_EDGE = 2;

        const int MARGIN_INDEX_L_CIRCLE = 2;
        const int THIKNESS_INDEX_L = 3;
        const int LENGTH_INDEX_L = 5;

        const int MARGIN_INDEX_S_CIRCLE = 2;
        const int THIKNESS_INDEX_S = 2;
        const int LENGTH_INDEX_S = 1;


        const int THIKNESS_HOUR_HAND = 5;
        const int LENGTH_HOUR_HAND = 25;

        const int THIKNESS_MINUTE_HAND = 5;
        const int LENGTH_MINUTE_HAND = 35;

        const int THIKNESS_SECOND_HAND = 1;
        const int LENGTH_SECOND_HAND = 40;

        const int RADIUS_OUTER_PIN = 2;
        const int RADIUS_INNER_PIN = 1;

        const int MAX_DISPLAY_MODE = 1;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode();

            showDigital = false;

            //DrawWatchFace();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 0, MILLISECOND_UPDATE);

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

            if (oldMinute != currentTime.Minute)
            {
                frameJump = 0;
                oldMinute = currentTime.Minute;
            }

            if (showDigital == false)
            {

                DrawHands();
                FlushDisplay();                
            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > (SHOW_DIGITAL_SECOND * 1000) / MILLISECOND_UPDATE)
                {
                    showDigital = false;
                    showDigitalCounter = 0;
                    DrawWatchFace();
                }

                _display.Flush();

            }

        }

        private static void DrawWatchFace()
        {

            DrawWatchFaceBack();
            DrawHands();

            FlushDisplay();

        }

        private static void DrawWatchFaceBack()
        {

            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _display.DrawEllipse(colorForeground, 0, screenCenterX, screenCenterY, screenCenterX - MARGIN_CIRCLE_EDGE, screenCenterY - MARGIN_CIRCLE_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            _display.DrawEllipse(colorBackground, 0, screenCenterX, screenCenterY, screenCenterX - MARGIN_CIRCLE_EDGE - WIDTH_CIRCLE_EDGE, screenCenterY - MARGIN_CIRCLE_EDGE - WIDTH_CIRCLE_EDGE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            for (int i = 0; i < 60; i++)
            {

                if (i % 5 == 0)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_INDEX_L, i * 6, screenCenterX, screenCenterY, screenCenterX - MARGIN_CIRCLE_EDGE - WIDTH_CIRCLE_EDGE - MARGIN_INDEX_L_CIRCLE - LENGTH_INDEX_L, LENGTH_INDEX_L, 0);
                }
                else
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_INDEX_S, i * 6, screenCenterX, screenCenterY, screenCenterX - MARGIN_CIRCLE_EDGE - WIDTH_CIRCLE_EDGE - MARGIN_INDEX_S_CIRCLE - LENGTH_INDEX_S, LENGTH_INDEX_S, 0);
                }

            }


        }

        private static void DrawHands()
        {

            degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
            degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
            degreeS = _azmdrawing.SecondToAngle(currentTime.Second);
            
            _display.DrawEllipse(colorBackground, 0, screenCenterX, screenCenterY, screenCenterX - MARGIN_CIRCLE_EDGE - WIDTH_CIRCLE_EDGE - MARGIN_INDEX_L_CIRCLE - LENGTH_INDEX_L - 2, screenCenterY - MARGIN_CIRCLE_EDGE - WIDTH_CIRCLE_EDGE - MARGIN_INDEX_L_CIRCLE - LENGTH_INDEX_L - 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_HOUR_HAND, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND, 6);
            _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_MINUTE_HAND, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND, 6);
            _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_SECOND_HAND, degreeS, screenCenterX, screenCenterY, 0, LENGTH_SECOND_HAND, 0);


            if (degreeS == 0)
            {

                if (frameJump == 0)
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontRunning, "8", degreeS, LENGTH_SECOND_HAND);
                    ++frameJump;
                }
                else
                {
                    _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontRunning, "9", degreeS, LENGTH_SECOND_HAND);
                    ++frameJump;
                }

            }
            else if (90 < degreeS && degreeS <= 270)
            {
                _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontRunning, (frameRunning + 4).ToString(), degreeS, LENGTH_SECOND_HAND);
            }
            else
            {
                _azmdrawing.DrawStringByDegreeDistance(_display, colorForeground, fontRunning, frameRunning.ToString(), degreeS, LENGTH_SECOND_HAND);
            }

            _display.DrawEllipse(colorForeground, 0, screenCenterX, screenCenterY, RADIUS_OUTER_PIN, RADIUS_OUTER_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            _display.DrawEllipse(colorBackground, 0, screenCenterX, screenCenterY, RADIUS_INNER_PIN, RADIUS_INNER_PIN, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            if (frameRunning == 3)
            {
                frameRunning = 0;
            }
            else
            {
                ++frameRunning;
            }
        
        }

        private static void FlushDisplay()
        {

            if (showDigital != true)
            {
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

                SetDisplayMode();

                UpdateTime(null);

            }

        }

        private static void SetDisplayMode()
        {

            switch (displayMode)
            {


                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;

                    DrawWatchFace();

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;

                    DrawWatchFace();

                    break;


            }

        }

    }

}

