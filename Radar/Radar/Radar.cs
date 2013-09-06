using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Radar
{
    public class Radar
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
        static AGENT.AZMutil.Point _pointHour;
        static AGENT.AZMutil.Point _pointMinute;

        static Random _random;
        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int circleRadius = 0;

        static int showHourCounter = 0;
        static int radiusHour = 0;

        static int showMinuteCounter = 0;
        static int radiusMinute = 0;


        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MAX_DISPLAY_MODE = 1;

        const int DISTANCE_HOUR = 25;
        const int MAX_RADIUS_HOUR = 5;
        const int INTERVAL_SHRINK_HOUR = 10;
        const int DISTANCE_MINUTE = 45;
        const int MAX_RADIUS_MINUTE = 5;
        const int INTERVAL_SHRINK_MINUTE = 10;


        const int MARGIN_CIRCLE_EDGE = 3;
        const int THIKNESS_RIM = 2;
        const int GAP_DIAL = 5;


        const int WIDTH_WAVE = 75;
        const int GAP_WAVE_THICKNESS = 3;
        const int GAP_START = 2;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            circleRadius = screenCenterX - MARGIN_CIRCLE_EDGE;

            showHourCounter = 0;
            radiusHour = 0;

            showMinuteCounter = 0;
            radiusMinute = 0;

            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode();

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            _random = new Random(currentTime.Millisecond);

            UpdateTime(null);

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

            if (showDigital == false)
            {

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, 0);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                DrawRadarDial();

                if (degreeH == (360 + degreeS - 6) % 360)
                {
                    radiusHour = MAX_RADIUS_HOUR;
                    showHourCounter = 0;
                    _pointHour = _azmdrawing.FindPointDegreeDistance(degreeH, screenCenterX, screenCenterY, DISTANCE_HOUR);
                }

                if (0 < radiusHour)
                {

                    if (currentTime.Second % 2 == 0)
                    {
                        _display.DrawEllipse(colorForeground, 1, _pointHour.X, _pointHour.Y, radiusHour, radiusHour, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    }
                    
                    ++showHourCounter;

                    if ((showHourCounter % INTERVAL_SHRINK_HOUR) == 0)
                    {
                        --radiusHour;
                    }

                }


                if (degreeM == (360 + degreeS - 6) % 360)
                {
                    radiusMinute = MAX_RADIUS_MINUTE;
                    showMinuteCounter = 0;
                    _pointMinute = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, DISTANCE_MINUTE);
                }

                if (0 < radiusMinute)
                {

                    if (currentTime.Second % 2 == 0)
                    {
                        _display.DrawEllipse(colorForeground, 1, _pointMinute.X, _pointMinute.Y, radiusMinute, radiusMinute, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    }
                    
                    ++showMinuteCounter;

                    if ((showMinuteCounter % INTERVAL_SHRINK_MINUTE) == 0)
                    {
                        --radiusMinute;
                    }

                }

                DrawRadarWave(degreeS);

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


        static void DrawRadarDial()
        {

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, circleRadius, circleRadius, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, circleRadius - THIKNESS_RIM, circleRadius - THIKNESS_RIM, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, DISTANCE_HOUR, DISTANCE_HOUR, colorForeground, 0, 0, colorForeground, 0, 0, 0);
            _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, DISTANCE_MINUTE, DISTANCE_MINUTE, colorForeground, 0, 0, colorForeground, 0, 0, 0);

            _display.DrawLine(colorForeground, 1, screenCenterX, MARGIN_CIRCLE_EDGE, screenCenterX, screenHeight - (MARGIN_CIRCLE_EDGE * 2) + 1);
            _display.DrawLine(colorForeground, 1, MARGIN_CIRCLE_EDGE, screenCenterY, screenWidth - (MARGIN_CIRCLE_EDGE * 2) + 1, screenCenterY);

            for (int i = 1; (GAP_DIAL * i) <= (screenCenterX - MARGIN_CIRCLE_EDGE); i++)
            {

                _display.DrawLine(colorForeground, 1, screenCenterX - 2, screenCenterY - (GAP_DIAL * i), screenCenterX + 2, screenCenterY - (GAP_DIAL * i));
                _display.DrawLine(colorForeground, 1, screenCenterX - 2, screenCenterY + (GAP_DIAL * i), screenCenterX + 2, screenCenterY + (GAP_DIAL * i));

                _display.DrawLine(colorForeground, 1, screenCenterX - (GAP_DIAL * i), screenCenterY - 2, screenCenterX - (GAP_DIAL * i), screenCenterY + 2);
                _display.DrawLine(colorForeground, 1, screenCenterX + (GAP_DIAL * i), screenCenterY - 2, screenCenterX + (GAP_DIAL * i), screenCenterY + 2);

            }


        }

        static void DrawRadarWave(int degree)
        {

            int rnd = 0;
            int gap = GAP_START;

            _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degree, screenCenterX, screenCenterY, 0, circleRadius - THIKNESS_RIM + 1);

            for (int i = 1; i <= WIDTH_WAVE; i++)
            {

                --degree;
                
                if(i % GAP_WAVE_THICKNESS == 0)
                {
                    ++gap;
                }

                for (int j = 0; j <= circleRadius - THIKNESS_RIM + 1; j++)
                {

                    rnd = _random.Next(gap);
                                        
                    if (rnd == 0)
                    {
                        _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degree, screenCenterX, screenCenterY, j, 1);
                    }
                
                }
            
            
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

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    break;


            }

        }

    }

}

