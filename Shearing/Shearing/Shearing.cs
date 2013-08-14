using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Shearing
{
    public class Shearing
    {
        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int centerX = 0;
        static int centerY = 0;


        static int displayMode = DISPLAY_MODE_SQUARED_BLACK;
        static bool showDisk = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int MAX_DISPLAY_MODE = 5;

        const int SHOW_DIGITAL_SECOND = 10;


        const int LENGTH_HOUR_HAND = 20;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 4;
        const int LENGTH_MINUTE_HAND = 30;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 4;

        const int DISPLAY_MODE_SQUARED_BLACK = 0;
        const int DISPLAY_MODE_ROUNDED_BLACK = 1;
        const int DISPLAY_MODE_ROUNDED_BLACK_DISK = 2;
        const int DISPLAY_MODE_SQUARED_WHITE = 3;
        const int DISPLAY_MODE_ROUNDED_WHITE = 4;
        const int DISPLAY_MODE_ROUNDED_WHITE_DISK = 5;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_SQUARED_BLACK;
            showDisk = false;
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

                degreeH = _azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute);
                degreeM = _azmdrawing.MinuteToAngle(currentTime.Minute);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_SQUARED_BLACK || displayMode == DISPLAY_MODE_SQUARED_WHITE)
                {
                
                    _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, fontsmall, 1);

                    centerX = 78;
                    centerY = 78;

                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, centerX, centerY, 0, LENGTH_HOUR_HAND, 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH - 180, centerX, centerY, 0, LENGTH_HOUR_HAND_TAIL, 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, centerX, centerY, 0, LENGTH_MINUTE_HAND, 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM - 180, centerX, centerY, 0, LENGTH_MINUTE_HAND_TAIL, 1);

                    _display.DrawEllipse(colorBackground, 1, centerX, centerY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, centerX, centerY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);
           
                }
                else if (displayMode == DISPLAY_MODE_ROUNDED_BLACK || displayMode == DISPLAY_MODE_ROUNDED_WHITE || displayMode == DISPLAY_MODE_ROUNDED_BLACK_DISK || displayMode == DISPLAY_MODE_ROUNDED_WHITE_DISK)
                {

                    if (showDisk == true)
                    {
                        _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, fontsmall, 0);
                    }
                    else
                    {
                        _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, fontsmall, 8);
                    }
                    
                    
                    centerX = 78;
                    centerY = 78;

                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH, centerX, centerY, 0, LENGTH_HOUR_HAND, 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_HOUR_HAND, degreeH - 180, centerX, centerY, 0, LENGTH_HOUR_HAND_TAIL, 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM, centerX, centerY, 0, LENGTH_MINUTE_HAND, 1);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_MINUTE_HAND, degreeM - 180, centerX, centerY, 0, LENGTH_MINUTE_HAND_TAIL, 1);

                    _display.DrawEllipse(colorBackground, 1, centerX, centerY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, centerX, centerY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            
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

                    case DISPLAY_MODE_SQUARED_BLACK:

                        showDisk = false;
                        displayMode = DISPLAY_MODE_SQUARED_BLACK;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUNDED_BLACK:

                        showDisk = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUNDED_BLACK_DISK:

                        showDisk = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_SQUARED_WHITE:

                        showDisk = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUNDED_WHITE:

                        showDisk = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_ROUNDED_WHITE_DISK:

                        showDisk = true;
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
