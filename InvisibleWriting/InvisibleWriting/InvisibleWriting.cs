using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace InvisibleWriting
{
    public class InvisibleWriting
    {

        static Bitmap _display;
        static Bitmap _backgroundBlack;
        static Bitmap _backgroundWhite;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font fontSmallNumbers = Resources.GetFont(Resources.FontResources.SmallNumbers);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;
        static Color colorDisk;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;
        static int degreeM = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int backgroundWidth = 0;
        static int backgroundHeight = 0;

        static int drawX = 0;
        static int drawY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int LENGTH_HOUR_HAND = 50;
        const int THICKNESS_HOUR_HAND = 6;

        const int RADIUS_PIN_OUTER = 4;
        const int RADIUS_PIN_INNER = 3;

        const int MARGIN_OUTER_CIRCLE_EDGE = 2;
        const int MARGIN_INNER_CIRCLE_OUTER_CIRCLE = 10;
        const int MARGIN_MINUTE_NUMBER_OUTER_CIRCLE = 2;
        const int MARGIN_INNER_CIRCLE_DIAL = 3;
        const int WIDTH_MINUTE = 12;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DISK = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_DISK = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);
            _backgroundBlack = new Bitmap(Resources.GetBytes(Resources.BinaryResources.InvisibleWritingBlack), Bitmap.BitmapImageType.Gif);
            _backgroundWhite = new Bitmap(Resources.GetBytes(Resources.BinaryResources.InvisibleWritingWhite), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            backgroundWidth = _backgroundBlack.Width;
            backgroundHeight = _backgroundBlack.Height;

            drawX = (screenWidth - backgroundWidth) / 2 + 1;
            drawY = (screenHeight - backgroundHeight) / 2 + 1;

            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

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

                if (displayMode == DISPLAY_MODE_BLACK_DISK || displayMode == DISPLAY_MODE_WHITE_DISK)
                {
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_OUTER_CIRCLE_EDGE, screenCenterY - MARGIN_OUTER_CIRCLE_EDGE, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), screenCenterY - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }
                else
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - MARGIN_OUTER_CIRCLE_EDGE, screenCenterY - MARGIN_OUTER_CIRCLE_EDGE, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), screenCenterY - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), colorForeground, 0, 0, colorForeground, 0, 0, 0);
                }

                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, (degreeM - (WIDTH_MINUTE / 2) + 360) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), MARGIN_INNER_CIRCLE_OUTER_CIRCLE);
                _azmdrawing.DrawAngledLine(_display, colorForeground, 1, (degreeM + (WIDTH_MINUTE / 2) + 360) % 360, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_INNER_CIRCLE_OUTER_CIRCLE), MARGIN_INNER_CIRCLE_OUTER_CIRCLE);
                _point = _azmdrawing.FindPointDegreeDistance(degreeM, screenCenterX, screenCenterY, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + 2));
                _azmdrawing.FillArea(_display, colorForeground, _point.X, _point.Y);

                DrawMinuteNumber(_display);

                if (colorForeground != Color.Black)
                {
                    _azmdrawing.DrawImageTransparently(_backgroundBlack, 0, 0, _display, drawX, drawY, backgroundWidth, backgroundHeight, colorForeground);
                }
                else
                {
                    _azmdrawing.DrawImageTransparently(_backgroundWhite, 0, 0, _display, drawX, drawY, backgroundWidth, backgroundHeight, colorForeground);
                }

                _azmdrawing.DrawAngledLine(_display, colorForeground, 6, degreeH, screenCenterX, screenCenterY, 0, 50, 7);

                if (displayMode == DISPLAY_MODE_BLACK_DISK || displayMode == DISPLAY_MODE_WHITE_DISK)
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN_OUTER, RADIUS_PIN_OUTER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorDisk, 1, screenCenterX, screenCenterY, RADIUS_PIN_INNER, RADIUS_PIN_INNER, colorDisk, 0, 0, colorDisk, 0, 0, 255);
                }
                else
                {
                    _display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN_OUTER, RADIUS_PIN_OUTER, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                    _display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_PIN_INNER, RADIUS_PIN_INNER, colorBackground, 0, 0, colorBackground, 0, 0, 255);
                }

                _display.Flush();

            }

        }

        private static void DrawMinuteNumber(Bitmap screen)
        {

            for (int i = 0; i < 12; i++)
            {
                switch (i)
                {

                    case 0:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i + 2, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                    case 1:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i + 1, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) + 0);

                        break;

                    case 2:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) + 0);

                        break;

                    case 3:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                    case 4:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i - 1, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) + 0);

                        break;

                    case 5:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i - 1, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) + 0);

                        break;

                    case 6:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i - 2, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                    case 7:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                    case 8:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                    case 9:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 2);

                        break;

                    case 10:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                    case 11:

                        _point = _azmdrawing.FindPointDegreeDistance(30 * i + 1, 64, 64, screenCenterX - (MARGIN_OUTER_CIRCLE_EDGE + MARGIN_MINUTE_NUMBER_OUTER_CIRCLE + (fontSmallNumbers.Height / 2)) - 1);

                        break;

                }

                if (displayMode == DISPLAY_MODE_BLACK_DISK || displayMode == DISPLAY_MODE_WHITE_DISK)
                {
                    _azmdrawing.DrawStringCentered(_display, colorDisk, fontSmallNumbers, _point.X, _point.Y, (5 * i).ToString("D2"));
                }
                else
                {
                    _azmdrawing.DrawStringCentered(_display, colorBackground, fontSmallNumbers, _point.X, _point.Y, (5 * i).ToString("D2"));
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

                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    colorDisk = Color.Black;

                    break;

                case DISPLAY_MODE_BLACK_DISK:

                    colorForeground = Color.White;
                    colorBackground = Color.White;
                    colorDisk = Color.Black;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    colorDisk = Color.White;

                    break;

                case DISPLAY_MODE_WHITE_DISK:

                    colorForeground = Color.Black;
                    colorBackground = Color.Black;
                    colorDisk = Color.White;

                    break;

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

