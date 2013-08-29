using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace NumeralHand
{
    public class NumeralHand
    {

        static Bitmap _display;
        static Bitmap bmpRotate;
        static Bitmap bmpHandBlack;
        static Bitmap bmpHandWhite;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontsmall = Resources.GetFont(Resources.FontResources.small);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDate = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_DIAL_EDGE = 3;
        const int MARGIN_NUMERAL_NUMERAL = 3;
        const int MARGIN_PIN_NUMERAL = 3;

        const int NUMERAL_WIDTH = 9;
        const int NUMERAL_HEIGHT = 20;

        const int OFFSET_NUMERAL_X = 5;
        const int OFFSET_NUMERAL_Y = 25;

        //const int CENTER_PIN_SIZE = 9;
        const int CENTER_PIN_SIZE = 7;

        const int DIAL_TYPE = 6;

        const int DATE_WIDTH = 19;
        const int DATE_HEIGHT = 13;
        const int DATE_MARGIN = 13;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_BLACK_DATE = 1;
        const int DISPLAY_MODE_WHITE = 2;
        const int DISPLAY_MODE_WHITE_DATE = 3;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            bmpRotate = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);
            bmpHandBlack = new Bitmap(Resources.GetBytes(Resources.BinaryResources.NumeralHandBlack), Bitmap.BitmapImageType.Gif);
            bmpHandWhite = new Bitmap(Resources.GetBytes(Resources.BinaryResources.NumeralHandWhite), Bitmap.BitmapImageType.Gif);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK;
            showDate = false;
            colorForeground = Color.White;
            colorBackground = Color.Black;

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

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                bmpRotate.Clear();

                bmpRotate.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_BLACK || displayMode == DISPLAY_MODE_BLACK_DATE)
                {
                    bmpRotate.DrawImage(screenCenterX - OFFSET_NUMERAL_X, screenCenterY - (CENTER_PIN_SIZE / 2) - MARGIN_PIN_NUMERAL - NUMERAL_HEIGHT - MARGIN_NUMERAL_NUMERAL - NUMERAL_HEIGHT, bmpHandWhite, NUMERAL_WIDTH * (currentTime.Minute / 10), 0, NUMERAL_WIDTH, NUMERAL_HEIGHT);
                    bmpRotate.DrawImage(screenCenterX - OFFSET_NUMERAL_X, screenCenterY - (CENTER_PIN_SIZE / 2) - MARGIN_PIN_NUMERAL - NUMERAL_HEIGHT, bmpHandWhite, NUMERAL_WIDTH * (currentTime.Minute % 10), 0, NUMERAL_WIDTH, NUMERAL_HEIGHT);
                }

                if (displayMode == DISPLAY_MODE_WHITE || displayMode == DISPLAY_MODE_WHITE_DATE)
                {
                    bmpRotate.DrawImage(screenCenterX - OFFSET_NUMERAL_X, screenCenterY - (CENTER_PIN_SIZE / 2) - MARGIN_PIN_NUMERAL - NUMERAL_HEIGHT - MARGIN_NUMERAL_NUMERAL - NUMERAL_HEIGHT, bmpHandBlack, NUMERAL_WIDTH * (currentTime.Minute / 10), 0, NUMERAL_WIDTH, NUMERAL_HEIGHT);
                    bmpRotate.DrawImage(screenCenterX - OFFSET_NUMERAL_X, screenCenterY - (CENTER_PIN_SIZE / 2) - MARGIN_PIN_NUMERAL - NUMERAL_HEIGHT, bmpHandBlack, NUMERAL_WIDTH * (currentTime.Minute % 10), 0, NUMERAL_WIDTH, NUMERAL_HEIGHT);
                }

                bmpRotate.DrawRectangle(colorForeground, 1, screenCenterX - (CENTER_PIN_SIZE / 2), screenCenterY - (CENTER_PIN_SIZE / 2), CENTER_PIN_SIZE, CENTER_PIN_SIZE, 0, 0, colorForeground, 0, 0, colorForeground, 0, 0, 255);

                _display.RotateImage(degreeH, 0, 0, bmpRotate, 0, 0, bmpRotate.Width, bmpRotate.Height, 255);

                _azmdrawing.DrawDial(_display, colorForeground, colorBackground, DIAL_TYPE, MARGIN_DIAL_EDGE);

                //_display.DrawEllipse(colorForeground, 1, screenCenterX, screenCenterY, RADIUS_PIN, RADIUS_PIN, colorForeground, 0, 0, colorForeground, 0, 0, 255);
                //_display.DrawEllipse(colorBackground, 1, screenCenterX, screenCenterY, RADIUS_PIN - 2, RADIUS_PIN - 2, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (showDate == true)
                {
                    _display.DrawRectangle(colorForeground, 1, screenWidth - DATE_MARGIN - DATE_WIDTH - 1, screenCenterY - (DATE_HEIGHT / 2) - 1, DATE_WIDTH + 2, DATE_HEIGHT + 2, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    _display.DrawRectangle(colorForeground, 1, screenWidth - DATE_MARGIN - DATE_WIDTH, screenCenterY - (DATE_HEIGHT / 2), DATE_WIDTH, DATE_HEIGHT, 2, 2, colorForeground, 0, 0, colorForeground, 0, 0, 0);
                    _display.DrawText(currentTime.Day.ToString("D2"), fontsmall, colorForeground, screenWidth - DATE_MARGIN - DATE_WIDTH + 4, screenCenterY - (DATE_HEIGHT / 2));
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

                    case DISPLAY_MODE_BLACK:

                        showDate = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DATE:

                        showDate = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE:

                        showDate = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DATE:

                        showDate = true;
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

