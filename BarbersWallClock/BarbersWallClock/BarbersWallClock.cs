using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using Agent.Contrib.Hardware;
using AGENT.AZMutil.AZMDrawing;


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

        static Bitmap _background;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontIPAexGothicReverseNumber06 = Resources.GetFont(Resources.FontResources.IPAexGothicReverseNumber06);

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static Color colorForeground;
        static Color colorBackground;

        static int degreeH = 0;
        static int degreeM = 0;
        static int degreeS = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int dateRectY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;
        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static int backgroundBitmapColor = BACKGROUND_BITMAP_COLOR_BLACK;

        const int BACKGROUND_BITMAP_COLOR_BLACK = 0;
        const int BACKGROUND_BITMAP_COLOR_WHITE = 1;

        const int LENGTH_HOUR_HAND = 31;
        const int LENGTH_HOUR_HAND_TAIL = 13;
        const int LENGTH_MINUTE_HAND = 41;
        const int LENGTH_MINUTE_HAND_TAIL = 13;
        const int LENGTH_SECOND_HAND = 41;
        const int LENGTH_SECOND_HAND_TAIL = 13;

        const int DATE_RECT_LEFT_MARGIN = 21;
        const int DATE_RECT_WIDTH = 17;
        const int DATE_RECT_HEIGHT = 14;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;

        const int MAX_DISPLAY_MODE = 1;

        const int SHOW_DIGITAL_SECOND = 10;

        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            currentTime = new DateTime();

            colorForeground = new Color();
            colorBackground = new Color();

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            dateRectY = screenCenterY - (DATE_RECT_HEIGHT / 2) - 1;            


            displayMode = DISPLAY_MODE_BLACK;
            _background = new Bitmap(Resources.GetBytes(Resources.BinaryResources.BarbersWallClockBlack), Bitmap.BitmapImageType.Gif);
            backgroundBitmapColor = BACKGROUND_BITMAP_COLOR_BLACK;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            showDigital = false;

            UpdateTime(null);

            currentTime = DateTime.Now;

            //dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            //period = new TimeSpan(0, 0, 1, 0, 0);
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

                _display.Clear();

                degreeH = 360 - (_azmdrawing.HourToAngle(currentTime.Hour, currentTime.Minute));
                degreeM = 360 - (_azmdrawing.MinuteToAngle(currentTime.Minute));
                degreeS = 360 - (_azmdrawing.SecondToAngle(currentTime.Second));

                _display.DrawImage(0, 0, _background, 0, 0, _background.Width, _background.Height);

                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeH + 180, screenCenterX, screenCenterY, 0, LENGTH_HOUR_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 2, degreeM + 180, screenCenterX, screenCenterY, 0, LENGTH_MINUTE_HAND_TAIL);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS, screenCenterX, screenCenterY, 0, LENGTH_SECOND_HAND);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, degreeS + 180, screenCenterX, screenCenterY, 0, LENGTH_SECOND_HAND_TAIL);
                _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 2, 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, 1, 1, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                //_display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, 1, 1, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _display.DrawRectangle(colorForeground, 1, DATE_RECT_LEFT_MARGIN, dateRectY, DATE_RECT_WIDTH, DATE_RECT_HEIGHT, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                _display.DrawText((currentTime.Day % 10).ToString("D1") + (currentTime.Day / 10).ToString("D1"), fontIPAexGothicReverseNumber06, colorForeground, DATE_RECT_LEFT_MARGIN + 2, dateRectY + 1);

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

                switch(displayMode)
                {
                    case DISPLAY_MODE_BLACK:

                        if (backgroundBitmapColor == BACKGROUND_BITMAP_COLOR_WHITE)
                        {
                            _azmdrawing.DrawImageReverseBW(_background, 0, 0, _background, 0, 0, _background.Width, _background.Height);
                            backgroundBitmapColor = BACKGROUND_BITMAP_COLOR_BLACK;
                        }
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:
                        if (backgroundBitmapColor == BACKGROUND_BITMAP_COLOR_BLACK)
                        {
                            _azmdrawing.DrawImageReverseBW(_background, 0, 0, _background, 0, 0, _background.Width, _background.Height);
                            backgroundBitmapColor = BACKGROUND_BITMAP_COLOR_WHITE;
                        }
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

