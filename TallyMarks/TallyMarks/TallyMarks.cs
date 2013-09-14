using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace TallyMarks
{
    public class TallyMarks
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static DateTime currentTime;

        static Font fontTallyMarksOccidental18 = Resources.GetFont(Resources.FontResources.TallyMarksOccidental18);
        static Font fontTallyMarksOriental18 = Resources.GetFont(Resources.FontResources.TallyMarksOriental18);
        static Font fontTallyMarksLatin18 = Resources.GetFont(Resources.FontResources.TallyMarksLatin18);

        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static int screenWidth = 0;
        static int screenHeight = 0;

        static int screenCenterX = 0;
        static int screenCenterY = 0;

        static int numH = 0;
        static int numMUpper = 0;
        static int numMLower = 0;
        static int numSUpper = 0;
        static int numSLower = 0;
        
        static string markH = "";
        static string markMUpper = "";
        static string markMLower = "";
        static string markSUpper = "";
        static string markSLower = "";

        static int topH = 0;
        static int topLine1 = 0;
        static int topMUpper = 0;
        static int topMLower = 0;
        static int topLine2 = 0;
        static int topSUpper = 0;
        static int topSLower = 0;

        static int marginLeft = 0;

        static int displayMode = DISPLAY_MODE_BLACK_OCCIDENTAL;

        static int markType = MARK_TYPE_OCCIDENTAL;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MAX_DISPLAY_MODE = 5;

        const int MARGIN_MARK_MARK = 5;
        const int MARGIN_MARK_LINE = 5;
        const int HEIGHT_MARK = 16;
        const int THICKNESS_LINE = 2;
        const int MOD_TOP = -1;

        const int MARK_TYPE_OCCIDENTAL = 0;
        const int MARK_TYPE_ORIENTAL = 1;
        const int MARK_TYPE_LATIN = 2;

        const int DISPLAY_MODE_BLACK_OCCIDENTAL = 0;
        const int DISPLAY_MODE_BLACK_ORIENTAL = 1;
        const int DISPLAY_MODE_BLACK_LATIN = 2;
        const int DISPLAY_MODE_WHITE_OCCIDENTAL = 3;
        const int DISPLAY_MODE_WHITE_ORIENTAL = 4;
        const int DISPLAY_MODE_WHITE_LATIN = 5;


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            screenCenterX = screenWidth / 2;
            screenCenterY = screenHeight / 2;

            topH = (screenHeight - ((HEIGHT_MARK * 5) + (MARGIN_MARK_MARK * 2) + (MARGIN_MARK_LINE * 4) + (THICKNESS_LINE * 2))) / 2 + MOD_TOP;

            topLine1 = topH + ((HEIGHT_MARK * 1) + (MARGIN_MARK_MARK * 0) + (MARGIN_MARK_LINE * 1) + (THICKNESS_LINE * 0));
            
            topMLower = topH + ((HEIGHT_MARK * 1) + (MARGIN_MARK_MARK * 0) + (MARGIN_MARK_LINE * 2) + (THICKNESS_LINE * 1));
            topMUpper = topH + ((HEIGHT_MARK * 2) + (MARGIN_MARK_MARK * 1) + (MARGIN_MARK_LINE * 2) + (THICKNESS_LINE * 1));

            topLine2 = topH + ((HEIGHT_MARK * 3) + (MARGIN_MARK_MARK * 1) + (MARGIN_MARK_LINE * 3) + (THICKNESS_LINE * 1));

            topSLower = topH + ((HEIGHT_MARK * 3) + (MARGIN_MARK_MARK * 1) + (MARGIN_MARK_LINE * 4) + (THICKNESS_LINE * 2));
            topSUpper = topH + ((HEIGHT_MARK * 4) + (MARGIN_MARK_MARK * 2) + (MARGIN_MARK_LINE * 4) + (THICKNESS_LINE * 2));


            marginLeft = (screenWidth - (fontTallyMarksOccidental18.CharWidth('0') * 6)) / 2;

            displayMode = DISPLAY_MODE_BLACK_OCCIDENTAL;
            SetDisplayMode();

            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            UpdateTime(null);

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

            if (showDigital == false)
            {

                _display.Clear();

                _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

                for (int i = 1; i < THICKNESS_LINE; i++)
                {
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_LINE, 90, 0, topLine1 + i + 1, 0, screenWidth);
                    _azmdrawing.DrawAngledLine(_display, colorForeground, THICKNESS_LINE, 90, 0, topLine2 + i + 1, 0, screenWidth);
                }

                numH = currentTime.Hour;

                if (30 <= currentTime.Minute)
                {
                    numMUpper = currentTime.Minute - 30;
                    numMLower = 30;
                }
                else
                {
                    numMUpper = 0;
                    numMLower = currentTime.Minute % 30;
                }

                if (30 <= currentTime.Second)
                {
                    numSUpper = currentTime.Second - 30;
                    numSLower = 30;
                }
                else
                {
                    numSUpper = 0;
                    numSLower = currentTime.Second % 30;
                }

                markH = "";
                markMUpper = "";
                markMLower = "";
                markSUpper = "";
                markSLower = "";


                for (int i = 1; i <= (numH / 5); i++)
                {
                    markH = markH + "5";
                }

                markH = markH + (numH % 5).ToString();


                for (int i = 1; i <= (numMUpper / 5); i++)
                {
                    markMUpper = markMUpper + "5";
                }

                markMUpper = markMUpper + (numMUpper % 5).ToString();


                for (int i = 1; i <= (numMLower / 5); i++)
                {
                    markMLower = markMLower + "5";
                }

                markMLower = markMLower + (numMLower % 5).ToString();


                for (int i = 1; i <= (numSUpper / 5); i++)
                {
                    markSUpper = markSUpper + "5";
                }

                markSUpper = markSUpper + (numSUpper % 5).ToString();


                for (int i = 1; i <= (numSLower / 5); i++)
                {
                    markSLower = markSLower + "5";
                }

                markSLower = markSLower + (numSLower % 5).ToString();

                
                switch (markType)
                {

                    case MARK_TYPE_OCCIDENTAL:

                        _display.DrawText(markH, fontTallyMarksOccidental18, colorForeground, marginLeft, topH);

                        _display.DrawText(markMLower, fontTallyMarksOccidental18, colorForeground, marginLeft, topMLower);
                        _display.DrawText(markMUpper, fontTallyMarksOccidental18, colorForeground, marginLeft, topMUpper);

                        _display.DrawText(markSLower, fontTallyMarksOccidental18, colorForeground, marginLeft, topSLower);
                        _display.DrawText(markSUpper, fontTallyMarksOccidental18, colorForeground, marginLeft, topSUpper);

                        break;


                    case MARK_TYPE_ORIENTAL:

                        _display.DrawText(markH, fontTallyMarksOriental18, colorForeground, marginLeft, topH);

                        _display.DrawText(markMLower, fontTallyMarksOriental18, colorForeground, marginLeft, topMLower);
                        _display.DrawText(markMUpper, fontTallyMarksOriental18, colorForeground, marginLeft, topMUpper);

                        _display.DrawText(markSLower, fontTallyMarksOriental18, colorForeground, marginLeft, topSLower);
                        _display.DrawText(markSUpper, fontTallyMarksOriental18, colorForeground, marginLeft, topSUpper);
                        
                        break;


                    case MARK_TYPE_LATIN:

                        _display.DrawText(markH, fontTallyMarksLatin18, colorForeground, marginLeft, topH);

                        _display.DrawText(markMLower, fontTallyMarksLatin18, colorForeground, marginLeft, topMLower);
                        _display.DrawText(markMUpper, fontTallyMarksLatin18, colorForeground, marginLeft, topMUpper);

                        _display.DrawText(markSLower, fontTallyMarksLatin18, colorForeground, marginLeft, topSLower);
                        _display.DrawText(markSUpper, fontTallyMarksLatin18, colorForeground, marginLeft, topSUpper);
                        
                        break;


                }
                                
            }
            else
            {

                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
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
                else if (button == Buttons.MiddleRight)
                {
                    if (showDigital != true)
                    {
                        showDigital = true;
                        showDigitalCounter = 0;
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

                SetDisplayMode();

                UpdateTime(null);

            }

        }

        private static void SetDisplayMode()
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK_OCCIDENTAL:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    markType = MARK_TYPE_OCCIDENTAL;

                    break;

                case DISPLAY_MODE_BLACK_ORIENTAL:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    markType = MARK_TYPE_ORIENTAL;

                    break;

                case DISPLAY_MODE_BLACK_LATIN:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;
                    markType = MARK_TYPE_LATIN;

                    break;

                case DISPLAY_MODE_WHITE_OCCIDENTAL:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    markType = MARK_TYPE_OCCIDENTAL;

                    break;

                case DISPLAY_MODE_WHITE_ORIENTAL:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    markType = MARK_TYPE_ORIENTAL;

                    break;

                case DISPLAY_MODE_WHITE_LATIN:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;
                    markType = MARK_TYPE_LATIN;

                    break;

            }
        }

    }

}

