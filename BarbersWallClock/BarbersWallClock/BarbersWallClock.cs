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

        static DateTime currentTime;

        static Bitmap _background;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

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

        static int radius = 0;

        static int displayMode = DISPLAY_MODE_BLACK;


        const int LENGTH_HOUR_HAND = 31;
        const int LENGTH_HOUR_HAND_TAIL = 13;
        const int LENGTH_MINUTE_HAND = 41;
        const int LENGTH_MINUTE_HAND_TAIL = 13;
        const int LENGTH_SECOND_HAND = 41;
        const int LENGTH_SECOND_HAND_TAIL = 13;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;
        const int DISPLAY_MODE_DIGITAL = 2;


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

            displayMode = DISPLAY_MODE_BLACK;
            _background = new Bitmap(Resources.GetBytes(Resources.BinaryResources.BarbersWallClockBlack), Bitmap.BitmapImageType.Gif);
            colorForeground = Color.White;
            colorBackground = Color.Black;

            UpdateTime(null);

            currentTime = DateTime.Now;

            //TimeSpan dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            //TimeSpan period = new TimeSpan(0, 0, 1, 0, 0);
            TimeSpan dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            TimeSpan period = new TimeSpan(0, 0, 0, 1, 0);
            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        static void UpdateTime(object state)
        {

            currentTime = DateTime.Now;

            _display.Clear();

            if (displayMode == DISPLAY_MODE_BLACK || displayMode == DISPLAY_MODE_WHITE)
            {

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

            }
            else
            {

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _azmdrawing.DrawStringAligned(_display, colorForeground, font7barPBd24, currentTime.Hour.ToString("D2") + ":" + currentTime.Minute.ToString("D2") + ":" + currentTime.Second.ToString("D2"), AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_MIDDLE, 0);

            }

            _display.Flush();

        }

        private static void Current_OnButtonPress(Buttons button, Microsoft.SPOT.Hardware.InterruptPort port, ButtonDirection direction, DateTime time)
        {

            if (direction == ButtonDirection.Up)
            {
                if (button == Buttons.MiddleRight)
                {
                    if (displayMode == DISPLAY_MODE_BLACK)
                    {
                        displayMode = DISPLAY_MODE_WHITE;
                        _background.Dispose();
                        _background = new Bitmap(Resources.GetBytes(Resources.BinaryResources.BarbersWallClockWhite), Bitmap.BitmapImageType.Gif);
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                    }
                    else if (displayMode == DISPLAY_MODE_WHITE)
                    {
                        displayMode = DISPLAY_MODE_DIGITAL;
                        _background.Dispose();
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                    }
                    else
                    {
                        displayMode = DISPLAY_MODE_BLACK;
                        _background.Dispose();
                        _background = new Bitmap(Resources.GetBytes(Resources.BinaryResources.BarbersWallClockBlack), Bitmap.BitmapImageType.Gif);
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                    }

                    UpdateTime(null);

                }

            }

        }        


    }

}

