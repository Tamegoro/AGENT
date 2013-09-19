using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;
using AGENT.AZMutil.AZMDrawing;
using Agent.Contrib.Hardware;


namespace AnimatedJapaneseFuzzyClock
{
    public class AnimatedJapaneseFuzzyClock
    {

        static Bitmap _display;

        static Timer _updateClockTimer;
        static TimeSpan dueTime;
        static TimeSpan period;

        static Timer _updateDigitalTimer;
        static TimeSpan dueTimeDigital;
        static TimeSpan periodDigital;

        static Timer _updateAnimationTimer;
        static TimeSpan dueTimeAnimation;
        static TimeSpan periodAnimation;

        static DateTime currentTime;

        static Font fontSetoAJFC14 = Resources.GetFont(Resources.FontResources.SetoAJFC14);
        static Font fontSetoAJFC18 = Resources.GetFont(Resources.FontResources.SetoAJFC18);
        static Font font7barPBd24 = Resources.GetFont(Resources.FontResources._7barPBd24);

        static Color colorForeground;
        static Color colorBackground;

        static AZMDrawing _azmdrawing;

        static Random _random;

        static int fluctuation = 0;

        static int screenWidth = 0;
        static int screenHeight = 0;

        //static bool showAnimation = false;

        static int messageNumber = 0;
        static int oldMessageNumber = 0;

        static int lineNumbers = 0;

        static string line1String = "";
        static string line2String = "";
        static string line3String = "";

        static int line1Left = 0;
        static int line2Left = 0;
        static int line3Left = 0;

        static int displayMode = DISPLAY_MODE_BLACK;

        static bool showDigital = false;
        static int showDigitalCounter = 0;

        static bool buttonFlag = false;

        const int MAX_FLUCTUATION = 2;

        const int SHOW_DIGITAL_SECOND = 10;

        const int MAX_DISPLAY_MODE = 1;

        const int MARGIN_HEIGHT_2_LINES = 25;
        const int MARGIN_HEIGHT_3_LINES = 10;
        const int SCROLL_WIDTH = 7;

        const int ANIMATION_INTERVAL = 100;

        const int DISPLAY_MODE_BLACK = 0;
        const int DISPLAY_MODE_WHITE = 1;


        const string MESSAGE_101_01 = "あけまして";
        const string MESSAGE_101_02 = "おめでとう";
        const string MESSAGE_101_03 = "ございます";

        const string MESSAGE_102_01 = "Happy";
        const string MESSAGE_102_02 = "Birthday";
        const string MESSAGE_102_03 = "AGENT!";

        const string MESSAGE_103_01 = "年越し";
        const string MESSAGE_103_02 = "蕎麦";
        const string MESSAGE_103_03 = "食べた?";

        const string MESSAGE_104_01 = "おやつの";
        const string MESSAGE_104_02 = "時間";

        //const string MESSAGE_105_01 = "真夜中";
        //const string MESSAGE_105_02 = "前後";
        const string MESSAGE_105_01 = "そろそろ";
        const string MESSAGE_105_02 = "寝ないと";
        const string MESSAGE_105_03 = "マズイよ";

        const string MESSAGE_01_01 = "だいたい";
        const string MESSAGE_01_02 = "時";
        const string MESSAGE_01_03 = "くらい";

        const string MESSAGE_02_01 = "時";
        const string MESSAGE_02_02 = "ちょい";
        const string MESSAGE_02_03 = "過ぎ";

        const string MESSAGE_03_01 = "そろそろ";
        const string MESSAGE_03_02 = "時";
        const string MESSAGE_03_03 = "十五分";

        const string MESSAGE_04_01 = "時";
        const string MESSAGE_04_02 = "十五分";
        const string MESSAGE_04_03 = "前後";

        const string MESSAGE_05_01 = "時";
        const string MESSAGE_05_02 = "十五分";
        const string MESSAGE_05_03 = "過ぎ";

        const string MESSAGE_06_01 = "そろそろ";
        const string MESSAGE_06_02 = "時半";

        const string MESSAGE_07_01 = "だいたい";
        const string MESSAGE_07_02 = "時半";
        const string MESSAGE_07_03 = "くらい";

        const string MESSAGE_08_01 = "時半";
        const string MESSAGE_08_02 = "ちょい";
        const string MESSAGE_08_03 = "過ぎ";

        const string MESSAGE_09_01 = "そろそろ";
        const string MESSAGE_09_02 = "時";
        const string MESSAGE_09_03 = "四十五分";

        const string MESSAGE_10_01 = "時";
        const string MESSAGE_10_02 = "四十五分";
        const string MESSAGE_10_03 = "前後";

        const string MESSAGE_11_01 = "時";
        const string MESSAGE_11_02 = "四十五分";
        const string MESSAGE_11_03 = "過ぎ";

        const string MESSAGE_12_01 = "そろそろ";
        const string MESSAGE_12_02 = "時";


        public static void Main()
        {

            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            _azmdrawing = new AZMDrawing();

            screenWidth = _display.Width;
            screenHeight = _display.Height;

            displayMode = DISPLAY_MODE_BLACK;
            SetDisplayMode();

            //showAnimation = false;
            showDigital = false;

            currentTime = new DateTime();
            currentTime = DateTime.Now;

            dueTime = new TimeSpan(0, 0, 0, 59 - currentTime.Second, 1000 - currentTime.Millisecond);
            period = new TimeSpan(0, 0, 1, 0, 0);

            dueTimeDigital = new TimeSpan(0, 0, 0, 0, 1000 - currentTime.Millisecond);
            periodDigital = new TimeSpan(0, 0, 0, 1, 0);

            dueTimeAnimation = new TimeSpan(0, 0, 0, 0, 0);
            periodAnimation = new TimeSpan(0, 0, 0, 0, ANIMATION_INTERVAL);

            _random = new Random(currentTime.Millisecond);

            fluctuation = _random.Next(MAX_FLUCTUATION * 2);
            fluctuation = fluctuation - MAX_FLUCTUATION;

            buttonFlag = false;

            oldMessageNumber = 0;
            UpdateTime(null);

            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period);

            ButtonHelper.ButtonSetup = new Buttons[]
            {
                Buttons.BottomRight, Buttons.MiddleRight, Buttons.TopRight
            };

            ButtonHelper.Current.OnButtonPress += Current_OnButtonPress;

            Thread.Sleep(Timeout.Infinite);

        }

        private static void UpdateTime(object state)
        {

            currentTime = DateTime.Now;
            currentTime = currentTime.AddMinutes(fluctuation);

            int month = currentTime.Month;
            int day = currentTime.Day;
            int hour = currentTime.Hour;
            int minute = currentTime.Minute;

            string hourString = HourToString(hour % 12);

            bool drawFlag = false;

            if (buttonFlag == true && 1 <= oldMessageNumber )
            {

                messageNumber = oldMessageNumber;
                oldMessageNumber = 0;

            }
            else
            {

                //if (showAnimation == false && showDigital == false)
                //{


                if (month == 1 && day <= 3)
                {
                    messageNumber = 101;
                }
                else if (month == 6 && day == 20)
                {
                    messageNumber = 102;
                }
                else if (month == 12 && day == 31 && hour == 23)
                {
                    messageNumber = 103;
                }
                else if (hour == 15)
                {
                    messageNumber = 104;
                }
                else if (hour == 0 && minute <= 4)
                {
                    messageNumber = 105;
                }
                else if (hour == 23 && 56 <= minute)
                {
                    messageNumber = 105;
                }
                else if (0 <= minute && minute <= 3)
                {
                    messageNumber = 1;
                }
                else if (4 <= minute && minute <= 7)
                {
                    messageNumber = 2;
                }
                else if (8 <= minute && minute <= 11)
                {
                    messageNumber = 3;
                }
                else if (12 <= minute && minute <= 18)
                {
                    messageNumber = 4;
                }
                else if (19 <= minute && minute <= 22)
                {
                    messageNumber = 5;
                }
                else if (23 <= minute && minute <= 26)
                {
                    messageNumber = 6;
                }
                else if (27 <= minute && minute <= 33)
                {
                    messageNumber = 7;
                }
                else if (34 <= minute && minute <= 37)
                {
                    messageNumber = 8;
                }
                else if (38 <= minute && minute <= 41)
                {
                    messageNumber = 9;
                }
                else if (42 <= minute && minute <= 48)
                {
                    messageNumber = 10;
                }
                else if (49 <= minute && minute <= 52)
                {
                    messageNumber = 11;
                }
                else if (53 <= minute && minute <= 56)
                {
                    messageNumber = 12;
                    hourString = HourToString((hour + 1) % 12);
                }
                else if (57 <= minute)
                {
                    messageNumber = 99;
                    hourString = HourToString((hour + 1) % 12);
                }

            }

            if (oldMessageNumber != messageNumber)
            {

                if (oldMessageNumber == 99 && messageNumber == 1)
                {
                    drawFlag = false;
                    oldMessageNumber = 1;
                }
                else if (oldMessageNumber <= 99 && messageNumber < oldMessageNumber)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 1 && messageNumber == 99)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 1 && messageNumber == 104)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 2 && messageNumber == 105)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 101 && messageNumber == 103)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 105 && messageNumber == 101)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 102 && messageNumber == 105)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 105 && messageNumber == 102)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 103 && messageNumber == 99)
                {
                    drawFlag = false;
                }
                else if (oldMessageNumber == 101 && messageNumber == 103)
                {
                    drawFlag = false;
                }
                else
                {
                    drawFlag = true;
                }

            }
            else
            {
                drawFlag = false;
            }


            if (drawFlag == true)
            {

                if (buttonFlag != true)
                {
                    fluctuation = _random.Next(MAX_FLUCTUATION * 2);
                    fluctuation = fluctuation - MAX_FLUCTUATION;
                }
                else
                {
                    buttonFlag = false;
                }

                oldMessageNumber = messageNumber;

                switch (messageNumber)
                {

                    case 1:

                        lineNumbers = 3;
                        line1String = MESSAGE_01_01;
                        line2String = hourString + MESSAGE_01_02;
                        line3String = MESSAGE_01_03;

                        break;

                    case 2:

                        lineNumbers = 3;
                        line1String = hourString + MESSAGE_02_01;
                        line2String = MESSAGE_02_02;
                        line3String = MESSAGE_02_03;

                        break;

                    case 3:

                        lineNumbers = 3;
                        line1String = MESSAGE_03_01;
                        line2String = hourString + MESSAGE_03_02;
                        line3String = MESSAGE_03_03;

                        break;

                    case 4:

                        lineNumbers = 3;
                        line1String = hourString + MESSAGE_04_01;
                        line2String = MESSAGE_04_02;
                        line3String = MESSAGE_04_03;

                        break;

                    case 5:

                        lineNumbers = 3;
                        line1String = hourString + MESSAGE_05_01;
                        line2String = MESSAGE_05_02;
                        line3String = MESSAGE_05_03;

                        break;

                    case 6:

                        lineNumbers = 2;
                        line1String = MESSAGE_06_01;
                        line2String = hourString + MESSAGE_06_02;
                        line3String = "";

                        break;

                    case 7:

                        lineNumbers = 3;
                        line1String = MESSAGE_07_01;
                        line2String = hourString + MESSAGE_07_02;
                        line3String = MESSAGE_07_03;

                        break;

                    case 8:

                        lineNumbers = 3;
                        line1String = hourString + MESSAGE_08_01;
                        line2String = MESSAGE_08_02;
                        line3String = MESSAGE_08_03;

                        break;

                    case 9:

                        lineNumbers = 3;
                        line1String = MESSAGE_09_01;
                        line2String = hourString + MESSAGE_09_02;
                        line3String = MESSAGE_09_03;

                        break;

                    case 10:

                        lineNumbers = 3;
                        line1String = hourString + MESSAGE_10_01;
                        line2String = MESSAGE_10_02;
                        line3String = MESSAGE_10_03;

                        break;

                    case 11:

                        lineNumbers = 3;
                        line1String = hourString + MESSAGE_11_01;
                        line2String = MESSAGE_11_02;
                        line3String = MESSAGE_11_03;

                        break;

                    case 12:

                        lineNumbers = 2;
                        hourString = HourToString((hour + 1) % 12);
                        line1String = MESSAGE_12_01;
                        line2String = hourString + MESSAGE_12_02;
                        line3String = "";

                        break;

                    case 99:

                        lineNumbers = 2;
                        hourString = HourToString((hour + 1) % 12);
                        lineNumbers = 3;
                        line1String = MESSAGE_01_01;
                        line2String = hourString + MESSAGE_01_02;
                        line3String = MESSAGE_01_03;

                        break;

                    case 101:

                        lineNumbers = 3;
                        line1String = MESSAGE_101_01;
                        line2String = MESSAGE_101_02;
                        line3String = MESSAGE_101_03;

                        break;

                    case 102:

                        lineNumbers = 3;
                        line1String = MESSAGE_102_01;
                        line2String = MESSAGE_102_02;
                        line3String = MESSAGE_102_03;

                        break;

                    case 103:

                        lineNumbers = 3;
                        line1String = MESSAGE_103_01;
                        line2String = MESSAGE_103_02;
                        line3String = MESSAGE_103_03;

                        break;

                    case 104:

                        lineNumbers = 2;
                        line1String = MESSAGE_104_01;
                        line2String = MESSAGE_104_02;
                        line3String = "";

                        break;

                    case 105:

                        lineNumbers = 3;
                        line1String = MESSAGE_105_01;
                        line2String = MESSAGE_105_02;
                        line3String = MESSAGE_105_03;

                        break;

                }

                line1Left = screenWidth;

                if (messageNumber == 101 || messageNumber == 102)
                {
                    line2Left = 0 - _azmdrawing.GetStringWidth(line2String, fontSetoAJFC14);
                }
                else
                {
                    line2Left = 0 - _azmdrawing.GetStringWidth(line2String, fontSetoAJFC18);
                }

                line3Left = screenWidth;

                if (_updateAnimationTimer != null)
                {
                    _updateAnimationTimer.Dispose();
                }

                if (_updateDigitalTimer != null)
                {
                    _updateDigitalTimer.Dispose();
                    showDigital = false;
                }

                //showAnimation = true;
                _updateAnimationTimer = new Timer(UpdateTimeAnimation, null, dueTimeAnimation, periodAnimation);

            }

            //}

        }

        private static void UpdateTimeAnimation(object state)
        {

            int marginLeftLine1 = 0;
            int marginLeftLine2 = 0;
            int marginLeftLine3 = 0;

            _display.Clear();

            _display.DrawRectangle(colorBackground, 1, 0, 0, screenWidth, screenHeight, 0, 0, colorBackground, 0, 0, colorBackground, 0, 0, 255);

            if (messageNumber == 101 || messageNumber == 102)
            {

                marginLeftLine1 = (screenWidth - _azmdrawing.GetStringWidth(line1String, fontSetoAJFC14)) / 2;
                marginLeftLine2 = (screenWidth - _azmdrawing.GetStringWidth(line2String, fontSetoAJFC14)) / 2;
                marginLeftLine3 = (screenWidth - _azmdrawing.GetStringWidth(line3String, fontSetoAJFC14)) / 2;

                if (marginLeftLine1 < line1Left)
                {

                    line1Left = line1Left - SCROLL_WIDTH;

                    if (line1Left < marginLeftLine1)
                    {
                        line1Left = marginLeftLine1;
                    }

                }
                else if (line2Left < marginLeftLine2)
                {


                    line2Left = line2Left + SCROLL_WIDTH;

                    if (marginLeftLine2 < line2Left)
                    {
                        line2Left = marginLeftLine2;
                    }

                }

                else if (marginLeftLine3 < line3Left)
                {


                    line3Left = line3Left - SCROLL_WIDTH;

                    if (line3Left < marginLeftLine3)
                    {
                        line3Left = marginLeftLine3;
                    }

                }

                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC14, line1String, AZMDrawing.ALIGN_LEFT, line1Left, AZMDrawing.VALIGN_TOP, MARGIN_HEIGHT_3_LINES + 5);
                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC14, line2String, AZMDrawing.ALIGN_LEFT, line2Left, AZMDrawing.VALIGN_MIDDLE, 0);
                _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC14, line3String, AZMDrawing.ALIGN_LEFT, line3Left, AZMDrawing.VALIGN_BOTTOM, MARGIN_HEIGHT_3_LINES + 5);

                if (line3Left == marginLeftLine3)
                {
                    //showAnimation = false;
                    _updateAnimationTimer.Dispose();
                }

            }
            else
            {

                marginLeftLine1 = (screenWidth - _azmdrawing.GetStringWidth(line1String, fontSetoAJFC18)) / 2;
                marginLeftLine2 = (screenWidth - _azmdrawing.GetStringWidth(line2String, fontSetoAJFC18)) / 2;
                marginLeftLine3 = (screenWidth - _azmdrawing.GetStringWidth(line3String, fontSetoAJFC18)) / 2;

                switch (lineNumbers)
                {

                    case 2:

                        if (marginLeftLine1 < line1Left)
                        {


                            line1Left = line1Left - SCROLL_WIDTH;

                            if (line1Left < marginLeftLine1)
                            {
                                line1Left = marginLeftLine1;
                            }

                        }
                        else if (line2Left < marginLeftLine2)
                        {

                            line2Left = line2Left + SCROLL_WIDTH;

                            if (marginLeftLine2 < line2Left)
                            {
                                line2Left = marginLeftLine2;
                            }

                        }

                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC18, line1String, AZMDrawing.ALIGN_LEFT, line1Left, AZMDrawing.VALIGN_TOP, MARGIN_HEIGHT_2_LINES);
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC18, line2String, AZMDrawing.ALIGN_LEFT, line2Left, AZMDrawing.VALIGN_BOTTOM, MARGIN_HEIGHT_2_LINES);

                        if (line2Left == marginLeftLine2)
                        {
                            //showAnimation = false;
                            _updateAnimationTimer.Dispose();
                        }

                        break;

                    case 3:

                        if (marginLeftLine1 < line1Left)
                        {

                            line1Left = line1Left - SCROLL_WIDTH;

                            if (line1Left < marginLeftLine1)
                            {
                                line1Left = marginLeftLine1;
                            }

                        }
                        else if (line2Left < marginLeftLine2)
                        {

                            line2Left = line2Left + SCROLL_WIDTH;

                            if (marginLeftLine2 < line2Left)
                            {
                                line2Left = marginLeftLine2;
                            }

                        }

                        else if (marginLeftLine3 < line3Left)
                        {

                            line3Left = line3Left - SCROLL_WIDTH;

                            if (line3Left < marginLeftLine3)
                            {
                                line3Left = marginLeftLine3;
                            }

                        }

                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC18, line1String, AZMDrawing.ALIGN_LEFT, line1Left, AZMDrawing.VALIGN_TOP, MARGIN_HEIGHT_3_LINES);
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC18, line2String, AZMDrawing.ALIGN_LEFT, line2Left, AZMDrawing.VALIGN_MIDDLE, 0);
                        _azmdrawing.DrawStringAligned(_display, colorForeground, fontSetoAJFC18, line3String, AZMDrawing.ALIGN_LEFT, line3Left, AZMDrawing.VALIGN_BOTTOM, MARGIN_HEIGHT_3_LINES);

                        if (line3Left == marginLeftLine3)
                        {
                            //showAnimation = false;
                            _updateAnimationTimer.Dispose();
                        }

                        break;
                }

            }

            _display.Flush();

        }

        private static string HourToString(int hour)
        {

            string hourString = "";

            switch (hour % 12)
            {

                case 0:

                    hourString = "十二";
                    break;

                case 1:

                    hourString = "一";
                    break;

                case 2:

                    hourString = "二";
                    break;

                case 3:

                    hourString = "三";
                    break;

                case 4:

                    hourString = "四";
                    break;

                case 5:

                    hourString = "五";
                    break;

                case 6:

                    hourString = "六";
                    break;

                case 7:

                    hourString = "七";
                    break;

                case 8:

                    hourString = "八";
                    break;

                case 9:

                    hourString = "九";
                    break;

                case 10:

                    hourString = "十";
                    break;

                case 11:

                    hourString = "十一";
                    break;

            }

            return hourString;

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
                        SetDisplayMode();
                    }
                    else
                    {
                        showDigital = false;
                        buttonFlag = true;
                        UpdateTime(null);
                    }


                }
                else if (button == Buttons.MiddleRight)
                {

                    if (showDigital != true)
                    {
                        if (_updateAnimationTimer != null)
                        {
                            _updateAnimationTimer.Dispose();
                            //showAnimation = false;
                        }

                        showDigital = true;
                        showDigitalCounter = 0;
                        UpdateTimeDigital(null);
                        _updateDigitalTimer = new Timer(UpdateTimeDigital, null, dueTimeDigital, periodDigital);

                    }
                    else
                    {

                        showDigital = false;
                        buttonFlag = true;
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
                        SetDisplayMode();
                    }
                    else
                    {
                        showDigital = false;
                        buttonFlag = true;
                        UpdateTime(null);
                    }

                }

            }

        }

        private static void UpdateTimeDigital(object state)
        {

            currentTime = DateTime.Now;


            if (showDigital == false || showDigitalCounter > SHOW_DIGITAL_SECOND)
            {
                showDigital = false;
                buttonFlag = true;
                UpdateTime(null);
                _updateDigitalTimer.Dispose();
            }
            else
            {
                _azmdrawing.DrawDigitalClock(_display, Color.White, Color.Black, font7barPBd24, currentTime, true);
                _display.Flush();
                showDigitalCounter++;
            }

        }

        private static void SetDisplayMode()
        {

            switch (displayMode)
            {

                case DISPLAY_MODE_BLACK:

                    colorForeground = Color.White;
                    colorBackground = Color.Black;

                    break;

                case DISPLAY_MODE_WHITE:

                    colorForeground = Color.Black;
                    colorBackground = Color.White;

                    break;

            }
        }

    }

}

