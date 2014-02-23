using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace Graffiti
{
    public class Graffiti
    {

        static Bitmap _display;

        static Bitmap _bmpFace;
        static Bitmap _bmpHourHand;
        static Bitmap _bmpMinuteHand;
        static Bitmap _bmpSecondHand;
        static Bitmap _bmpPin;
        static Bitmap _bmpRotateHourHand;
        static Bitmap _bmpRotateMinuteHand;
        static Bitmap _bmpRotateSecondHand;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

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

        static int hourHandWidth = 0;
        static int hourHandHeight = 0;
        static int rotateHourHandX = 0;
        static int rotateHourHandY = 0;

        static int minuteHandWidth = 0;
        static int minuteHandHeight = 0;
        static int rotateMinuteHandX = 0;
        static int rotateMinuteHandY = 0;

        static int secondHandWidth = 0;
        static int secondHandHeight = 0;
        static int rotateSecondHandX = 0;
        static int rotateSecondHandY = 0;

        static int modHourHandX = 0;
        static int modHourHandY = 0;
        static int modMinuteHandX = 0;
        static int modMinuteHandY = 0;
        static int modSecondHandX = 0;
        static int modSecondHandY = 0;

        static int pinWidth = 0;
        static int pinHeight = 0;
        static int pinX = 0;
        static int pinY = 0;

        static bool blackOrWhite = WHITE;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showSecondHand = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const bool WHITE = true;
        const bool BLACK = false;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MOD_HOUR_HAND_X = 0;
        const int MOD_HOUR_HAND_Y = 0;
        const int MOD_MINUTE_HAND_X = 0;
        const int MOD_MINUTE_HAND_Y = 0;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_SECOND = 0;
        const int DISPLAY_MODE_BLACK = 1;
        const int DISPLAY_MODE_WHITE_SECOND = 2;
        const int DISPLAY_MODE_WHITE = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _bmpFace = new Bitmap(Resources.GetBytes(Resources.BinaryResources.GraffitiFace), Bitmap.BitmapImageType.Gif);
            _bmpHourHand = new Bitmap(Resources.GetBytes(Resources.BinaryResources.GraffitiHourHand), Bitmap.BitmapImageType.Gif);
            _bmpMinuteHand = new Bitmap(Resources.GetBytes(Resources.BinaryResources.GraffitiMinuteHand), Bitmap.BitmapImageType.Gif);
            _bmpSecondHand = new Bitmap(Resources.GetBytes(Resources.BinaryResources.GraffitiSecondHand), Bitmap.BitmapImageType.Gif);
            _bmpPin = new Bitmap(Resources.GetBytes(Resources.BinaryResources.GraffitiPin), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            colorForeground = new Color();
            colorBackground = new Color();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            hourHandWidth = _bmpHourHand.Width;
            hourHandHeight = _bmpHourHand.Height;

            minuteHandWidth = _bmpMinuteHand.Width;
            minuteHandHeight = _bmpMinuteHand.Height;

            secondHandWidth = _bmpSecondHand.Width;
            secondHandHeight = _bmpSecondHand.Height;

            pinWidth = _bmpPin.Width;
            pinHeight = _bmpPin.Height;
            pinX = screenCenterX - (pinWidth / 2);
            pinY = screenCenterY - (pinHeight / 2);

            _bmpRotateHourHand = new Bitmap(hourHandWidth, hourHandHeight);
            _bmpRotateMinuteHand = new Bitmap(minuteHandWidth, minuteHandHeight);
            _bmpRotateSecondHand = new Bitmap(secondHandWidth, secondHandHeight);

            displayMode = DISPLAY_MODE_BLACK_SECOND;
            blackOrWhite = WHITE;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

            dueTime = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 0, 1, 0);

            //dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            //period = new TimeSpan(0, 0, 1, 0, 0);

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
                degreeS = _azmdrawing.SecondToAngle(currentTime.Second);

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                _display.DrawImage(0, 0, _bmpFace, 0, 0, screenWidth, screenHeight);


                _point = _azmdrawing.FindPointDegreeDistance(degreeH, 0, 0, (hourHandHeight / 2));
                modHourHandX = _point.X;
                modHourHandY = _point.Y;

                rotateHourHandX = (screenCenterX - (hourHandWidth / 2)) + _point.X;
                rotateHourHandY = (screenCenterY - (hourHandHeight / 2)) + _point.Y; 
                
                if (degreeH == 0)
                {

                    _azmdrawing.DrawImageTransparently(_bmpHourHand, 0, 0, _display, rotateHourHandX, rotateHourHandY, hourHandHeight, hourHandHeight, colorForeground);
                
                }
                else
                {

                    _bmpRotateHourHand.Clear();
                    _bmpRotateHourHand.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _bmpRotateHourHand.RotateImage(degreeH, 0, 0, _bmpHourHand, 0, 0, hourHandWidth, hourHandHeight, 255);

                    _azmdrawing.DrawImageTransparently(_bmpRotateHourHand, 0, 0, _display, rotateHourHandX, rotateHourHandY, hourHandHeight, hourHandHeight, colorForeground);

                }


                _point = _azmdrawing.FindPointDegreeDistance(degreeM, 0, 0, (minuteHandHeight / 2));
                modMinuteHandX = _point.X;
                modMinuteHandY = _point.Y;

                rotateMinuteHandX = (screenCenterX - (minuteHandWidth / 2)) + _point.X;
                rotateMinuteHandY = (screenCenterY - (minuteHandHeight / 2)) + _point.Y;

                if (degreeM == 0)
                {

                    _azmdrawing.DrawImageTransparently(_bmpMinuteHand, 0, 0, _display, rotateMinuteHandX, rotateMinuteHandY, minuteHandHeight, minuteHandHeight, colorForeground);

                }
                else
                {

                    _bmpRotateMinuteHand.Clear();
                    _bmpRotateMinuteHand.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                    _bmpRotateMinuteHand.RotateImage(degreeM, 0, 0, _bmpMinuteHand, 0, 0, minuteHandWidth, minuteHandHeight, 255);

                    _azmdrawing.DrawImageTransparently(_bmpRotateMinuteHand, 0, 0, _display, rotateMinuteHandX, rotateMinuteHandY, minuteHandHeight, minuteHandHeight, colorForeground);

                }


                if (showSecondHand == true)
                {

                    _point = _azmdrawing.FindPointDegreeDistance(degreeS, 0, 0, (secondHandHeight / 2));
                    modSecondHandX = _point.X;
                    modSecondHandY = _point.Y;

                    rotateSecondHandX = (screenCenterX - (secondHandWidth / 2)) + _point.X;
                    rotateSecondHandY = (screenCenterY - (secondHandHeight / 2)) + _point.Y;

                    if (degreeS == 0)
                    {

                        _azmdrawing.DrawImageTransparently(_bmpSecondHand, 0, 0, _display, rotateSecondHandX, rotateSecondHandY, secondHandWidth, secondHandHeight, colorForeground);

                    }
                    else
                    {

                        _bmpRotateSecondHand.Clear();
                        _bmpRotateSecondHand.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                        _bmpRotateSecondHand.RotateImage(degreeS, 0, 0, _bmpSecondHand, 0, 0, secondHandWidth, secondHandHeight, 255);

                        _azmdrawing.DrawImageTransparently(_bmpRotateSecondHand, 0, 0, _display, rotateSecondHandX, rotateSecondHandY, secondHandWidth, secondHandHeight, colorForeground);

                    }

                }

                _azmdrawing.DrawImageTransparently(_bmpPin, 0, 0, _display, pinX, pinY, pinWidth, pinHeight, colorForeground);

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
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

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
                        UpdateTime(null);
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
                        SetDisplayMode(displayMode);
                    }
                    else
                    {
                        showDigital = false;
                    }

                    UpdateTime(null);

                }
            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK_SECOND:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    showSecondHand = true;

                    break;

                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    showSecondHand = false;

                    break;

                case DISPLAY_MODE_WHITE_SECOND:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    showSecondHand = true;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    showSecondHand = false;

                    break;

            }

            ReverseBW();

        }

        private static void ReverseBW()
        {

            if (colorForeground == Color.White && blackOrWhite == BLACK)
            {

                _azmdrawing.DrawImageReverseBW(_bmpFace, 0, 0, _bmpFace, 0, 0, screenWidth, screenHeight);
                _azmdrawing.DrawImageReverseBW(_bmpHourHand, 0, 0, _bmpHourHand, 0, 0, hourHandWidth, hourHandHeight);
                _azmdrawing.DrawImageReverseBW(_bmpMinuteHand, 0, 0, _bmpMinuteHand, 0, 0, minuteHandWidth, minuteHandHeight);
                _azmdrawing.DrawImageReverseBW(_bmpSecondHand, 0, 0, _bmpSecondHand, 0, 0, secondHandWidth, secondHandHeight);
                _azmdrawing.DrawImageReverseBW(_bmpPin, 0, 0, _bmpPin, 0, 0, pinWidth, pinHeight);

                blackOrWhite = WHITE;

            }
            else if (colorForeground != Color.White && blackOrWhite == WHITE)
            {

                _azmdrawing.DrawImageReverseBW(_bmpFace, 0, 0, _bmpFace, 0, 0, screenWidth, screenHeight);
                _azmdrawing.DrawImageReverseBW(_bmpHourHand, 0, 0, _bmpHourHand, 0, 0, hourHandWidth, hourHandHeight);
                _azmdrawing.DrawImageReverseBW(_bmpMinuteHand, 0, 0, _bmpMinuteHand, 0, 0, minuteHandWidth, minuteHandHeight);
                _azmdrawing.DrawImageReverseBW(_bmpSecondHand, 0, 0, _bmpSecondHand, 0, 0, secondHandWidth, secondHandHeight);
                _azmdrawing.DrawImageReverseBW(_bmpPin, 0, 0, _bmpPin, 0, 0, pinWidth, pinHeight);

                blackOrWhite = BLACK;

            }

        }

        private static void UpdateTimeDigital(object state)
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

