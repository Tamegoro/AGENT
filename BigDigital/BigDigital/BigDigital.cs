using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace BigDigital
{
    public class BigDigital
    {

        static Bitmap _display;

        static Bitmap _baseBlack;
        static Bitmap _baseWhite;

        static Bitmap _bmpWork;

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
        static Color colorFill;

        static AZMDrawing _azmdrawing;
        static AGENT.AZMutil.Point _point;

        static int degreeH = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_12;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_NUMBER_EDGE_X = 1;
        const int MARGIN_NUMBER_EDGE_Y = 1;

        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_12 = 0;
        const int DISPLAY_MODE_BLACK_24 = 1;
        const int DISPLAY_MODE_WHITE_12 = 2;
        const int DISPLAY_MODE_WHITE_24 = 3;

        const int BAR_LEFT_UPPER = 0;
        const int BAR_LEFT_LOWER = 1;
        const int BAR_TOP = 2;
        const int BAR_MIDDLE = 3;
        const int BAR_BOTTOM = 4;
        const int BAR_RIGHT_UPPER = 5;
        const int BAR_RIGHT_LOWER = 6;

        const int FILL_LEFT_UPPER_X = 7;
        const int FILL_LEFT_UPPER_Y = 19;
        const int FILL_LEFT_LOWER_X = 7;
        const int FILL_LEFT_LOWER_Y = 43;
        const int FILL_TOP_X = 32;
        const int FILL_TOP_Y = 7;
        const int FILL_MIDDLE_X = 32;
        const int FILL_MIDDLE_Y = 31;
        const int FILL_BOTTOM_X = 32;
        const int FILL_BOTTOM_Y = 55;
        const int FILL_RIGHT_UPPER_X = 55;
        const int FILL_RIGHT_UPPER_Y = 19;
        const int FILL_RIGHT_LOWER_X = 55;
        const int FILL_RIGHT_LOWER_Y = 43;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _baseBlack = new Bitmap(Resources.GetBytes(Resources.BinaryResources.BigDigitalBlack), Bitmap.BitmapImageType.Gif);
            _baseWhite = new Bitmap(Resources.GetBytes(Resources.BinaryResources.BigDigitalWhite), Bitmap.BitmapImageType.Gif);

            _bmpWork = new Bitmap(_baseBlack.Width, _baseBlack.Height);

            _azmdrawing = new AZMDrawing();
            _point = new AGENT.AZMutil.Point();

            colorForeground = new Color();
            colorBackground = new Color();
            colorFill = new Color();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_12;
            SetDisplayMode(displayMode);

            showDigital = false;

            currentTime = new DateTime();

            UpdateTime(null);

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

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                if (displayMode == DISPLAY_MODE_BLACK_12 || displayMode == DISPLAY_MODE_WHITE_12)
                {
                    DrawNumber(_display, (currentTime.Hour % 12) / 10, colorForeground, MARGIN_NUMBER_EDGE_X, MARGIN_NUMBER_EDGE_Y);
                    DrawNumber(_display, (currentTime.Hour % 12) % 10, colorForeground, MARGIN_NUMBER_EDGE_X + _baseBlack.Width, MARGIN_NUMBER_EDGE_Y);
                }
                else
                {
                    DrawNumber(_display, currentTime.Hour / 10, colorForeground, MARGIN_NUMBER_EDGE_X, MARGIN_NUMBER_EDGE_Y);
                    DrawNumber(_display, currentTime.Hour % 10, colorForeground, MARGIN_NUMBER_EDGE_X + _baseBlack.Width, MARGIN_NUMBER_EDGE_Y);
                }

                DrawNumber(_display, currentTime.Minute / 10, colorForeground, MARGIN_NUMBER_EDGE_X, MARGIN_NUMBER_EDGE_Y + _baseBlack.Height);
                DrawNumber(_display, currentTime.Minute % 10, colorForeground, MARGIN_NUMBER_EDGE_X + _baseBlack.Width, MARGIN_NUMBER_EDGE_Y + _baseBlack.Height);

                _display.Flush();

            }

        }

        private static void DrawNumber(Bitmap _bmp, int number, Color color, int xDst, int yDst)
        {

            if (color != Color.Black)
            {
                _bmpWork.DrawImage(0, 0, _baseWhite, 0, 0, _baseWhite.Width, _baseWhite.Height);
            }
            else
            {
                _bmpWork.DrawImage(0, 0, _baseBlack, 0, 0, _baseBlack.Width, _baseBlack.Height);
            }

            switch (number)
            {

                case 0:

                    FillBar(_bmpWork, BAR_MIDDLE, color);

                    break;

                case 1:

                    FillBar(_bmpWork, BAR_LEFT_UPPER, color);
                    FillBar(_bmpWork, BAR_LEFT_LOWER, color);
                    FillBar(_bmpWork, BAR_TOP, color);
                    FillBar(_bmpWork, BAR_MIDDLE, color);
                    FillBar(_bmpWork, BAR_BOTTOM, color);
                    
                    break;

                case 2:

                    FillBar(_bmpWork, BAR_LEFT_UPPER, color);
                    FillBar(_bmpWork, BAR_RIGHT_LOWER, color);

                    break;

                case 3:

                    FillBar(_bmpWork, BAR_LEFT_UPPER, color);
                    FillBar(_bmpWork, BAR_LEFT_LOWER, color);

                    break;

                case 4:

                    FillBar(_bmpWork, BAR_LEFT_LOWER, color);
                    FillBar(_bmpWork, BAR_TOP, color);
                    FillBar(_bmpWork, BAR_BOTTOM, color);

                    break;

                case 5:

                    FillBar(_bmpWork, BAR_LEFT_LOWER, color);
                    FillBar(_bmpWork, BAR_RIGHT_UPPER, color);

                    break;

                case 6:

                    FillBar(_bmpWork, BAR_RIGHT_UPPER, color);

                    break;

                case 7:

                    FillBar(_bmpWork, BAR_LEFT_UPPER, color);
                    FillBar(_bmpWork, BAR_LEFT_LOWER, color);
                    FillBar(_bmpWork, BAR_MIDDLE, color);
                    FillBar(_bmpWork, BAR_BOTTOM, color);

                    break;

                case 8:

                    break;

                case 9:

                    FillBar(_bmpWork, BAR_LEFT_LOWER, color);

                    break;

            }

            _azmdrawing.DrawImageTransparently(_bmpWork, 0, 0, _bmp, xDst, yDst, _bmpWork.Width, _bmpWork.Height, color); 

        }

        private static void FillBar(Bitmap _bmp, int bar, Color color)
        {



            if (color != Color.Black)
            {
                colorFill = Color.Black;
            }
            else
            {
                colorFill = Color.White;
            }
            
            switch (bar)
            {

                case BAR_LEFT_UPPER:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_LEFT_UPPER_X, FILL_LEFT_UPPER_Y);

                    break;

                case BAR_LEFT_LOWER:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_LEFT_LOWER_X, FILL_LEFT_LOWER_Y);

                    break;

                case BAR_TOP:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_TOP_X, FILL_TOP_Y);

                    break;

                case BAR_MIDDLE:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_MIDDLE_X, FILL_MIDDLE_Y);

                    break;

                case BAR_BOTTOM:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_BOTTOM_X, FILL_BOTTOM_Y);

                    break;

                case BAR_RIGHT_UPPER:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_RIGHT_UPPER_X, FILL_RIGHT_UPPER_Y);

                    break;

                case BAR_RIGHT_LOWER:

                    _azmdrawing.FillArea(_bmp, colorFill, FILL_RIGHT_LOWER_X, FILL_RIGHT_LOWER_Y);

                    break;

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

                case DISPLAY_MODE_BLACK_12:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;

                    break;

                case DISPLAY_MODE_BLACK_24:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;

                    break;

                case DISPLAY_MODE_WHITE_12:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;

                    break;

                case DISPLAY_MODE_WHITE_24:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;

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

