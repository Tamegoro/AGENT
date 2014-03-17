using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace PendulumClock
{
    public class PendulumClock
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
        //static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int squareCornerX = 0;
        static int squareCornerY = 0;

        static int pendulumCounter = 0;
        static int degreePendulum = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool simpleMode = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MILLISECOND_UPDATE = 200;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_CIRCLE_EDGE = 2;
        const int RADIUS_CIRCLE = 40;

        const int MARGIN_SQUARE_EDGE = 3;
        const int WIDTH_SQUARE = 51;
        const int HEIGHT_SQUARE = 39;

        const int THIKNESS_PENDULUM = 3;
        const int LENGTH_PENDULUM = 69;
        const int RADIUS_PENDULUM = 8;
        const int DEGREE_SWING = 5;

        const int MARGIN_INDEX_CIRCLE = 1;
        const int THIKNESS_INDEX = 3;
        const int LENGTH_INDEX = 6;

        const int THIKNESS_HOUR_HAND = 4;
        const int LENGTH_HOUR_HAND = 20;

        const int THIKNESS_MINUTE_HAND = 3;
        const int LENGTH_MINUTE_HAND = 30;

        //const int THIKNESS_SECOND_HAND = 1;
        //const int LENGTH_SECOND_HAND = 30;

        const int RADIUS_OUTER_PIN = 2;
        const int RADIUS_INNER_PIN = 1;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_SIMPLE = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_SIMPLE = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            squareCornerX = screenCenterX - (WIDTH_SQUARE / 2);
            squareCornerY = screenHeight - (HEIGHT_SQUARE + MARGIN_SQUARE_EDGE);

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode();

            showDigital = false;

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

            if (showDigital == false)
            {
                DrawWatchFace();
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

            _display.Clear();

            _display.DrawRectangle(colorForeground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

            DrawPendulum();
            DrawClock();

            FlushDisplay();

        }


        private static void DrawPendulum()
        {

            _display.DrawRectangle(colorBackground, 0, squareCornerX, squareCornerY, WIDTH_SQUARE, HEIGHT_SQUARE, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            switch(pendulumCounter)
            {

                case 0:

                    degreePendulum = 180 - (DEGREE_SWING * 2);

                    break;

                case 1:

                    degreePendulum = 180 - (DEGREE_SWING * 1);

                    break;

                case 2:

                    degreePendulum = 180;

                    break;

                case 3:

                    degreePendulum = 180 + (DEGREE_SWING * 1);

                    break;

                case 4:

                    degreePendulum = 180 + (DEGREE_SWING * 2);

                    break;

                case 5:

                    degreePendulum = 180 + (DEGREE_SWING * 2);

                    break;

                case 6:

                    degreePendulum = 180 + (DEGREE_SWING * 2);

                    break;

                case 7:

                    degreePendulum = 180 + (DEGREE_SWING * 1);

                    break;

                case 8:

                    degreePendulum = 180;

                    break;

                case 9:

                    degreePendulum = 180 - (DEGREE_SWING * 1);

                    break;

                case 10:

                    degreePendulum = 180 - (DEGREE_SWING * 2);

                    break;

                case 11:

                    degreePendulum = 180 - (DEGREE_SWING * 2);

                    break;

            }

            _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_PENDULUM, degreePendulum, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, 0, LENGTH_PENDULUM, 0);

            _point = _azmdrawing.FindPointDegreeDistance(degreePendulum, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, LENGTH_PENDULUM);
            _display.DrawEllipse(colorForeground, 0, _point.X, _point.Y, RADIUS_PENDULUM, RADIUS_PENDULUM, colorForeground, 0, 0, colorForeground, 0, 0, 255);


            if (simpleMode == false)
            {
                _display.DrawEllipse(colorBackground, 1, _point.X, _point.Y, RADIUS_PENDULUM - 2, RADIUS_PENDULUM - 2, colorBackground, 0, 0, colorBackground, 0, 0, 0);
            }

            _display.DrawRectangle(colorBackground, 1, squareCornerX, squareCornerY, WIDTH_SQUARE, HEIGHT_SQUARE, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 0);


            if(11 <= pendulumCounter)
            {
                pendulumCounter = 0;
            }
            else
            {
                pendulumCounter++;
            }

        }

        private static void DrawClock()
        {

            degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
            degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
            //degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

            _display.DrawEllipse(colorBackground, 0, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, RADIUS_CIRCLE, RADIUS_CIRCLE, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            if (simpleMode == false)
            {
                for (int i = 0; i < 4; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_INDEX, i * 90, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, RADIUS_CIRCLE - MARGIN_INDEX_CIRCLE - LENGTH_INDEX + 1, LENGTH_INDEX, 0);
                }
            }

            _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_HOUR_HAND, degreeH, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, 0, LENGTH_HOUR_HAND, 0);
            _azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_MINUTE_HAND, degreeM, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, 0, LENGTH_MINUTE_HAND, 0);
            //_azmdrawing.DrawAngledLine(_display, colorForeground, THIKNESS_SECOND_HAND, degreeS, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, 0, LENGTH_SECOND_HAND, 0);

            if (simpleMode == false)
            {
                _display.DrawEllipse(colorForeground, 0, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, RADIUS_OUTER_PIN, RADIUS_OUTER_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                _display.DrawEllipse(colorBackground, 0, screenCenterX, MARGIN_CIRCLE_EDGE + RADIUS_CIRCLE, RADIUS_INNER_PIN, RADIUS_INNER_PIN, colorBackground, 0, 0, colorBackground, 0, 0, 255);
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

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    simpleMode = false;

                    break;

                case DISPLAY_MODE_BLACK_SIMPLE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    simpleMode = true;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    simpleMode = false;

                    break;

                case DISPLAY_MODE_WHITE_SIMPLE:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    simpleMode = true;

                    break;

            }

            DrawWatchFace();

        }

    }

}

