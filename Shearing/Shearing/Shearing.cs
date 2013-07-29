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

        static DateTime currentTime;

        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);
        static Font font7barPBd32 = Resources.GetFont(Resources.FontResources._7barPBd32);

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


        const int LENGTH_HOUR_HAND = 20;
        const int LENGTH_HOUR_HAND_TAIL = 10;
        const int THICKNESS_HOUR_HAND = 2;
        const int LENGTH_MINUTE_HAND = 30;
        const int LENGTH_MINUTE_HAND_TAIL = 10;
        const int THICKNESS_MINUTE_HAND = 2;

        const int DISPLAY_MODE_SQUARED_BLACK = 0;
        const int DISPLAY_MODE_SQUARED_WHITE = 1;
        const int DISPLAY_MODE_ROUNDED_BLACK = 2;
        const int DISPLAY_MODE_ROUNDED_WHITE = 3;
        const int DISPLAY_MODE_DIGITAL = 4;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_SQUARED_BLACK;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            TimeSpan period = new TimeSpan(0, 0, 1, 0, 0);
            //TimeSpan dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            //TimeSpan period = new TimeSpan(0, 0, 0, 1, 0);
            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
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

                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH, centerX, centerY, 0, LENGTH_HOUR_HAND, 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH - 180, centerX, centerY, 0, LENGTH_HOUR_HAND_TAIL, 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM, centerX, centerY, 0, LENGTH_MINUTE_HAND, 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM - 180, centerX, centerY, 0, LENGTH_MINUTE_HAND_TAIL, 1);

                _display.DrawEllipse(colorBackground, 1, centerX, centerY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, centerX, centerY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);
           
            }
            else if (displayMode == DISPLAY_MODE_ROUNDED_BLACK || displayMode == DISPLAY_MODE_ROUNDED_WHITE)
            {

                _azmdrawing.DrawWatchfaceBase(_display, colorForeground, colorBackground, fontsmall, 0);

                centerX = 78;
                centerY = 78;

                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH, centerX, centerY, 0, LENGTH_HOUR_HAND, 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH - 180, centerX, centerY, 0, LENGTH_HOUR_HAND_TAIL, 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM, centerX, centerY, 0, LENGTH_MINUTE_HAND, 1);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM - 180, centerX, centerY, 0, LENGTH_MINUTE_HAND_TAIL, 1);

                _display.DrawEllipse(colorBackground, 1, centerX, centerY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, centerX, centerY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);
            
            }
            else if (displayMode == DISPLAY_MODE_DIGITAL)
            {
                _azmdrawing.DrawStringAligned(_display, colorForeground, font7barPBd32, currentTime.Hour.ToString("D2") + ":" + currentTime.Minute.ToString("D2"), AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_MIDDLE, 0);
            }

            _display.Flush();

        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {

                if (button == Buttons.MiddleRight)
                {
                    switch (displayMode)
                    {

                        case DISPLAY_MODE_SQUARED_BLACK:

                            displayMode = DISPLAY_MODE_SQUARED_WHITE;
                            colorForeground = Color.Black;
                            colorBackground = Color.White;

                            break;

                        case DISPLAY_MODE_SQUARED_WHITE:

                            displayMode = DISPLAY_MODE_ROUNDED_BLACK;
                            colorForeground = Color.White;
                            colorBackground = Color.Black;

                            break;

                        case DISPLAY_MODE_ROUNDED_BLACK:

                            displayMode = DISPLAY_MODE_ROUNDED_WHITE;
                            colorForeground = Color.Black;
                            colorBackground = Color.White;

                            break;

                        case DISPLAY_MODE_ROUNDED_WHITE:

                            displayMode = DISPLAY_MODE_DIGITAL;
                            colorForeground = Color.White;
                            colorBackground = Color.Black;

                            break;

                        case DISPLAY_MODE_DIGITAL:

                            displayMode = DISPLAY_MODE_SQUARED_BLACK;
                            colorForeground = Color.White;
                            colorBackground = Color.Black;

                            break;
                    
                    }

                    UpdateTime(null);

                }

            }

        }        


    }
}
