using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace OctDecHex
{
    public class OctDecHex
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontLetsGoDigitalFixedNumber20 = Resources.GetFont(Resources.FontResources.LetsGoDigitalFixedNumber20);
        static Font fontLetsGoDigital11 = Resources.GetFont(Resources.FontResources.LetsGoDigital11);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static string stringH = "";
        static string stringM = "";
        static string stringS = "";

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int displayMode = DISPLAY_MODE_BLACK_OCT;

        static int baseNumber = 0;
        static string baseNumberString = "";
        static bool showBase = false;

        static bool showDigital = false;
        static int showDigitalCounter = 0;


        const int SHOW_DIGITAL_SECOND = 10;

        const int MARGIN_BASE_EDGE = 5;

        const int MAX_DISPLAY_MODE = 11;

        const int DISPLAY_MODE_BLACK_OCT = 0;
        const int DISPLAY_MODE_BLACK_DEC = 1;
        const int DISPLAY_MODE_BLACK_HEX = 2;
        const int DISPLAY_MODE_BLACK_OCT_BASE = 3;
        const int DISPLAY_MODE_BLACK_DEC_BASE = 4;
        const int DISPLAY_MODE_BLACK_HEX_BASE = 5;
        const int DISPLAY_MODE_WHITE_OCT = 6;
        const int DISPLAY_MODE_WHITE_DEC = 7;
        const int DISPLAY_MODE_WHITE_HEX = 8;
        const int DISPLAY_MODE_WHITE_OCT_BASE = 9;
        const int DISPLAY_MODE_WHITE_DEC_BASE = 10;
        const int DISPLAY_MODE_WHITE_HEX_BASE = 11;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            displayMode = DISPLAY_MODE_BLACK_OCT;
            baseNumber = 8;
            showBase = false;
            colorForeground = Color.White;
            colorBackground = Color.Black;

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

            currentTime = DateTime.Now;

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

            _display.Clear();


            if (showDigital == false)
            {

                stringH = _azmdrawing.ChangeBaseNumber(currentTime.Hour, baseNumber);
                stringM = _azmdrawing.ChangeBaseNumber(currentTime.Minute, baseNumber);
                stringS = _azmdrawing.ChangeBaseNumber(currentTime.Second, baseNumber);

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawStringAligned(_display, colorForeground, fontLetsGoDigitalFixedNumber20, stringH + ":" + stringM + ":" + stringS, AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_MIDDLE, 0);


                if (showBase == true)
                {
                    _azmdrawing.DrawStringAligned(_display, colorForeground, fontLetsGoDigital11, baseNumberString, AZMDrawing.ALIGN_RIGHT, MARGIN_BASE_EDGE, AZMDrawing.VALIGN_BOTTOM, MARGIN_BASE_EDGE);
                }


            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, fontLetsGoDigitalFixedNumber20, currentTime, true);
                ++showDigitalCounter;

                if (showDigitalCounter > SHOW_DIGITAL_SECOND)
                {
                    showDigital = false;
                    showDigitalCounter = 0;
                }

            }

            _display.Flush();

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
                        UpdateTime(null);
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

                    case DISPLAY_MODE_BLACK_OCT:

                        baseNumber = 8;
                        baseNumberString = "OCT";
                        showBase = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DEC:

                        baseNumber = 10;
                        baseNumberString = "DEC";
                        showBase = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_HEX:

                        baseNumber = 16;
                        baseNumberString = "HEX";
                        showBase = false;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_OCT_BASE:

                        baseNumber = 8;
                        baseNumberString = "OCT";
                        showBase = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_DEC_BASE:

                        baseNumber = 10;
                        baseNumberString = "DEC";
                        showBase = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_BLACK_HEX_BASE:

                        baseNumber = 16;
                        baseNumberString = "HEX";
                        showBase = true;
                        colorForeground = Color.White;
                        colorBackground = Color.Black;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_OCT:

                        baseNumber = 8;
                        baseNumberString = "OCT";
                        showBase = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DEC:

                        baseNumber = 10;
                        baseNumberString = "DEC";
                        showBase = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_HEX:

                        baseNumber = 16;
                        baseNumberString = "HEX";
                        showBase = false;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_OCT_BASE:

                        baseNumber = 8;
                        baseNumberString = "OCT";
                        showBase = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_DEC_BASE:

                        baseNumber = 10;
                        baseNumberString = "DEC";
                        showBase = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                    case DISPLAY_MODE_WHITE_HEX_BASE:

                        baseNumber = 16;
                        baseNumberString = "HEX";
                        showBase = true;
                        colorForeground = Color.Black;
                        colorBackground = Color.White;
                        UpdateTime(null);

                        break;

                }

            }

        }

    }

}
