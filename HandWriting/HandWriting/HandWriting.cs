using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace HandWriting
{
    public class HandWriting
    {

        static Bitmap _display;

        static Bitmap _bmpEraser;
        static Bitmap _bmpEraserWork;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateClockTimerHandWriting;
        static TimeSpan dueTimeHandWriting;
        static TimeSpan periodHandWriting;

        static Timer _updateClockTimerDigital;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static DateTime currentTime;

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);
        static Font fontSetoNumber32 = Resources.GetFont(Resources.FontResources.SetoNumber32);

        static Color colorForeground;
        static Color colorBackground;
        static Color colorBackgroundOld;
        static Color colorEraser;

        static AZMDrawing _azmdrawing;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int eraserWidth = 0;
        static int eraserHeight = 0;

        static int eraserCenterX = 0;
        static int eraserCenterY = 0;

        static int oldMM = 0;
        static string hHMM = "";

        static int h12h24 = 0;
        static int h1 = 0;
        static int h2 = 0;
        static int m1 = 0;
        static int m2 = 0;

        static bool handWriting = false;
        static int handWritingCounter = 0;
        static bool eraser = false;
        static int eraserCounter = 0;

        static int displayMode = DISPLAY_MODE_BLACK_12;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MILLISECOND_HAND_WRITING_INTERVAL = 200;
        const int ERASER_COUNT = 5;


        const int MAX_DISPLAY_MODE = 3;

        const int DISPLAY_MODE_BLACK_12 = 0;
        const int DISPLAY_MODE_BLACK_24 = 1;
        const int DISPLAY_MODE_WHITE_12 = 2;
        const int DISPLAY_MODE_WHITE_24 = 3;

        const string CHAR_TO_NUMBER = "@ABCDEFGHI";


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            colorForeground = new Color();
            colorBackground = new Color();
            colorBackgroundOld = new Color();
            colorEraser = new Color();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            _bmpEraser = new Bitmap(Resources.GetBytes(Resources.BinaryResources.Eraser), Bitmap.BitmapImageType.Gif);

            eraserWidth = _bmpEraser.Width;
            eraserHeight =_bmpEraser.Height;

            eraserCenterX = eraserWidth / 2;
            eraserCenterY = eraserHeight / 2;

            _bmpEraserWork = new Bitmap(eraserWidth, eraserHeight);
                
            currentTime = new DateTime();
            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeHandWriting = new TimeSpan(0, 0, 0, 0, 0);
            periodHandWriting = new TimeSpan(0, 0, 0, 0, MILLISECOND_HAND_WRITING_INTERVAL);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            displayMode = DISPLAY_MODE_BLACK_12;
            SetDisplayMode(displayMode);
            //_bmpEraserWork.DrawImage(0, 0, _bmpEraser, 0, 0, eraserWidth, eraserHeight);
            //colorEraser = Color.Black;

            eraser = false;
            oldMM = -1;

            UpdateTime(null);

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

            if (handWriting == false && showDigital == false)
            {

                if (currentTime.Minute != oldMM)
                {

                    handWriting = true;
                    handWritingCounter = 0;
                    eraserCounter = 0;

                    if (oldMM != -1)
                    {
                        eraser = true;
                    }

                    _updateClockTimerHandWriting = new Timer(UpdateTimeHandWriting, null, dueTimeHandWriting, periodHandWriting);

                }
                //else
                //{
                //    DrawFace();
                //}

            }

            if (currentTime.Minute != oldMM)
            {
                oldMM = currentTime.Minute;
            }

        }

        //private static void DrawFace()
        //{


        //    _display.Clear();

        //    _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

        //    _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoNumber32, "00:00", AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_MIDDLE, 0);

        //    _display.Flush();

        //}

        private static void UpdateTimeHandWriting(object state)
        {

            if (eraser == true)
            {

                if (eraserCounter == 0)
                {

                    if (colorBackgroundOld == Color.Black)
                    {
                        _bmpEraserWork.DrawImage(0, 0, _bmpEraser, 0, 0, eraserWidth, eraserHeight);
                        colorEraser = Color.Black;
                    }
                    else
                    {
                        _azmdrawing.DrawImageReverseBW(_bmpEraser, 0, 0, _bmpEraserWork, 0, 0, eraserWidth, eraserHeight);
                        colorEraser = Color.White;
                    }

                }

                if (eraserCounter < ERASER_COUNT)
                {
                    _azmdrawing.DrawImageTransparently(_bmpEraserWork, 0, 0, _display, (screenWidth - eraserWidth) / 2, screenCenterY - eraserCenterY, (eraserWidth / ERASER_COUNT) * (eraserCounter + 1), eraserHeight, colorEraser);
                    eraserCounter++;
                }
                //else if (eraserCounter == ERASER_COUNT)
                //{
                //    _azmdrawing.DrawImageReverseH(_bmpEraserWork, 0, 0, _bmpEraserWork, 0, 0, eraserWidth, eraserHeight);
                //    eraserCounter++;
                //}
                //else if (eraserCounter <= ERASER_COUNT * 2)
                //{
                //    _azmdrawing.DrawImageTransparently(_bmpEraserWork, 0, 0, _display, (screenWidth - eraserWidth) / 2, screenCenterY - eraserCenterY, (eraserWidth / ERASER_COUNT) * (eraserCounter - ERASER_COUNT), eraserHeight, colorEraser);
                //    eraserCounter++;
                //}
                else
                {

                    colorBackgroundOld = colorBackground;
                    eraser = false;

                }
            
            }
            else
            {

                h1 = (currentTime.Hour % h12h24) / 10;
                h2 = (currentTime.Hour % h12h24) % 10;
                m1 = currentTime.Minute / 10;
                m2 = currentTime.Minute % 10;

                switch (handWritingCounter)
                {

                    case 0:

                        hHMM = "";

                        break;

                    case 1:

                        hHMM = CHAR_TO_NUMBER.Substring(h1, 1) + "    ";

                        break;

                    case 2:

                        hHMM = h1.ToString() + "    ";

                        break;

                    case 3:

                        hHMM = h1.ToString() + CHAR_TO_NUMBER.Substring(h2, 1) + "   ";

                        break;

                    case 4:

                        hHMM = h1.ToString() + h2.ToString() + "   ";

                        break;

                    case 5:

                        hHMM = h1.ToString() + h2.ToString() + ";" + "  ";

                        break;

                    case 6:

                        hHMM = h1.ToString() + h2.ToString() + ":" + "  ";

                        break;

                    case 7:

                        hHMM = h1.ToString() + h2.ToString() + ":" + CHAR_TO_NUMBER.Substring(m1, 1) + " ";

                        break;

                    case 8:

                        hHMM = h1.ToString() + h2.ToString() + ":" + m1.ToString() + " ";

                        break;

                    case 9:

                        hHMM = h1.ToString() + h2.ToString() + ":" + m1.ToString() + CHAR_TO_NUMBER.Substring(m2, 1);

                        break;

                    case 10:

                        hHMM = h1.ToString() + h2.ToString() + ":" + m1.ToString() + m2.ToString();

                        break;

                }


                _display.Clear();
                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoNumber32, hHMM, AZMDrawing.ALIGN_CENTER, 0, AZMDrawing.VALIGN_MIDDLE, 0);

                if (10 <= handWritingCounter)
                {
                    handWriting = false;
                    _updateClockTimerHandWriting.Dispose();
                }
                else
                {
                    handWritingCounter++;
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
                    if (showDigital == false && handWriting == false)
                    {
                        --displayMode;
                        if (displayMode < 0)
                        {
                            displayMode = MAX_DISPLAY_MODE;
                        }

                        SetDisplayMode(displayMode);

                        eraser = true;
                        oldMM = -1;
                        UpdateTime(null);

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

                        try {
                            _updateClockTimerHandWriting.Dispose();
                        }
                        finally
                        {
                            handWriting = false;
                        }

                        
                        showDigital = true;
                        showDigitalCounter = 0;
                        UpdateTimeDigital(null);
                        _updateClockTimerDigital = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);
                    }
                    else
                    {
                        showDigital = false;
                        eraser = false;
                        oldMM = -1;
                        UpdateTime(null);
                    }
                }
                else if (button == Buttons.BottomRight)
                {

                    if (showDigital == false && handWriting == false)
                    {
                        ++displayMode;
                        if (displayMode > MAX_DISPLAY_MODE)
                        {
                            displayMode = 0;
                        }

                        SetDisplayMode(displayMode);

                        eraser = true;
                        oldMM = -1;
                        UpdateTime(null);


                    }
                    else
                    {
                        showDigital = false;
                    }

                }
            }

        }

        private static void SetDisplayMode(int displayMode)
        {

            colorBackgroundOld = colorBackground;

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK_12:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    h12h24 = 12;

                    break;


                case DISPLAY_MODE_BLACK_24:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    h12h24 = 24;

                    break;


                case DISPLAY_MODE_WHITE_12:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    h12h24 = 12;

                    break;

                case DISPLAY_MODE_WHITE_24:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    h12h24 = 24;

                    break;

            }

        }

        private static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;


            if (showDigital == false || showDigitalCounter > SHOW_DIGITAL_SECOND)
            {
                showDigital = false;
                eraser = false;
                oldMM = -1;
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

